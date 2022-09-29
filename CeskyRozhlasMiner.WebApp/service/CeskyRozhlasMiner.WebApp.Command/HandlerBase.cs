using AutoMapper;
using CeskyRozhlasMiner.WebApp.Command.State;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.DSX.ProjectTemplate.Data;

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
    }
}
