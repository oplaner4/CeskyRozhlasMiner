using CeskyRozhlasMiner.Lib.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.DSX.ProjectTemplate.Data.DTOs
{
    public class SongDto : AuditDto<int>
    {
        public string Artist { get; set; }
        public string Title { get; set; }
        public DateTime PlayedAt { get; set; }
        public RozhlasStation SourceStation { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
