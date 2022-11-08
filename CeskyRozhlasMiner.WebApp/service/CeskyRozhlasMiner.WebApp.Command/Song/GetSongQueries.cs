using AutoMapper;
using CeskyRozhlasMiner.Lib.Common;
using CeskyRozhlasMiner.Lib.Playlist;
using CeskyRozhlasMiner.Time;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.DSX.ProjectTemplate.Command.Playlist;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.Song
{
    public class GetAllSongsForPlaylist : IRequest<IEnumerable<Data.Models.Song>>
    {
        public int PlaylistId { get; set; }
    }

    public class GetSongsWithLimitForPlaylist : IRequest<GetSongsForPlaylistDto>
    {
        public int PlaylistId { get; set; }
        public int SongsLimit { get; set; }
    }

    public class GetCurrentlyPlayingSongs : IRequest<IEnumerable<SongDto>> { }

    public class SongQueryHandler : QueryHandlerBase,
        IRequestHandler<GetAllSongsForPlaylist, IEnumerable<Data.Models.Song>>,
        IRequestHandler<GetSongsWithLimitForPlaylist, GetSongsForPlaylistDto>,
        IRequestHandler<GetCurrentlyPlayingSongs, IEnumerable<SongDto>>
    {
        private PlaylistDto _playlist;
        private Dictionary<DateTime, FetchRange> _relevantStartsAndRanges;
        private HashSet<RozhlasStation> _requestedStations;
        private readonly List<Data.Models.Song> _fetchedSongs;
        private readonly object _extendFetchRangeLock = new();
        private readonly object _addRangeLock = new();
        private readonly object _fetchNewStationsLock = new();
        private readonly ITimeProvider _timeProvider;

        public SongQueryHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ITimeProvider timeProvider)
            : base(mediator, database, mapper, httpContextAccessor)
        {
            _fetchedSongs = new();
            _timeProvider = timeProvider;
        }

        /// <summary>
        /// Must be used in critical section.
        /// </summary>
        private async Task FetchSongs(DateTime from, DateTime to, IEnumerable<RozhlasStation> stations, CancellationToken cancellationToken)
        {
            List<Data.Models.Song> songsToSave = new();

            await foreach (var song in new PlaylistMiner(_timeProvider, from, to, stations).GetSongs().WithCancellation(cancellationToken))
            {
                if (song.PlayedAt >= from && song.PlayedAt <= to)
                {
                    var dbSong = Mapper.Map<Data.Models.Song>(song);
                    _fetchedSongs.Add(dbSong);
                    songsToSave.Add(dbSong);
                }
            }

            await Database.Songs.AddRangeAsync(songsToSave, cancellationToken);
        }

        private void LoadAvailableSongs(DateTime from, DateTime to, HashSet<RozhlasStation> stations)
        {
            foreach (var song in Database.Songs.Where(s => s.PlayedAt >= from && s.PlayedAt <= to && stations.Contains(s.SourceStation)))
            {
                _fetchedSongs.Add(song);
            }
        }

        /// <summary>
        /// Must be used in critical section.
        /// </summary>
        private async Task ExtendFetchRange(FetchRange range, CancellationToken cancellationToken)
        {
            DateTime wantedTo = range.From.AddDays(1).AddMilliseconds(-1);

            if (range.To == wantedTo)
            {
                return;
            }

            if (range.To < wantedTo)
            {
                await FetchSongs(range.To.AddMilliseconds(1), wantedTo, range.SourceStations.Select(x => x.Station), cancellationToken);
            }

            DateTime utcNow = _timeProvider.UtcNow;

            if (wantedTo > utcNow && range.To < utcNow)
            {
                range.To = utcNow;
            }
            else
            {
                range.To = wantedTo;
            }

            Database.FetchRanges.Update(range);
            await Database.SaveChangesAsync(cancellationToken);
        }


        /// <summary>
        /// Must be used in critical section.
        /// </summary>
        private async Task FetchNewStations(FetchRange range, CancellationToken cancellationToken)
        {
            var newStations = _requestedStations.Except(range.SourceStations.Select(x => x.Station));

            if (!newStations.Any())
            {
                return;
            }

            await FetchSongs(range.From, range.To, newStations, cancellationToken);

            foreach (var station in newStations)
            {
                range.SourceStations.Add(new() { Station = station });
            }

            await Database.SaveChangesAsync(cancellationToken);
        }


        private async Task Init(int playlistId, CancellationToken cancellationToken)
        {
            _playlist = await Mediator.Send(new GetPlaylistByIdQuery() { PlaylistId = playlistId }, cancellationToken);
            _relevantStartsAndRanges = await Database.FetchRanges.Where(r => r.From >= _playlist.From || r.To <= _playlist.To)
                .ToDictionaryAsync(x => x.From, x => x, cancellationToken);

            IEnumerable<DateTime> rangesStarts;

            if (_relevantStartsAndRanges.ContainsKey(_playlist.From))
            {
                rangesStarts = _relevantStartsAndRanges.Keys.Append(_playlist.From);
            }
            else
            {
                rangesStarts = _relevantStartsAndRanges.Keys;
            }

            _requestedStations = _playlist.SourceStations.Select(x => x.Station).ToHashSet();
        }

        // GET ALL FOR PLAYLIST
        public async Task<IEnumerable<Data.Models.Song>> Handle(GetAllSongsForPlaylist request, CancellationToken cancellationToken)
        {
            await Init(request.PlaylistId, cancellationToken);

            DateTime actual = _playlist.From;
            while (actual <= _playlist.To)
            {
                DateTime nextStart;

                if (_relevantStartsAndRanges.TryGetValue(actual, out var range))
                {
                    lock (_fetchNewStationsLock)
                    {
                        Task.WaitAll(new Task[] { FetchNewStations(range, cancellationToken) }, cancellationToken);
                    }

                    lock (_extendFetchRangeLock)
                    {
                        Task.WaitAll(new Task[] { ExtendFetchRange(range, cancellationToken) }, cancellationToken);
                    }

                    LoadAvailableSongs(actual, range.To < _playlist.To ? range.To : _playlist.To,
                        range.SourceStations.Select(x => x.Station).ToHashSet());
                    nextStart = range.From.AddDays(1);
                }
                else
                {
                    nextStart = actual.AddDays(1);
                    DateTime to = nextStart.AddMilliseconds(-1);

                    DateTime utcNow = _timeProvider.UtcNow;
                    FetchRange newRange = new()
                    {
                        From = actual,
                        To = to < utcNow ? to : utcNow,
                        SourceStations = _requestedStations.Select(x => new FetchRangeSourceStation() { Station = x }).ToList(),
                    };

                    lock (_addRangeLock)
                    {
                        var existingRange = Database.FetchRanges.FirstOrDefault(r => r.From == actual);
                        if (existingRange != null)
                        {
                            _relevantStartsAndRanges.Add(existingRange.From, existingRange);
                            continue;
                        }

                        Task.WaitAll(new Task[] { FetchSongs(actual, to, _requestedStations, cancellationToken) }, cancellationToken);
                        Database.FetchRanges.Add(newRange);
                        Database.SaveChanges();
                    }
                }

                actual = nextStart;
            }

            return _fetchedSongs.OrderByDescending(s => s.PlayedAt).ToList();
        }

        // GET SONGS FOR PLAYLIST WITH LIMIT
        public async Task<GetSongsForPlaylistDto> Handle(GetSongsWithLimitForPlaylist request, CancellationToken cancellationToken)
        {
            var songs = await Mediator.Send(new GetAllSongsForPlaylist() { PlaylistId = request.PlaylistId }, cancellationToken);
            return new GetSongsForPlaylistDto(songs.Select(s => Mapper.Map<SongDto>(s)).ToList(), request.SongsLimit);
        }

        // GET CURRENLTY PLAYING SONGS
        public async Task<IEnumerable<SongDto>> Handle(GetCurrentlyPlayingSongs request, CancellationToken cancellationToken)
        {
            List<SongDto> result = new();

            await foreach (var song in new PlaylistMiner(_timeProvider).GetSongsNow().WithCancellation(cancellationToken))
            {
                result.Add(Mapper.Map<SongDto>(song));
            }

            return result.OrderByDescending(s => s.PlayedAt).ThenBy(s => s.SourceStation);
        }
    }
}
