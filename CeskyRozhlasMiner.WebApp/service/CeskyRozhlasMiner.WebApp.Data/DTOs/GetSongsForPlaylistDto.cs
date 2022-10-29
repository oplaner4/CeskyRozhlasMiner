using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Microsoft.DSX.ProjectTemplate.Data.DTOs
{
    public class GetSongsForPlaylistDto
    {
        public GetSongsForPlaylistDto(ICollection<SongDto> relevantSongs, int maxLimit)
        {
            MaxLimit = maxLimit;
            TotalCount = relevantSongs.Count;
            MaxLimitExceeded = TotalCount > maxLimit;
            Songs = relevantSongs.Take(MaxLimit);
        }

        [Required]
        public IEnumerable<SongDto> Songs { get; private set; }
        public int MaxLimit { get; private set; }
        public int TotalCount { get; private set; }
        public bool MaxLimitExceeded { get; private set; }
    }
}
