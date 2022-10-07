using AutoMapper;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.User
{
    public class SignInViaGoogleCommand : IRequest<UserDto>
    {
        public GoogleSignInDataDto GoogleData { get; set; }
        public string GoogleClientId { get; set; }
    }

    public class SignInViaGoogleCommandHandler : CommandHandlerBase, IRequestHandler<SignInViaGoogleCommand, UserDto>
    {
        public SignInViaGoogleCommandHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(mediator, database, mapper, httpContextAccessor)
        {
        }

        public async Task<UserDto> Handle(SignInViaGoogleCommand request, CancellationToken cancellationToken)
        {
            Thread.Sleep(500);

            var dto = request.GoogleData;

            GoogleJsonWebSignature.Payload payload;

            try
            {
                var validationSettings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new string[] { request.GoogleClientId },
                };

                payload = await GoogleJsonWebSignature.ValidateAsync(dto.Credential, validationSettings);

            }
            catch (InvalidJwtException)
            {
                throw new UnauthorizedAccessException("Invalid credential was provided.");
            }

            var user = await Database.Users.FirstOrDefaultAsync(u => !u.Deleted && u.Email == payload.Email, cancellationToken);

            UserDto result = null;

            if (user == null)
            {
                string password = payload.Email + Guid.NewGuid().ToString();

                result = await Mediator.Send(new CreateUserCommand()
                {
                    User = new()
                    {
                        Email = payload.Email,
                        DisplayName = payload.Name,
                    },
                    GeneratePasswordHash = false,
                }, cancellationToken);
            }
            else
            {
                result = Mapper.Map<UserDto>(user);
            }

            await Mediator.Send(new GiveClaimsCommand()
            {
                User = result,
            }, cancellationToken);

            return result;
        }
    }
}
