using AutoMapper;
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
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.User
{
    public class SignInUserCommand : IRequest<UserDto>
    {
        public UserAuthenticateDto User { get; set; }
    }

    public class SignInUserCommandHandler : CommandHandlerBase, IRequestHandler<SignInUserCommand, UserDto>
    {
        public SignInUserCommandHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(mediator, database, mapper, httpContextAccessor)
        {
        }

        public async Task<UserDto> Handle(SignInUserCommand request, CancellationToken cancellationToken)
        {
            Thread.Sleep(500);
            
            var dto = request.User;

            var user = await Database.Users.FirstOrDefaultAsync(u => !u.Deleted && u.Email == dto.Email, cancellationToken);

            if (user == null)
            {
                throw new EntityNotFoundException($"{nameof(Data.Models.User)} cannot be found.");
            }

            var verification = new PasswordHasher<Data.Models.User>().VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (verification == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException($"Invalid credentials were provided.");
            }

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.Email),
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new(identity);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true,
            };

            await HttpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return Mapper.Map<UserDto>(user);
        }
    }
}
