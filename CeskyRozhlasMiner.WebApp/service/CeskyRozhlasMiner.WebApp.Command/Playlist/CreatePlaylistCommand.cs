using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Events;
using Microsoft.DSX.ProjectTemplate.Data.Exceptions;
using Microsoft.DSX.ProjectTemplate.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.Playlist
{
    public class CreatePlaylistCommand : IRequest<PlaylistDto>
    {
        public PlaylistDto Playlist { get; set; }
    }

    public class CreatePlaylistCommandHandler : CommandHandlerBase,
        IRequestHandler<CreatePlaylistCommand, PlaylistDto>
    {
        public CreatePlaylistCommandHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(mediator, database, mapper, httpContextAccessor)
        {
        }

        public async Task<PlaylistDto> Handle(CreatePlaylistCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Playlist;

            bool nameAlreadyUsed = await Database.Playlists.AnyAsync(e => !e.Deleted && e.OwnerId == UserId && e.Name.Trim() == dto.Name.Trim(), cancellationToken);

            if (nameAlreadyUsed)
            {
                throw new NotAcceptableException($"{nameof(dto.Name)} '{dto.Name}' already used.");
            }

            var model = new Data.Models.Playlist()
            {
                Name = dto.Name,
                From = dto.From,
                To = dto.To,
                SourceStations = dto.SourceStations.Select(st => new PlaylistSourceStation()
                {
                    Description = st.Description,
                    Station = st.Station,
                }).ToList(),
                OwnerId = UserId,
            };

            Database.Playlists.Add(model);

            await Database.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new PlaylistCreatedDomainEvent(model), cancellationToken);

            return Mapper.Map<PlaylistDto>(model);
        }
    }
}
