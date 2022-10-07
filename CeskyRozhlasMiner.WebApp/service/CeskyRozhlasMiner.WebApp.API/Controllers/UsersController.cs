using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DSX.ProjectTemplate.Command.User;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
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
        /// Signs User in.
        /// </summary>
        /// <param name="dto">A UserAuthenticate dto</param>
        [HttpPost(nameof(SignInUser))]
        public async Task<ActionResult<UserDto>> SignInUser([FromBody] UserAuthenticateDto dto)
        {
            return Ok(await Mediator.Send(new SignInUserCommand()
            {
                User = dto
            }));
        }

        /// <summary>
        /// Signs User out.
        /// </summary>
        [HttpPost(nameof(SignOutUser))]
        public ActionResult<bool> SignOutUser()
        {
            return Ok(Mediator.Send(new SignOutUserCommand()));
        }

        /// <summary>
        /// Create a new User.
        /// </summary>
        /// <param name="dto">A UserSet DTO.</param>
        [HttpPost(nameof(CreateUser))]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserSetDto dto)
        {
            return Ok(await Mediator.Send(new CreateUserCommand() { User = dto, GeneratePasswordHash = true, }));
        }

        /// <summary>
        /// Update existing User.
        /// </summary>
        /// <param name="dto">A UserSet DTO.</param>
        [HttpPut]
        public async Task<ActionResult<UserDto>> UpdateUser([FromBody] UserSetDto dto)
        {
            return Ok(await Mediator.Send(new UpdateUserCommand() { User = dto }));
        }

        /// <summary>
        /// Delete existing User.
        /// </summary>
        /// <param name="dto">A UserAuthenticate DTO.</param>
        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteUser([FromBody] UserAuthenticateDto dto)
        {
            return Ok(await Mediator.Send(new DeleteUserCommand() { User = dto }));
        }
    }
}
