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

        public HashSet<PlaylistSourceStationDto> SourceStations { get; set; }

        public int OwnerId { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            DateTime now = DateTime.Now;

            if (To > now || From > now)
            {
                yield return new ValidationResult("Invalid date range. Bounds must be somewhere in the past.",
                    new string[] { nameof(From), nameof(To) });
            }

            if (To < From)
            {
                yield return new ValidationResult("Invalid date range. Beginning must precede the end.",
                    new string[] { nameof(From), nameof(To) });
            }

            if (To - From > TimeSpan.FromDays(Constants.MaximumLengths.MaxDaysPlaylistRange))
            {
                yield return new ValidationResult($"Invalid date range. It must be maximum {Constants.MaximumLengths.MaxDaysPlaylistRange} days.",
                    new string[] { nameof(From), nameof(To) });
            }

            foreach (var result in new ValidationHelper<PlaylistDto>(this).CheckStringsNotEmptyAndCorrectLength(nameof(Name)))
            {
                yield return result;
            }
        }
    }
}
