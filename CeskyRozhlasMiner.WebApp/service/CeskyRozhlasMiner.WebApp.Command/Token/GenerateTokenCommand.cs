using AutoMapper;
using CeskyRozhlasMiner.WebApp.Data.Utilities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.DSX.ProjectTemplate.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.Token
{
    public class GenerateTokenCommand : IRequest<Data.Models.Token> { }

    public class GenerateTokenCommandHandler : QueryHandlerBase,
        IRequestHandler<GenerateTokenCommand, Data.Models.Token>
    {
        public GenerateTokenCommandHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(mediator, database, mapper, httpContextAccessor)
        { }

        public async Task<Data.Models.Token> Handle(GenerateTokenCommand request, CancellationToken cancellationToken)
        {

            var token = new Data.Models.Token()
            {
                Value = TokenValueGenerator.GetNewValue(),
                OwnerId = UserId,
                UsedCount = 0,
            };

            Database.Tokens.Add(token);
            await Database.SaveChangesAsync(cancellationToken);
            return token;
        }
    }
}
