namespace CeskyRozhlasMiner.Lib.Playlist.Json.Now.Data
{
    internal enum PlaylistNowDataStatus
    {
        /// <summary>
        /// Specific station is not currently
        /// playing any song.
        /// </summary>
        Quiet,

        /// <summary>
        /// Specific station is currently playing a song.
        /// </summary>
        OnAir,

        /// <summary>
        /// Unable to find out status of the station.
        /// </summary>
        Unknown,
    }
}
