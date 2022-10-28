using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.Exceptions;
using Microsoft.DSX.ProjectTemplate.Data.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.Playlist
{
    public class DeletePlaylistCommand : IRequest<bool>
    {
        public int PlaylistId { get; set; }
    }

    public class DeletePlaylistCommandHandler : CommandHandlerBase,
        IRequestHandler<DeletePlaylistCommand, bool>
    {
        public DeletePlaylistCommandHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(mediator, database, mapper, httpContextAccessor)
        {
        }

        public async Task<bool> Handle(DeletePlaylistCommand request, CancellationToken cancellationToken)
        {
            if (request.PlaylistId <= 0)
            {
                throw new BadRequestException($"A valid {nameof(Data.Models.Playlist)} Id must be provided.");
            }

            var playlist = await Database.Playlists.FindAsync(new object[] { request.PlaylistId }, cancellationToken);
            if (playlist == null)
            {
                throw new EntityNotFoundException($"{nameof(Data.Models.Playlist)} not found.");
            }

            if (playlist.OwnerId != UserId)
            {
                throw new ForbiddenException("Unauthorized access");
            }

            playlist.Deleted = true;
            Database.Playlists.Update(playlist);
            await Database.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
