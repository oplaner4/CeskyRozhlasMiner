using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DSX.ProjectTemplate.Command.Statistics;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.API.Controllers
{
    /// <summary>
    /// Controller for Playlist statistics APIs.
    /// </summary>
    [Authorize]
    public class StatisticsController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsController"/> class.
        /// </summary>
        /// <param name="mediator">Mediator instance from dependency injection.</param>
        public StatisticsController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// Get statistics for playlist.
        /// </summary>
        /// <param name="id">ID of the Playlist to use.</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<GetStatisticsForPlaylistDto>> GetStatisticsForPlaylist(int id)
        {
            return Ok(await Mediator.Send(new GetStatisticsForPlaylist()
            {
                PlaylistId = id,
            }));
        }
    }
}
