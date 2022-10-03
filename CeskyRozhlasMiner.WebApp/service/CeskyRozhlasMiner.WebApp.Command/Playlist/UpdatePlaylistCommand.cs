using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Exceptions;
using Microsoft.DSX.ProjectTemplate.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.Playlist
{
    public class UpdatePlaylistCommand : IRequest<PlaylistDto>
    {
        public PlaylistDto Playlist { get; set; }
    }

    public class UpdatePlaylistCommandHandler : CommandHandlerBase,
        IRequestHandler<UpdatePlaylistCommand, PlaylistDto>
    {
        public UpdatePlaylistCommandHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(mediator, database, mapper, httpContextAccessor)
        {
        }

        public async Task<PlaylistDto> Handle(UpdatePlaylistCommand request, CancellationToken cancellationToken)
        {
            EnsureSignedIn();

            var playlist = await Database.Playlists.FindAsync(new object[] { request.Playlist.Id }, cancellationToken);
            if (playlist == null)
            {
                throw new EntityNotFoundException($"{nameof(Data.Models.Playlist)} not found.");
            }

            if (playlist.OwnerId != UserId)
            {
                throw new UnauthorizedException("Unauthorized access");
            }

            var dto = request.Playlist;

            playlist.Name = dto.Name;
            playlist.From = dto.From;
            playlist.To = dto.To;

            Dictionary<int, PlaylistSourceStation> idAndStation = playlist.SourceStations.ToDictionary(x => x.Id, x => x);

            playlist.SourceStations = dto.SourceStations.DistinctBy(x => x.Id).DistinctBy(x => x.Station).ToDictionary(x => x.Id, x => x).Select(pair =>
            {
                if (idAndStation.TryGetValue(pair.Key, out var value))
                {
                    value.Station = pair.Value.Station;
                    value.Description = pair.Value.Description;
                    return value;
                }

                return new PlaylistSourceStation()
                {
                    Description = pair.Value.Description,
                    Station = pair.Value.Station,
                };
            }).ToList();

            await Database.SaveChangesAsync(cancellationToken);

            return Mapper.Map<PlaylistDto>(playlist);
        }
    }
}
