using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.DSX.ProjectTemplate.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.Token
{
    public class SendNewTokenCommand : IRequest<bool> { }

    public class SendNewTokenCommandHandler : QueryHandlerBase,
        IRequestHandler<SendNewTokenCommand, bool>
    {
        public SendNewTokenCommandHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(mediator, database, mapper, httpContextAccessor)
        { }

        public async Task<bool> Handle(SendNewTokenCommand request, CancellationToken cancellationToken)
        {
            var token = await Mediator.Send(new GenerateTokenCommand(), cancellationToken);
            await Task.CompletedTask;
            return true;
        }
    }
}
