using CeskyRozhlasMiner.WebApp.Data.Utilities;
using FluentAssertions;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.DSX.ProjectTemplate.Test.Tests.Unit.DtoValidation
{
    [TestClass]
    [TestCategory("Token")]
    public class TokenDtoTests : BaseDtoTest
    {
        [TestMethod]
        public void TokenDtoValidation_Valid_NoErrors()
        {
            var dto = new TokenDto
            {
                Value = TokenValueGenerator.GetNewValue(),
            };

            var validationContext = new ValidationContext(dto);

            var validationResults = dto.Validate(validationContext);

            validationResults.Should().HaveCount(0);
        }

        [TestMethod]
        public void TokenDtoValidation_ValueTooLong_HasValidationErrors()
        {
            var dto = new TokenDto
            {
                Value = RandomFactory.GetAlphanumericString(Constants.MaximumLengths.StringColumn + 3),
            };

            var validationContext = new ValidationContext(dto);

            var validationResults = dto.Validate(validationContext);

            validationResults.Should().HaveCountGreaterThan(0);
            FindMember(validationResults, nameof(TokenDto.Value)).Should().NotBeNull();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("     ")]
        public void TokenDtoValidation_MissingValue_HasValidationErrors(string value)
        {
            var dto = new TokenDto
            {
                Value = value,
            };

            var validationContext = new ValidationContext(dto);

            var validationResults = dto.Validate(validationContext);

            validationResults.Should().HaveCountGreaterThan(0);
            FindMember(validationResults, nameof(TokenDto.Value)).Should().NotBeNull();
        }
    }
}
