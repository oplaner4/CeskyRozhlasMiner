using AutoMapper;
using CeskyRozhlasMiner.WebApp.Data.Utilities;
using Google.Apis.Auth.OAuth2;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.User
{
    public class SignInViaGoogleCommand : IRequest<UserDto>
    {
        public GoogleSignInDataDto GoogleData { get; set; }
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
            var handler = new JwtSecurityTokenHandler();

            JwtSecurityToken token;

            try
            {
                token = handler.ReadJwtToken(dto.Credential);
            }
            catch
            {
                // Should not happen
                throw new BadRequestException("Invalid credential");
            }

            string email = (string)token.Payload["email"];
            var user = await Database.Users.FirstOrDefaultAsync(u => !u.Deleted && u.Email == email, cancellationToken);

            UserDto result = null;

            if (user == null)
            {
                string password = Guid.NewGuid().ToString();

                result = await Mediator.Send(new CreateUserCommand()
                {
                    User = new()
                        {
                            Email = email,
                            DisplayName = (string)token.Payload["name"],
                            NewPassword = password,
                            NewPasswordConfirm = password,
                        }
                    }, cancellationToken);
            }
            else
            {
                result = Mapper.Map<UserDto>(user);
            }

            await Mediator.Send(new SignInAndGiveClaimsCommand()
            {
                User = result,
            }, cancellationToken);

            return result;
        }
    }
}
