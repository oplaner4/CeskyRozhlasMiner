using CeskyRozhlasMiner.Lib.Playlist.DataProcessing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Microsoft.DSX.ProjectTemplate.Data.DTOs
{
    public class GetDataForPlaylistDto
    {
        public GetDataForPlaylistDto(List<SongDto> relevantSongs, int maxLimit)
        {
            MaxLimit = maxLimit;
            MaxLimitExceeded = relevantSongs.Count > maxLimit;
            Songs = relevantSongs.Take(MaxLimit);
        }

        [Required]
        public IEnumerable<SongDto> Songs { get; private set; }

        public int MaxLimit { get; private set; }

        public bool MaxLimitExceeded { get; private set; }
    }
}
