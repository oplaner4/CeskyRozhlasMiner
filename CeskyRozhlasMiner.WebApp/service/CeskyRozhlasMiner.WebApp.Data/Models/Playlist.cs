using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.DSX.ProjectTemplate.Data.Models
{
    public class Playlist : AuditModel<int>
    {
        [Required]
        [MaxLength(Constants.MaximumLengths.StringColumn)]
        public string Name { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public virtual User Owner { get; set; }

        public int OwnerId { get; set; }

        public virtual ICollection<PlaylistSourceStation> SourceStations { get; set; } = new HashSet<PlaylistSourceStation>();
    }
}
