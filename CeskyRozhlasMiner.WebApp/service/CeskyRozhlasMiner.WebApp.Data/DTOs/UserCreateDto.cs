﻿using CeskyRozhlasMiner.WebApp.Data.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Microsoft.DSX.ProjectTemplate.Data.DTOs
{
    public class UserCreateDto : UserDto
    {
        public string PasswordConfirm { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            ValidationHelper<UserCreateDto> helper = new ValidationHelper<UserCreateDto>(this);

            foreach (var result in helper.CheckStringsNotEmptyAndCorrectLength(nameof(DisplayName), nameof(Password), nameof(PasswordConfirm))
                .Concat(helper.CheckStringValidEmailAdress(nameof(Email))))
            {
                yield return result;
            }

            if (!Password.Any(char.IsDigit) || Password.All(char.IsLetterOrDigit))
            {
                yield return new ValidationResult($"must be minimum {Constants.MimimumLengths.Password} characters, contain at least one digit and one special character", new[] { nameof(Password) });
            }

            if (Password != PasswordConfirm)
            {
                yield return new ValidationResult($"are not equal", new[] { nameof(Password), nameof(PasswordConfirm) });
            }
        }
    }
}
