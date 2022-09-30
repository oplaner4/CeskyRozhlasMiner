using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.Abstractions;
using Microsoft.DSX.ProjectTemplate.Data.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Command.Playlist
{
    public class PlaylistCreatedEventHandler : HandlerBase, INotificationHandler<PlaylistCreatedDomainEvent>
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<PlaylistCreatedEventHandler> _logger;

        public PlaylistCreatedEventHandler(
            IMediator mediator,
            ProjectTemplateDbContext database,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IEmailService emailService,
            ILogger<PlaylistCreatedEventHandler> logger)
            : base(mediator, database, mapper, httpContextAccessor)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task Handle(PlaylistCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            // As a subscriber, we run on the thread pool so we need to handle our own failures appropriately.
            try
            {
                await _emailService.SendEmailAsync("a@microsoft.com", "b@microsoft.com", $"New Playlist '{notification.Playlist.Name}' was created.", "lorem ipsum");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email.");
            }
        }
    }
}
