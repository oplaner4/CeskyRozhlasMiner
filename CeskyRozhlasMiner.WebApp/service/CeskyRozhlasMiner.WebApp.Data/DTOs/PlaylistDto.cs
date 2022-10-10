using CeskyRozhlasMiner.WebApp.Data.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.DSX.ProjectTemplate.Data.DTOs
{
    public class PlaylistDto : AuditDto<int>
    {
        public string Name { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        [Required]
        public HashSet<PlaylistSourceStationDto> SourceStations { get; set; }

        public int OwnerId { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in new ValidationHelper<PlaylistDto>(this).CheckStringsNotEmptyAndCorrectLength(nameof(Name)))
            {
                yield return result;
            }

            DateTime maxBound = DateTime.UtcNow.Date.AddDays(1);

            if (From > maxBound)
            {
                yield return new ValidationResult("'From' must be somewhere in the past.",
                    new string[] { nameof(From) });
            }

            if (To > maxBound)
            {
                yield return new ValidationResult("'To' must be somewhere in the past.",
                    new string[] { nameof(To) });
            }

            if (To < From)
            {
                yield return new ValidationResult("Invalid date range. 'From' must precede the 'To'.",
                    new string[] { nameof(From) });
            }

            if (To - From > TimeSpan.FromDays(Constants.MaximumLengths.MaxDaysPlaylistRange))
            {
                yield return new ValidationResult($"Date range must be maximum {Constants.MaximumLengths.MaxDaysPlaylistRange} days.",
                    new string[] { nameof(From) });
            }

            if (SourceStations.Count == 0)
            {
                yield return new ValidationResult($"At least one source station must be added.",
                    new string[] { nameof(SourceStations) });
            }
        }
    }
}
