using CeskyRozhlasMiner.Lib.Playlist.Json.Now.Data;
using RadiozurnalMiner.Lib.Playlist.Json;
using System.Collections.Generic;

namespace CeskyRozhlasMiner.Lib.Playlist.Json.Day
{
    public class PlaylistDay : PlaylistContainer
    {
        public IEnumerable<PlaylistNowData> Data { get; set; }
    }
}
