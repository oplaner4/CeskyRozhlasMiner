using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.DSX.ProjectTemplate.Data;

namespace Microsoft.DSX.ProjectTemplate.Command
{
    /// <summary>
	/// Base class of all query handlers.
	/// </summary>
    public abstract class QueryHandlerBase : HandlerBase
    {
        protected QueryHandlerBase(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(mediator, database, mapper, httpContextAccessor)
        {
            // queries do not make changes to the database so we do not need ChangeTracker
            database.ChangeTracker.QueryTrackingBehavior = EntityFrameworkCore.QueryTrackingBehavior.NoTracking;
        }
    }
}
