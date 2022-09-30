using MediatR;
using Microsoft.DSX.ProjectTemplate.Data.Models;

namespace Microsoft.DSX.ProjectTemplate.Data.Events
{
    public class PlaylistCreatedDomainEvent : INotification
    {
        public Playlist Playlist { get; }

        public PlaylistCreatedDomainEvent(Playlist playlist)
        {
            Playlist = playlist;
        }
    }
}
