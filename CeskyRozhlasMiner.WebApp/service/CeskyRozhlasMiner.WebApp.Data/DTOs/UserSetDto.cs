using CeskyRozhlasMiner.WebApp.Data.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Microsoft.DSX.ProjectTemplate.Data.DTOs
{
    public class UserSetDto : UserDto
    {
        public string NewPasswordConfirm { get; set; }

        public string NewPassword { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            DisplayName = XssPrevention.Sanitize(DisplayName);
            ValidationHelper<UserSetDto> helper = new ValidationHelper<UserSetDto>(this);

            foreach (var result in helper.CheckStringNotEmptyAndCorrectLength(nameof(DisplayName), "Name")
                .Concat(helper.CheckStringValidEmailAdress(nameof(Email)))
            )
            {
                yield return result;
            }

            bool creating = string.IsNullOrEmpty(Password);

            if (!creating && string.IsNullOrEmpty(NewPassword))
            {
                // User does not want to change password.
                yield break;
            }

            foreach (var result in helper.CheckValidPassword(nameof(NewPassword), "New password"))
            {
                yield return result;
            }

            if (NewPassword != NewPasswordConfirm)
            {
                yield return new ValidationResult($"Passwords are not equal.", new[] { nameof(NewPasswordConfirm) });
            }
        }
    }
}
