using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.DSX.ProjectTemplate.Data.DTOs
{
    public class GetStatisticsForPlaylistDto
    {
        [Required]
        public bool NoSourceData { get; set; }

        [Required]
        public StatisticsEntryDto MostFrequentArtist { get; set; }

        [Required]
        public StatisticsEntryDto MostPlayedSong { get; set; }

        [Required]
        public IEnumerable<StatisticsEntryDto> LeaderBoard { get; set; }
    }
}
