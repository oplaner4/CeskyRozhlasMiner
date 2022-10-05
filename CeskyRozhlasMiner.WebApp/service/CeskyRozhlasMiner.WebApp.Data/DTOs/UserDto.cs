using CeskyRozhlasMiner.WebApp.Data.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Microsoft.DSX.ProjectTemplate.Data.DTOs
{
    public class UserDto : UserAuthenticateDto
    {
        public string DisplayName { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            ValidationHelper<UserDto> helper = new ValidationHelper<UserDto>(this);
            return helper.CheckStringNotEmptyAndCorrectLength(nameof(DisplayName), "Name").ToList();
        }
    }
}
