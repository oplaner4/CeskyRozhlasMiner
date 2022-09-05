using System;

namespace RadiozurnalMiner.Lib.Playlist.Json
{
    public class PlaylistDayDataSong
    {
        public DateTime Since { get; set; }
        public int Id { get; set; }
        public string Interpret { get; set; }
        public int Interpret_id { get; set; }
        public string Track { get; set; }
        public int Track_id { get; set; }
        public string Itemcode { get; set; }
    }
}
