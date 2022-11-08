using AutoMapper;
using CeskyRozhlasMiner.Lib.Playlist;
using CeskyRozhlasMiner.Lib.Playlist.DataProcessing;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.DSX.ProjectTemplate.Command.Song;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.Statistics
{
    public class GetStatisticsForPlaylist : IRequest<GetStatisticsForPlaylistDto>
    {
        public int PlaylistId { get; set; }
    }

    public class StatisticsQueryHandler : QueryHandlerBase,
        IRequestHandler<GetStatisticsForPlaylist, GetStatisticsForPlaylistDto>
    {
        public StatisticsQueryHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(mediator, database, mapper, httpContextAccessor)
        {
        }

        // GET STATISTICS FOR PLAYLIST
        public async Task<GetStatisticsForPlaylistDto> Handle(GetStatisticsForPlaylist request, CancellationToken cancellationToken)
        {
            IEnumerable<Data.Models.Song> songs = await Mediator.Send(new GetAllSongsForPlaylist()
            {
                PlaylistId = request.PlaylistId
            }, cancellationToken);

            var result = new GetStatisticsForPlaylistDto();

            if (!songs.Any())
            {
                result.NoSourceData = true;
                return result;
            }

            PlaylistStatistics source = new(songs.Select(s =>
                new PlaylistSong(s.Artist, s.Title, s.PlayedAt, s.SourceStation)
            ));

            var mostFrequentArtistPair = source.GetMostFrequentArtist();
            result.MostFrequentArtist = new(mostFrequentArtistPair.Key, mostFrequentArtistPair.Value);

            var mostPlayedSongPair = source.GetMostPlayedSong();
            result.MostPlayedSong = new(mostPlayedSongPair.Key, mostPlayedSongPair.Value);

            var leaderBoard = source.GetLeaderBoard();
            result.LeaderBoard = leaderBoard.Select(s => new StatisticsEntryDto(s.Key, s.Value));

            return result;
        }
    }
}
