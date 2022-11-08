using FluentAssertions;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Microsoft.DSX.ProjectTemplate.Test.Tests.Unit.DtoValidation
{
    [TestClass]
    [TestCategory("Playlist")]
    public class PlaylistDtoTests : BaseDtoTest
    {
        [TestMethod]
        public void PlaylistDtoValidation_Valid_NoErrors()
        {
            var dto = new PlaylistDto
            {
                Name = RandomFactory.GetCompanyName(),
                From = DateTime.UtcNow.AddDays(-3).Date,
                To = DateTime.UtcNow.AddDays(-1).Date.AddMilliseconds(-1),
                SourceStations = new() {
                    new() {
                         Description = "My favourite station",
                         Station = CeskyRozhlasMiner.Lib.Common.RozhlasStation.Zurnal,
                    },
                    new() {
                         Description = "News every day",
                         Station = CeskyRozhlasMiner.Lib.Common.RozhlasStation.Plus,
                    }
                }
            };

            var validationContext = new ValidationContext(dto);

            var validationResults = dto.Validate(validationContext);

            validationResults.Should().HaveCount(0);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("     ")]
        public void PlaylistDtoValidation_MissingName_HasValidationErrors(string name)
        {
            var dto = new PlaylistDto
            {
                Name = name,
                SourceStations = new(),
            };

            var validationContext = new ValidationContext(dto);

            var validationResults = dto.Validate(validationContext);

            validationResults.Should().HaveCountGreaterThan(0);
            validationResults.FirstOrDefault(validationResult => validationResult.MemberNames.Any(memberName => memberName.Equals(nameof(PlaylistDto.Name)))).Should().NotBeNull();
        }

        [TestMethod]
        public void PlaylistDtoValidation_NameTooLong_HasValidationErrors()
        {
            var dto = new PlaylistDto
            {
                Name = RandomFactory.GetAlphanumericString(Constants.MaximumLengths.StringColumn + 3),
                SourceStations = new(),
            };

            var validationContext = new ValidationContext(dto);

            var validationResults = dto.Validate(validationContext);

            validationResults.Should().HaveCountGreaterThan(0);
            validationResults.FirstOrDefault(validationResult => validationResult.MemberNames.Any(memberName => memberName.Equals(nameof(PlaylistDto.Name)))).Should().NotBeNull();
        }

        [TestMethod]
        public void PlaylistDtoValidation_NoStation_HasValidationErrors()
        {
            var dto = new PlaylistDto
            {
                Name = RandomFactory.GetCompanyName(),
                SourceStations = new(),
                From = DateTime.UtcNow.AddDays(-5),
                To = DateTime.UtcNow.AddDays(-3),
            };

            var validationContext = new ValidationContext(dto);

            var validationResults = dto.Validate(validationContext);

            validationResults.Should().HaveCountGreaterThan(0);

            validationResults.FirstOrDefault(validationResult => validationResult.MemberNames.Any(memberName => memberName.Equals(nameof(PlaylistDto.SourceStations)))).Should().NotBeNull();
        }


        [TestMethod]
        public void PlaylistDtoValidation_InvalidDateRange_HasValidationErrors()
        {
            var dto = new PlaylistDto
            {
                Name = RandomFactory.GetCompanyName(),
                SourceStations = new()
                    {
                        new() {
                         Description = "Wanna sport?",
                         Station = CeskyRozhlasMiner.Lib.Common.RozhlasStation.ZurnalSport,
                        }
                    }
            };

            foreach (var range in new (DateTime, DateTime)[] {
                (DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(-2)),
                (DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(3)),
                (DateTime.UtcNow.Date.AddDays(1).AddMilliseconds(1), DateTime.UtcNow.AddDays(4)),
                (DateTime.UtcNow.AddDays(-1 * Constants.MaximumLengths.MaxDaysPlaylistRange - 2), DateTime.UtcNow.AddDays(-1)),
            })
            {
                (dto.From, dto.To) = range;

                var validationContext = new ValidationContext(dto);

                var validationResults = dto.Validate(validationContext);

                validationResults.Should().HaveCountGreaterThan(0);
                validationResults.FirstOrDefault(validationResult => validationResult.MemberNames.Any(memberName => memberName.Equals(nameof(PlaylistDto.From)) ||
                     memberName.Equals(nameof(PlaylistDto.To)))).Should().NotBeNull();
            }
        }
    }
}
