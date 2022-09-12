using RadiozurnalMiner.Lib.Common;
using System;
using System.IO;

namespace RadiozurnalMiner.Lib.Playlist
{
    public class PlaylistUriConstructor
    {
        private readonly RozhlasStation _station;

        public PlaylistUriConstructor(RozhlasStation station)
        {
            _station = station;
        }

        public string Day(DateOnly date)
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

        public string Now()
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
