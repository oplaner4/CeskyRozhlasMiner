using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.DSX.ProjectTemplate.Data.DTOs
{
    public class PlaylistDto : AuditDto<int>
    {
        public string Name { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public HashSet<PlaylistSourceStationDto> SourceStations { get; set; }

        public int OwnerId { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
