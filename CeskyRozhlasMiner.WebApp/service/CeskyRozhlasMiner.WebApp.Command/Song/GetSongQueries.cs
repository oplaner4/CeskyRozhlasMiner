using AutoMapper;
using CeskyRozhlasMiner.Lib.Common;
using CeskyRozhlasMiner.Lib.Playlist;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.DSX.ProjectTemplate.Command.Playlist;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.Song
{
    public class GetAllSongsForPlaylist : IRequest<GetDataForPlaylistDto>
    {
        public int PlaylistId { get; set; }

        public int SongsLimit { get; set; }
    }

    public class SongQueryHandler : QueryHandlerBase,
        IRequestHandler<GetAllSongsForPlaylist, GetDataForPlaylistDto>
    {
        private PlaylistDto _playlist;
        private Dictionary<DateTime, FetchRange> _relevantStartsAndRanges;
        private IEnumerator<DateTime> _relevantStartsEnumerator;
        private HashSet<RozhlasStation> _requestedStations;
        private readonly List<Data.Models.Song> _alreadyFetchedSongs;
        private readonly List<Data.Models.Song> _rightNowFetchedSongs;

        public SongQueryHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(mediator, database, mapper, httpContextAccessor)
        {
            _alreadyFetchedSongs = new();
            _rightNowFetchedSongs = new();
        }

        private async Task FetchSongs(DateTime from, DateTime to, IEnumerable<RozhlasStation> stations, CancellationToken cancellationToken)
        {
            await foreach (var song in new PlaylistMiner(from, to, stations).GetSongs().WithCancellation(cancellationToken))
            {
                _rightNowFetchedSongs.Add(Mapper.Map<Data.Models.Song>(song));
            }
        }

        private void LoadAvailableSongs(DateTime from, DateTime to, IEnumerable<RozhlasStation> stations)
        {
            foreach (var song in Database.Songs.Where(s => s.PlayedAt >= from && s.PlayedAt <= to && stations.Contains(s.SourceStation)))
            {
                _alreadyFetchedSongs.Add(Mapper.Map<Data.Models.Song>(song));
            }
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

            _relevantStartsEnumerator = rangesStarts.GetEnumerator();
            _relevantStartsEnumerator.MoveNext();
            _requestedStations = _playlist.SourceStations.Select(x => x.Station).ToHashSet();
        }

        // GET ALL FOR PLAYLIST
        public async Task<GetDataForPlaylistDto> Handle(GetAllSongsForPlaylist request, CancellationToken cancellationToken)
        {
            await Init(request.PlaylistId, cancellationToken);

            DateTime actual = _playlist.From;
            while (actual <= _playlist.To)
            {
                DateTime nextStart;

                if (_relevantStartsAndRanges.TryGetValue(actual, out var range))
                {
                    nextStart = range.To.AddMilliseconds(1);

                    var availableStations = range.SourceStations.Select(x => x.Station).ToHashSet();
                    LoadAvailableSongs(actual, range.To < _playlist.To ? range.To : _playlist.To, availableStations);

                    var newStations = _requestedStations.Except(availableStations);
                    await FetchSongs(range.From, range.To, newStations, cancellationToken);

                    foreach (var station in newStations)
                    {
                        range.SourceStations.Add(new() { Station = station });
                    }
                }
                else
                {
                    nextStart = actual.AddDays(1);
                    DateTime to = nextStart.AddMilliseconds(-1);

                    await FetchSongs(actual, to, _requestedStations, cancellationToken);

                    Database.FetchRanges.Add(new FetchRange()
                    {
                        From = actual,
                        To = to,
                        SourceStations = _requestedStations.Select(x => new FetchRangeSourceStation() { Station = x }).ToList(),
                    });
                }

                actual = nextStart;
            }

            await Database.Songs.AddRangeAsync(_rightNowFetchedSongs, cancellationToken);
            await Database.SaveChangesAsync(cancellationToken);

            var result = _alreadyFetchedSongs.Concat(_rightNowFetchedSongs.Where(s => s.PlayedAt >= _playlist.From && s.PlayedAt <= _playlist.To
                && _requestedStations.Contains(s.SourceStation)));

            return new(result.OrderByDescending(s => s.PlayedAt).Select(s => Mapper.Map<SongDto>(s)).Take(request.SongsLimit + 1).ToList(),
                request.SongsLimit);
        }
    }
}
