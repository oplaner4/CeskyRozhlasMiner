using CeskyRozhlasMiner.Lib.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DSX.ProjectTemplate.Command.Group;
using System;
using System.Collections.Generic;

namespace Microsoft.DSX.ProjectTemplate.API.Controllers
{
    /// <summary>
    /// Controller for Cesky rozhlas stations APIs.
    /// </summary>
    public class StationsController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StationsController"/> class.
        /// </summary>
        /// <param name="mediator">Mediator instance from dependency injection.</param>
        public StationsController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// Get Cesky rozhlas stations.
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<RozhlasStation>> GetStations()
        {
            return Ok(Enum.GetValues(typeof(RozhlasStation)));
        }

    }
}
