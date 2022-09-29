using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DSX.ProjectTemplate.Command.Group;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CeskyRozhlasMiner.WebApp.API.State;

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
        /// Signs User in.
        /// </summary>
        /// <param name="dto">A UserAuthenticate dto</param>
        /// <returns></returns>
        [HttpPost(nameof(SignUserIn))]
        public async Task<ActionResult<UserDto>> SignUserIn([FromBody] UserAuthenticateDto dto)
        {
            return Ok(await Mediator.Send(new SignInUserCommand()
            {
                User = dto
            }));
        }

        /// <summary>
        /// Signs User out.
        /// </summary>
        /// <returns></returns>
        [HttpPost(nameof(SignUserOut))]
        public ActionResult<bool> SignUserOut()
        {
            new SessionManipulator(HttpContext.Session).SetUserId(SessionManipulator.UserIdNotFound);
            return Ok(true);
        }


        /// <summary>
        /// Create a new User.
        /// </summary>
        /// <param name="dto">A UserCreate DTO.</param>
        [HttpPost(nameof(CreateUser))]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserCreateDto dto)
        {
            UserDto created = await Mediator.Send(new CreateUserCommand() { User = dto });
            return Ok(created);
        }
    }
}
