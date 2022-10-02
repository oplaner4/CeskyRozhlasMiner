using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.Exceptions;
using System.Linq;
using System.Security.Claims;

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

        protected int UserId { get; }

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
            UserId = -1;

            var claim = HttpContextAccessor.HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault();

            if (claim != null)
            {
                UserId = int.Parse(claim.Value);
            }
                
        }

        protected void EnsureSignedIn()
        {
            if (!HttpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                throw new UnauthorizedException("Not signed in");
            }
        }
    }
}
