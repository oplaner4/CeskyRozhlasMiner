using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.State;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.User
{
    public class SignOutUserCommand : IRequest<bool> { }

    public class SignOutUserCommandHandler : CommandHandlerBase, IRequestHandler<SignOutUserCommand, bool>
    {
        public SignOutUserCommandHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(mediator, database, mapper, httpContextAccessor)
        {
        }

        public Task<bool> Handle(SignOutUserCommand request, CancellationToken cancellationToken)
        {
            Manipulator.SetUserId(SessionManipulator.UserIdUnset);
            return Task.FromResult(true);
        }
    }
}
