using FluentAssertions;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Microsoft.DSX.ProjectTemplate.Test.Tests.Unit.DtoValidation
{
    [TestClass]
    [TestCategory("UserSet")]
    public class UserSetDtoTests : BaseDtoTest
    {
        [DataTestMethod]
        [DataRow("oplaner4@gmail.com", "sup3r str0ng_password", null)]
        [DataRow("john.smith@email.net", "@aa__2x?ssssssffdfesaaa", "")]
        [DataRow("email@subdomain.example.com", "hello_4_world", null)]
        [DataRow("email@123.123.123.123", "hello_4_world", null)]
        [DataRow("email@[123.123.123.123]", "hello_4_world", null)]
        [DataRow("\"email\"@example.com", "hello_4_world", "hello_5_world")]
        [DataRow("1234567890@example.com", "hello_4_world", null)]
        [DataRow("email@example-one.com", "hello_4_world", null)]
        [DataRow("_______@example.com", "hello_4_world", null)]
        [DataRow("email@example.name", "hello_4_world", null)]
        [DataRow("email@example.museum", "hello_4_world", null)]
        [DataRow("email@example.co.jp", "hello_4_world", null)]
        [DataRow("Abc..123@example.com", "hello_4_world", null)]
        [DataRow("email.@example.com", "hello_4_world", null)]
        [DataRow("email@example.web", "hello_4_world", null)]
        [DataRow("email@-example.com", "hello_4_world", null)]
        [DataRow("email@111.222.333.44444", "hello_4_world", null)]
        [DataRow("あいうえお@example.com", "hello_4_world", null)]
        public void UserSetDtoValidation_Valid_NoErrors(string email, string password, string newPassword)
        {
            var dto = new UserSetDto
            {
                Email = email,
                Password = password,
                DisplayName = RandomFactory.GetCompanyName(),
                NewPassword = newPassword,
            };

            dto.NewPasswordConfirm = dto.NewPassword;

            var validationContext = new ValidationContext(dto);

            var validationResults = dto.Validate(validationContext);

            validationResults.Should().HaveCount(0);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("    ")]
        [DataRow("#@%^%#$@#$@#.com")]
        [DataRow("plainaddress")]
        [DataRow("@example.com")]
        [DataRow("Joe Smith <email @example.com>")]
        [DataRow("email.example.com")]
        [DataRow("email@example @example.com")]
        [DataRow(".email @example.com")]
        [DataRow("email..email @example.com")]
        [DataRow("email@example.com (Joe Smith)")]
        [DataRow("email @example..com")]
        public void UserSetDtoValidation_InvalidEmail_HasValidationErrors(string email)
        {
            var dto = new UserSetDto
            {
                Email = email,
            };

            var validationContext = new ValidationContext(dto);

            var validationResults = dto.Validate(validationContext);

            validationResults.Should().HaveCountGreaterThan(0);
            FindMember(validationResults, nameof(UserSetDto.Email)).Should().NotBeNull();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("    ")]
        [DataRow("short")]
        [DataRow("short_6")]
        [DataRow("missingspecialanddigit")]
        [DataRow("missing_digit")]
        [DataRow("missing6spec1al")]
        public void UserSetDtoValidation_InvalidPassword_HasValidationErrors(string password)
        {
            var dto = new UserSetDto
            {
                NewPassword = password,
            };

            dto.NewPasswordConfirm = dto.NewPassword;

            var validationContext = new ValidationContext(dto);

            var validationResults = dto.Validate(validationContext);

            validationResults.Should().HaveCountGreaterThan(0);
            FindMember(validationResults, nameof(UserSetDto.NewPassword)).Should().NotBeNull();
        }
    }
}
