using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.Group
{
    public class GetUserQuery : IRequest<UserDto> { }

    public class GetUserQueryHandler : QueryHandlerBase, IRequestHandler<GetUserQuery, UserDto>
    {
        public GetUserQueryHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IAuthorizationService authorizationService)
            : base(mediator, database, mapper, authorizationService)
        {
        }

        public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var innerResult = await Database.Users.FindAsync(new object[] { }, cancellationToken);
            if (innerResult == null)
            {
                throw new EntityNotFoundException($"{nameof(Data.Models.User)} cannot be found.");
            }

           
            
            return Mapper.Map<UserDto>(innerResult);
        }
    }
}
