﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.User
{
    public class GetUserQuery : IRequest<UserDto> { }

    public class GetUserByEmailQuery : IRequest<UserDto>
    {
        public string Email { get; set; }
    }

    public class GetUserQueryHandler : QueryHandlerBase, IRequestHandler<GetUserQuery, UserDto>
    {
        public GetUserQueryHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(mediator, database, mapper, httpContextAccessor)
        {
        }

        public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            if (!HttpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                throw new NotAuthenticatedException("Not signed in.");
            }

            var innerResult = await Database.Users.FindAsync(new object[] { UserId }, cancellationToken);

            if (innerResult == null || innerResult.Deleted)
            {
                throw new EntityNotFoundException($"{nameof(Data.Models.User)} cannot be found.");
            }

            return Mapper.Map<UserDto>(innerResult);
        }
    }
}
