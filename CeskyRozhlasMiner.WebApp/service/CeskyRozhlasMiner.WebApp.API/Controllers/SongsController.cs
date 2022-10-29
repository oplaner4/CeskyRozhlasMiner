using CeskyRozhlasMiner.WebApp.API.Immutable;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DSX.ProjectTemplate.Command.Song;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.API.Controllers
{
    /// <summary>
    /// Controller for Song APIs.
    /// </summary>
    public class SongsController : BaseController
    {
        private readonly SettingsAccessor _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SongsController"/> class.
        /// </summary>
        /// <param name="mediator">Mediator instance from dependency injection.</param>
        /// <param name="settings">App settings accessor from dependency injection.</param>
        public SongsController(IMediator mediator, SettingsAccessor settings) : base(mediator) {
            _settings = settings;
        }

        /// <summary>
        /// Get currently playing Songs.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SongDto>>> GetCurrentlyPlayingSongs()
        {
            return Ok(await Mediator.Send(new GetCurrentlyPlayingSongs()));
        }

        /// <summary>
        /// Get Songs for playlist.
        /// </summary>
        /// <param name="id">ID of the Playlist to use.</param>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<GetSongsForPlaylistDto>> GetAllSongsForPlaylist(int id)
        {
            return Ok(await Mediator.Send(new GetSongsWithLimitForPlaylist()
            {
                PlaylistId = id,
                SongsLimit = _settings.Own.SongsFetchLimit,
            }));
        }
    }
}
