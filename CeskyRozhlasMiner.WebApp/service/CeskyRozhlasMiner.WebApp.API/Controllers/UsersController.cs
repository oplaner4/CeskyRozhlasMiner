using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DSX.ProjectTemplate.Command.Group;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.API.Controllers
{
    /// <summary>
    /// Controller for User APIs.
    /// </summary>
    public class UsersController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="mediator">Mediator instance from dependency injection.</param>
        public UsersController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// Get actually signed User.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetUser()
        {
            return Ok(await Mediator.Send(new GetUserQuery()));
        }

        /// <summary>
        /// Create a new User.
        /// </summary>
        /// <param name="dto">A UserCreate DTO.</param>
        [HttpPost]
        public async Task<ActionResult<GroupDto>> CreateUser([FromBody] UserCreateDto dto)
        {
            UserDto created = await Mediator.Send(new CreateUserCommand() { User = dto });

            var claims = new List<Claim>
            {
                new Claim("Id", created.Id.ToString()),
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
                
            return Ok(created);
        }
    }
}
