using CeskyRozhlasMiner.Lib.Common;
using System;
using System.IO;

namespace CeskyRozhlasMiner.Lib.Playlist
{
    internal class PlaylistUriConstructor
    {
        private readonly RozhlasStation _station;

        internal PlaylistUriConstructor(RozhlasStation station)
        {
            _station = station;
        }

        internal string Day(DateOnly date)
        {
            return new UriBuilder(Settings.RozhlasApi)
            {
                Path = Path.Combine(
                Settings.RozhlasApiPlaylistDayPath,
                    date.Year.ToString(),
                    date.Month.ToString("D2"),
                date.Day.ToString("D2"),
                    Settings.RozhlasApiStation[_station] +
                        Settings.RozhlasApiPlaylistJsonExtension
                )
            }.ToString();
        }

        internal string Now()
        {
            return new UriBuilder(Settings.RozhlasApi)
            {
                Path = Path.Combine(
                Settings.RozhlasApiPlaylistNowPath,
                    Settings.RozhlasApiStation[_station] +
                        Settings.RozhlasApiPlaylistJsonExtension
                )
            }.ToString();
        }
    }
}
