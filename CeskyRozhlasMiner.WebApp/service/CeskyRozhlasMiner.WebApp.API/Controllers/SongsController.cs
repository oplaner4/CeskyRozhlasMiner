using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DSX.ProjectTemplate.Command.Song;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.API.Controllers
{
    /// <summary>
    /// Controller for Song APIs.
    /// </summary>
    public class SongsController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SongsController"/> class.
        /// </summary>
        /// <param name="mediator">Mediator instance from dependency injection.</param>
        public SongsController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// Get Songs for playlist.
        /// </summary>
        /// <param name="id">ID of the Playlist to use.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<SongDto>>> GetAllSongsForPlaylist(int id)
        {
            return Ok(await Mediator.Send(new GetAllSongsForPlaylist()
            {
                PlaylistId = id,
            }));
        }
    }
}
