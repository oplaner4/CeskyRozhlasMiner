using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.Exceptions;
using Microsoft.DSX.ProjectTemplate.Data.State;

namespace Microsoft.DSX.ProjectTemplate.Command
{
    /// <summary>
    /// Base class of all handlers.
    /// </summary>
    public abstract class HandlerBase
    {
        protected IMediator Mediator { get; }

        protected ProjectTemplateDbContext Database { get; }

        protected IMapper Mapper { get; }

        protected IHttpContextAccessor HttpContextAccessor { get; }

        protected SessionManipulator Manipulator { get; }

        protected HandlerBase(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            Mediator = mediator;
            Database = database;
            Mapper = mapper;
            HttpContextAccessor = httpContextAccessor;
            Manipulator = new SessionManipulator(HttpContextAccessor.HttpContext.Session);
        }

        protected void EnsureSignedIn()
        {
            if (!Manipulator.IsSignedIn())
            {
                throw new UnauthorizedException("Not signed in");
            }
        }
    }
}
