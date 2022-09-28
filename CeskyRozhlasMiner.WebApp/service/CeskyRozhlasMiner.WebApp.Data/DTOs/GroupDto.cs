using CeskyRozhlasMiner.WebApp.Data.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Microsoft.DSX.ProjectTemplate.Data.DTOs
{
    public class GroupDto : AuditDto<int>
    {
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new ValidationHelper<GroupDto>(this).CheckStringNotEmptyAndCorrectLength(nameof(Name)).ToList();
        }
    }
}
