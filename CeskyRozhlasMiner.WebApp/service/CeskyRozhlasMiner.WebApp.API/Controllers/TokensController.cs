using CeskyRozhlasMiner.WebApp.API.Immutable;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DSX.ProjectTemplate.Command.Token;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.API.Controllers
{
    /// <summary>
    /// Controller for Token APIs.
    /// </summary>
    public class TokensController : BaseController
    {
        private readonly SettingsAccessor _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokensController"/> class.
        /// </summary>
        /// <param name="mediator">Mediator instance from dependency injection.</param>
        /// <param name="settings">App settings accessor from dependency injection.</param>
        public TokensController(IMediator mediator, SettingsAccessor settings) : base(mediator)
        {
            _settings = settings;
        }

        /// <summary>
        /// Verify user with token.
        /// </summary>
        /// <param name="dto">Token DTO.</param>
        [HttpPost]
        public async Task<ActionResult<UserDto>> VerifyUser([FromBody] TokenDto dto)
        {
            return Ok(await Mediator.Send(new VerifyUserWithTokenCommand()
            {
                Token = dto,
                TokenExpirationMinutes = _settings.Own.TokenExpirationMinutes,
            }));
        }

        /// <summary>
        /// Send a new token for verification of user.
        /// </summary>
        [Authorize]
        [HttpPut]
        public async Task<ActionResult<bool>> SendNewToken()
        {
            return Ok(await Mediator.Send(new SendNewTokenCommand()));
        }
    }
}
