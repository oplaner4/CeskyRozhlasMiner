using CeskyRozhlasMiner.WebApp.API.Immutable;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DSX.ProjectTemplate.Command.User;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.API.Controllers
{
    /// <summary>
    /// Controller for Google Oauth support.
    /// </summary>
    public class GoogleController : BaseController
    {
        private readonly SettingsAccessor _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleController"/> class.
        /// </summary>
        /// <param name="mediator">Mediator instance from dependency injection.</param>
        /// <param name="settings">App settings accessor from dependency injection.</param>
        public GoogleController(IMediator mediator, SettingsAccessor settings) : base(mediator)
        {
            _settings = settings;
        }

        /// <summary>
        /// Get Google client id for oauth.
        /// </summary>
        [HttpGet]
        public ActionResult<string> GetClientId()
        {
            return Ok(_settings.Own.Google.ClientId);
        }

        /// <summary>
        /// Sign user in by Google auth provider.
        /// </summary>
        /// <param name="dto">A GoogleSignInData DtO.</param>
        [HttpPost]
        public async Task<ActionResult<UserDto>> SignInUser([FromBody] GoogleSignInDataDto dto)
        {
            return Ok(await Mediator.Send(new SignInViaGoogleCommand()
            {
                GoogleData = dto,
                GoogleClientId = _settings.Own.Google.ClientId,
            }));
        }

    }
}
