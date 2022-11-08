using FluentAssertions;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Microsoft.DSX.ProjectTemplate.Test.Tests.Unit.DtoValidation
{
    [TestClass]
    [TestCategory("UserAuthenticate")]
    public class UserAuthenticateDtoTests : BaseDtoTest
    {
        [TestMethod]
        public void UserAuthenticateDtoValidation_Valid_NoErrors()
        {
            var dto = new UserAuthenticateDto
            {
                Email = $"{RandomFactory.GetCodeName()}@email.com",
                Password = $"_@{RandomFactory.GetAlphanumericString(10)}",
            };

            var validationContext = new ValidationContext(dto);

            var validationResults = dto.Validate(validationContext);

            validationResults.Should().HaveCount(0);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("     ")]
        public void UserAuthenticateDtoValidation_MissingEmail_HasValidationErrors(string email)
        {
            var dto = new UserAuthenticateDto
            {
                Email = email,
            };

            var validationContext = new ValidationContext(dto);

            var validationResults = dto.Validate(validationContext);

            validationResults.Should().HaveCountGreaterThan(0);
            validationResults.FirstOrDefault(validationResult => validationResult.MemberNames.Any(memberName => memberName.Equals(nameof(UserAuthenticateDto.Email)))).Should().NotBeNull();
        }


        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("     ")]
        public void UserAuthenticateDtoValidation_MissingPassword_HasValidationErrors(string password)
        {
            var dto = new UserAuthenticateDto
            {
                Password = password,
            };

            var validationContext = new ValidationContext(dto);

            var validationResults = dto.Validate(validationContext);

            validationResults.Should().HaveCountGreaterThan(0);
            validationResults.FirstOrDefault(validationResult => validationResult.MemberNames.Any(memberName => memberName.Equals(nameof(UserAuthenticateDto.Password)))).Should().NotBeNull();
        }
    }
}
