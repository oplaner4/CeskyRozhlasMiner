using CeskyRozhlasMiner.WebApp.Data.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Microsoft.DSX.ProjectTemplate.Data.DTOs
{
    public class UserAuthenticateDto : AuditDto<int>
    {
        public string Email { get; set; }

        /// <summary>
        /// This field is used only in the request, not in response.
        /// </summary>
        public string Password { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new ValidationHelper<UserAuthenticateDto>(this).CheckStringsNotEmptyAndCorrectLength(nameof(Email), nameof(Password)).ToList();
        }
    }
}
