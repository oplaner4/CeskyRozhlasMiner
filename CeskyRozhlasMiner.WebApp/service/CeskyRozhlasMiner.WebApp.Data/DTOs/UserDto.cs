using CeskyRozhlasMiner.WebApp.Data.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Microsoft.DSX.ProjectTemplate.Data.DTOs
{
    public class UserDto : AuditDto<int>
    {
        public string Email { get; set; }

        public string DisplayName { get; set; }

        /// <summary>
        /// This field is used only in the request, not in response.
        /// </summary>
        public string Password { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            ValidationHelper<UserDto> helper = new ValidationHelper<UserDto>(this);

            return helper.CheckStringNotEmptyAndCorrectLength(nameof(Password))
                .Concat(helper.CheckStringValidEmailAdress(nameof(Email)));
        }
    }
}
