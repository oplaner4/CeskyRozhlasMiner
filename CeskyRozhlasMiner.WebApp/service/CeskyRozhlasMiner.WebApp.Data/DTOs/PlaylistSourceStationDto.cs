using CeskyRozhlasMiner.Lib.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.DSX.ProjectTemplate.Data.DTOs
{
    public class PlaylistSourceStationDto : AuditDto<int>
    {
        public string Description { get; set; }

        public RozhlasStation Station { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
