using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.Playlist
{
    public class GetAllUserPlaylistsQuery : IRequest<IEnumerable<PlaylistDto>> { }

    public class GetPlaylistByIdQuery : IRequest<PlaylistDto>
    {
        public int PlaylistId { get; set; }
    }

    public class PlaylistQueryHandler : QueryHandlerBase,
        IRequestHandler<GetAllUserPlaylistsQuery, IEnumerable<PlaylistDto>>,
        IRequestHandler<GetPlaylistByIdQuery, PlaylistDto>
    {
        public PlaylistQueryHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(mediator, database, mapper, httpContextAccessor)
        {
        }

        // GET ALL FOR USER
        public async Task<IEnumerable<PlaylistDto>> Handle(GetAllUserPlaylistsQuery request, CancellationToken cancellationToken)
        {
            EnsureSignedIn();

            return await Database.Playlists
                .Where(x => !x.Deleted && x.OwnerId == UserId)
                .Select(x => Mapper.Map<PlaylistDto>(x))
                .ToListAsync(cancellationToken);
        }

        // GET BY ID
        public async Task<PlaylistDto> Handle(GetPlaylistByIdQuery request, CancellationToken cancellationToken)
        {
            EnsureSignedIn();

            if (request.PlaylistId <= 0)
            {
                throw new BadRequestException($"A valid {nameof(Data.Models.Playlist)} Id must be provided.");
            }

            var innerResult = await Database.Playlists.FindAsync(new object[] { request.PlaylistId }, cancellationToken);
            if (innerResult == null || innerResult.Deleted)
            {
                throw new EntityNotFoundException($"{nameof(Data.Models.Playlist)} with Id {request.PlaylistId} cannot be found.");
            }

            if (innerResult.OwnerId != UserId)
            {
                throw new UnauthorizedException("Unauthorized access");
            }

            return Mapper.Map<PlaylistDto>(innerResult);
        }
    }
}
