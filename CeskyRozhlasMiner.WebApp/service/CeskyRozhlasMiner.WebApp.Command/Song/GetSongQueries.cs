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
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.Song
{
    public class GetAllSongsForPlaylist : IRequest<IEnumerable<SongDto>>
    {
        public int PlaylistId { get; set; }
    }

    public class SongQueryHandler : QueryHandlerBase,
        IRequestHandler<GetAllSongsForPlaylist, IEnumerable<SongDto>>
    {
        public SongQueryHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(mediator, database, mapper, httpContextAccessor)
        {
        }

        private async IAsyncEnumerable<SongDto> FetchSongs(DateTime from, DateTime to, IEnumerable<RozhlasStation> stations)
        {
            await foreach (var song in new PlaylistMiner(from, to, stations).GetSongs(_ => { }))
            {
                var dbSong = Mapper.Map<Data.Models.Song>(song);
                Database.Songs.Add(dbSong);
                yield return Mapper.Map<SongDto>(dbSong);
            }
        }

        // GET ALL FOR PLAYLIST
        public async Task<IEnumerable<SongDto>> Handle(GetAllSongsForPlaylist request, CancellationToken cancellationToken)
        {
            var result = new List<SongDto>();
            var playlist = await Mediator.Send(new GetPlaylistByIdQuery() { PlaylistId = request.PlaylistId }, cancellationToken);
            var rangeStartAndRange = await Database.FetchRanges.Where(r => r.From >= playlist.From || r.To <= playlist.To)
                .ToDictionaryAsync(x => x.From, x => x, cancellationToken);
            var rangeStartsOrdered = rangeStartAndRange.Keys.Append(playlist.From).OrderBy(x => x).ToList();
            var rangeStartAndNext = Enumerable.Range(0, rangeStartsOrdered.Count - 1).ToDictionary(i => rangeStartsOrdered[i], i => rangeStartsOrdered[i + 1]);

            IEnumerable<RozhlasStation> requestedStations = playlist.SourceStations.Select(x => x.Station);

            DateTime inspected = playlist.From;
            while (inspected <= playlist.To)
            {
                var maxBound = playlist.To;

                if (rangeStartAndRange.TryGetValue(inspected, out var range))
                {
                    if (range.To < maxBound)
                    {
                        maxBound = range.To;
                    }

                    var availableStations = range.SourceStations.Select(x => x.Station).ToHashSet();

                    foreach (var song in Database.Songs.Where(s =>
                        s.PlayedAt >= range.From
                        && s.PlayedAt <= maxBound
                        && availableStations.Contains(s.SourceStation)
                        && requestedStations.Contains(s.SourceStation)))
                    {
                        result.Add(Mapper.Map<SongDto>(song));
                    }

                    var newStations = requestedStations.Where(x => !availableStations.Contains(x));

                    await foreach (var song in FetchSongs(range.From, maxBound, newStations).WithCancellation(cancellationToken))
                    {
                        result.Add(song);
                    }

                    foreach (var station in newStations)
                    {
                        range.SourceStations.Add(new() { Station = station });
                    }
                }
                else
                {
                    if (rangeStartAndNext.TryGetValue(inspected, out var nextStart) && nextStart < maxBound)
                    {
                        maxBound = nextStart.AddDays(-1);
                    }

                    await foreach (var song in FetchSongs(inspected, maxBound, requestedStations).WithCancellation(cancellationToken))
                    {
                        result.Add(Mapper.Map<SongDto>(song));
                    }

                    Database.FetchRanges.Add(new FetchRange()
                    {
                        From = inspected,
                        To = maxBound,
                        SourceStations = requestedStations.Select(x => new FetchRangeSourceStation() { Station = x }).ToList()
                    });
                }

                inspected = maxBound.AddDays(1);
            }

            await Database.SaveChangesAsync(cancellationToken);
            return result;
        }
    }
}
