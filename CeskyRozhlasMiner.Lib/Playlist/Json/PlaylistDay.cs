using System;
using System.Collections.Generic;

namespace RadiozurnalMiner.Lib.Playlist.Json
{
    public class PlaylistDay
    {
        public DateTime Timestamp { get; set; }

        public IEnumerable<PlaylistDayDataSong> Data { get; set; }
    }
}
