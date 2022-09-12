﻿using CeskyRozhlasMiner.Lib.Playlist.Json.Day.Data;

namespace CeskyRozhlasMiner.Lib.Playlist.Json.Now.Data
{
    public class PlaylistNowData : PlaylistDayDataSong
    {
        public string Status { get; set; }

        /// <summary>
        /// Tries to interpret string value from api
        /// as a readable enum value.
        /// </summary>
        /// <returns>Status</returns>
        public PlaylistNowDataStatus InterpretStatus()
        {
            return Status switch
            {
                "quiet" => PlaylistNowDataStatus.Quiet,
                "onair" => PlaylistNowDataStatus.OnAir,
                _ => PlaylistNowDataStatus.Unknown,
            };
        }
    }
}
