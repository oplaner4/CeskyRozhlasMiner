using CeskyRozhlasMiner.Lib.Common;
using CeskyRozhlasMiner.WebApp.Data.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.DSX.ProjectTemplate.Data.DTOs
{
    public class PlaylistSourceStationDto : AuditDto<int>
    {
        public string Description { get; set; }

        public RozhlasStation Station { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            Description = XssPrevention.Sanitize(Description);
            return new ValidationHelper<PlaylistSourceStationDto>(this).CheckStringCorrectLength(nameof(Description)).ToList();
        }
    }
}
