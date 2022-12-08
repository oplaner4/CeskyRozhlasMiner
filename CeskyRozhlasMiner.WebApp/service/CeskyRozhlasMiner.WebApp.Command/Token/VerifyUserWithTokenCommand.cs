using AutoMapper;
using CeskyRozhlasMiner.Time;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.Token
{
    public class VerifyUserWithTokenCommand : IRequest<UserDto>
    {
        public TokenDto Token { get; set; }

        public int TokenExpirationMinutes { get; set; }
    }

    public class VerifyUserWithTokenCommandHandler : QueryHandlerBase,
        IRequestHandler<VerifyUserWithTokenCommand, UserDto>
    {
        private readonly ITimeProvider _timeProvider;

        public VerifyUserWithTokenCommandHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ITimeProvider timeProvider)
            : base(mediator, database, mapper, httpContextAccessor)
        {
            _timeProvider = timeProvider;
        }

        public async Task<UserDto> Handle(VerifyUserWithTokenCommand request, CancellationToken cancellationToken)
        {
            var token = await Database.Tokens.Include(t => t.Owner).FirstOrDefaultAsync(t => t.Value == request.Token.Value, cancellationToken);

            if (token == null)
            {
                throw new EntityNotFoundException($"{nameof(Data.Models.Token)} not found.");
            }

            if (token.CreatedDate.AddMinutes(request.TokenExpirationMinutes) < _timeProvider.UtcNow)
            {
                throw new NotAcceptableException($"{nameof(Data.Models.Token)} has expired.");
            }

            token.UsedCount++;
            token.Owner.Verified = true;

            Database.Update(token);
            Database.Update(token.Owner);
            await Database.SaveChangesAsync(cancellationToken);

            return Mapper.Map<UserDto>(token.Owner);
        }
    }
}
