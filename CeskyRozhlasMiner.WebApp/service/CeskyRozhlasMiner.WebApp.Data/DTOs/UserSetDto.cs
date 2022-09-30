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
            ValidationHelper<UserSetDto> helper = new ValidationHelper<UserSetDto>(this);

            foreach (var result in helper.CheckStringsNotEmptyAndCorrectLength(nameof(DisplayName), nameof(NewPassword), nameof(NewPasswordConfirm))
                .Concat(helper.CheckStringValidEmailAdress(nameof(Email)))
                .Concat(helper.CheckValidPassword(nameof(NewPassword))))
            {
                yield return result;
            }

            if (NewPassword != NewPasswordConfirm)
            {
                yield return new ValidationResult($"are not equal", new[] { nameof(NewPassword), nameof(NewPasswordConfirm) });
            }


        }
    }
}
