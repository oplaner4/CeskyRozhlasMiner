using System;

namespace CeskyRozhlasMiner.Lib.Playlist.Json.Day.Data
{
    internal class PlaylistDayDataSong
    {
        private DateTime _since;

        /// <summary>
        /// Date and time in the time zone specified in the Settings. The 'Kind' 
        /// property is set to 'Unspecified' when setting value.
        /// </summary>
        public DateTime Since
        {
            get
            {
                return _since;
            }
            set
            {
                _since = DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
            }
        }

        public int Id { get; set; }
        public string Interpret { get; set; }
        public int Interpret_id { get; set; }
        public string Track { get; set; }
        public int Track_id { get; set; }
        public string Itemcode { get; set; }
    }
}
