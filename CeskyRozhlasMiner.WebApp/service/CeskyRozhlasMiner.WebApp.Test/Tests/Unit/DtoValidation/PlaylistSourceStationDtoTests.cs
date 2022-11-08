using CeskyRozhlasMiner.Lib.Common;
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
    [TestCategory("PlaylistSourceStation")]
    public class PlaylistSourceStationDtoTests : BaseDtoTest
    {
        [DataTestMethod]
        [DataRow("", RozhlasStation.Vltava)]
        [DataRow(null, RozhlasStation.Zurnal)]
        [DataRow("Listening this every day", RozhlasStation.Pohoda)]
        public void PlaylistSourceStationDtoValidation_Valid_NoErrors(string description, RozhlasStation station)
        {
            var dto = new PlaylistSourceStationDto
            {
                Description = description,
                Station = station
            };

            var validationContext = new ValidationContext(dto);

            var validationResults = dto.Validate(validationContext);

            validationResults.Should().HaveCount(0);
        }

        [TestMethod]
        public void PlaylistSourceStationDtoValidation_DescriptionTooLong_HasValidationErrors()
        {
            var dto = new PlaylistSourceStationDto
            {
                Description = RandomFactory.GetAlphanumericString(Constants.MaximumLengths.StringColumn + 4),
            };

            var validationContext = new ValidationContext(dto);

            var validationResults = dto.Validate(validationContext);

            validationResults.Should().HaveCountGreaterThan(0);
            validationResults.FirstOrDefault(validationResult => validationResult.MemberNames.Any(memberName => memberName.Equals(nameof(PlaylistSourceStationDto.Description)))).Should().NotBeNull();
        }
    }
}
