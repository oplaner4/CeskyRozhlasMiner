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
    public class SignInAndGiveClaimsCommand : IRequest<bool>
    {
        public UserAuthenticateDto User { get; set; }
    }

    public class SignInAndGiveClaimsCommandHandler : CommandHandlerBase, IRequestHandler<SignInAndGiveClaimsCommand, bool>
    {
        public SignInAndGiveClaimsCommandHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(mediator, database, mapper, httpContextAccessor)
        {
        }

        public async Task<bool> Handle(SignInAndGiveClaimsCommand request, CancellationToken cancellationToken)
        {
            var dto = request.User;

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, dto.Email),
                new(ClaimTypes.NameIdentifier, dto.Id.ToString())
            };

            ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new(identity);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true,
            };

            await HttpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
            return true;
        }
    }
}
