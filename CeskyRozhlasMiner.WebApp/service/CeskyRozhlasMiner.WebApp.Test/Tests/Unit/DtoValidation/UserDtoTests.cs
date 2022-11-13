using FluentAssertions;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Microsoft.DSX.ProjectTemplate.Test.Tests.Unit.DtoValidation
{
    [TestClass]
    [TestCategory("User")]
    public class UserDtoTests : BaseDtoTest
    {
        [TestMethod]
        public void UserDtoValidation_Valid_NoErrors()
        {
            var dto = new UserDto
            {
                Email = $"{RandomFactory.GetCodeName()}@email.com",
                DisplayName = RandomFactory.GetCompanyName(),
                Verified = true,
            };

            var validationContext = new ValidationContext(dto);

            var validationResults = dto.Validate(validationContext);

            validationResults.Should().HaveCount(0);
        }

        [TestMethod]
        public void UserDtoValidation_DisplayNameRemoveXss_NoErrors()
        {
            var name = RandomFactory.GetCompanyName();

            var dto = new UserDto
            {
                DisplayName = $"{name}<script>console.log('xss')</script>",
            };

            var validationContext = new ValidationContext(dto);

            var validationResults = dto.Validate(validationContext);

            dto.DisplayName.Should().Be(name);

            validationResults.Should().HaveCount(0);
        }

        [TestMethod]
        public void UserDtoValidation_DisplayNameTooLong_HasValidationErrors()
        {
            var dto = new UserDto
            {
                DisplayName = RandomFactory.GetAlphanumericString(Constants.MaximumLengths.StringColumn + 6),
            };

            var validationContext = new ValidationContext(dto);

            var validationResults = dto.Validate(validationContext);

            validationResults.Should().HaveCountGreaterThan(0);
            FindMember(validationResults, nameof(UserDto.DisplayName)).Should().NotBeNull();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("    ")]
        [DataRow("    <script>console.log('xss')</script>  ")]
        public void UserDtoValidation_MissingDisplayName_HasValidationErrors(string displayName)
        {
            var dto = new UserDto
            {
                DisplayName = displayName,
            };

            var validationContext = new ValidationContext(dto);

            var validationResults = dto.Validate(validationContext);

            validationResults.Should().HaveCountGreaterThan(0);
            FindMember(validationResults, nameof(UserDto.DisplayName)).Should().NotBeNull();
        }
    }
}
