using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DSX.ProjectTemplate.Command.Playlist;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.API.Controllers
{
    /// <summary>
    /// Controller for Playlist APIs.
    /// </summary>
    [Authorize]
    public class PlaylistsController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistsController"/> class.
        /// </summary>
        /// <param name="mediator">Mediator instance from dependency injection.</param>
        public PlaylistsController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// Get all User Playlists.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlaylistDto>>> GetAllUserPlaylists()
        {
            return Ok(await Mediator.Send(new GetAllUserPlaylistsQuery()));
        }

        /// <summary>
        /// Get a Playlist by its Id.
        /// </summary>
        /// <param name="id">ID of the Playlist to get.</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<PlaylistDto>> GetPlaylist(int id)
        {
            return Ok(await Mediator.Send(new GetPlaylistByIdQuery() { PlaylistId = id }));
        }

        /// <summary>
        /// Create a new Playlist.
        /// </summary>
        /// <param name="dto">A Playlist DTO.</param>
        [HttpPost]
        public async Task<ActionResult<PlaylistDto>> CreatePlaylist([FromBody] PlaylistDto dto)
        {
            PlaylistDto createdDto = await Mediator.Send(new CreatePlaylistCommand() { Playlist = dto });
            return Ok(createdDto);
        }

        /// <summary>
        /// Update an existing Playlist.
        /// </summary>
        /// <param name="dto">Updated Playlist DTO.</param>
        [HttpPut]
        public async Task<ActionResult<PlaylistDto>> UpdatePlaylist([FromBody] PlaylistDto dto)
        {
            return Ok(await Mediator.Send(new UpdatePlaylistCommand() { Playlist = dto }));
        }

        /// <summary>
        /// Delete an existing Playlist.
        /// </summary>
        /// <param name="id">Id of the Playlist to be deleted.</param>
        [HttpDelete("{id}")]
        public async Task<ActionResult<PlaylistDto>> DeletePlaylist([FromRoute] int id)
        {
            return Ok(await Mediator.Send(new DeletePlaylistCommand() { PlaylistId = id }));
        }
    }
}
