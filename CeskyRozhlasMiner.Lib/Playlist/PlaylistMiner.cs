using RadiozurnalMiner.Lib.Common;
using RadiozurnalMiner.Lib.Diagnostics;
using RadiozurnalMiner.Lib.Playlist.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

namespace RadiozurnalMiner.Lib.Playlist
{
    public class PlaylistMiner
    {
        private static DateOnly _today = DateOnly.FromDateTime(DateTime.Now);
        private DateOnly _from = _today;
        private DateOnly _to = _today;

        private readonly HttpClient _client = new();

        /// <summary>
        /// Stations to mine from.
        /// </summary>
        public HashSet<RozhlasStation> SourceStations { get; private set; }

        /// <summary>
        /// Data are mined from this date.
        /// </summary>
        public DateOnly From
        {
            private set
            {
                if (value <= _to || value > _today)
                {
                    _from = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(From));
                }
            }
            get => _from;
        }

        /// <summary>
        /// Data are mined until this date.
        /// </summary>
        public DateOnly To
        {
            private set
            {
                if (value >= _from || value > _today)
                {
                    _to = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(To));
                }
            }
            get => _to;
        }

        /// <summary>
        /// Initilizes miner only in the current day.
        /// </summary>
        public PlaylistMiner()
        {
            SetSourceAllStations();
        }

        /// <summary>
        /// Initializes miner from any date in the past until now.
        /// </summary>
        /// <param name="from">Mine from the specific date</param>
        public PlaylistMiner(DateOnly from)
        {
            From = from;
            SetSourceAllStations();
        }

        /// <summary>
        /// Initializes miner within the specific range.
        /// </summary>
        /// <param name="from">Mine from the specific date</param>
        /// <param name="to">Mine to the specific date</param>
        public PlaylistMiner(DateOnly from, DateOnly to)
        {
            From = from;
            To = to;
            SetSourceAllStations();
        }


        /// <summary>
        /// Sets Rozhlas stations to mine from.
        /// </summary>
        /// <param name="stations">Enumerable stations to be set. If null
        /// then all stations are considered.</param>
        /// <returns>Current instance to allow chaining</returns>
        public PlaylistMiner SetSourceStations(IEnumerable<RozhlasStation> stations)
        {
            if (stations == null)
            {
                SetSourceAllStations();
            }
            else
            {
                SourceStations = stations.ToHashSet();
            }

            return this;
        }

        public async IAsyncEnumerable<PlaylistSong> GetSongs()
        {
            DateOnly inspected = From;
            while (inspected <= To)
            {
                await foreach (PlaylistSong song in GetSongsInDay(inspected))
                {
                    yield return song;
                }

                inspected = inspected.AddDays(1);
            }
        }

        private async IAsyncEnumerable<PlaylistSong> GetSongsInDay(DateOnly date)
        {
            foreach (RozhlasStation station in SourceStations)
            {
                await foreach (PlaylistSong song in GetSongsInDayForStation(date, station))
                {
                    yield return song;
                }
            }
        }

        private async IAsyncEnumerable<PlaylistSong> GetSongsInDayForStation(
            DateOnly date, RozhlasStation station)
        {
            HttpResponseMessage response = null;

            string briefLogInfo = $"Date: {date} Station: {station}";

            try
            {
                response = await _client.GetAsync(GetPlaylistUri(date, station).ToString());

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Logging.SaveRecord(Logging.Severity.Error, nameof(GetSongsInDayForStation),
                        $"{briefLogInfo}. Request to access Json file failed with status {response.StatusCode}.");
                    yield break;
                }
            }
            catch (HttpRequestException ex)
            {
                Logging.SaveRecord(Logging.Severity.Error, nameof(GetSongsInDayForStation),
                    $"{briefLogInfo}. Unable to make http request. {ex.Message}");
                yield break;
            }

            using (Stream jsonStream = await response.Content.ReadAsStreamAsync())
            {
                PlaylistDay day = null;

                try
                {
                    day = await JsonSerializer.DeserializeAsync<PlaylistDay>(
                    jsonStream, Settings.SerializeSettings);

                }
                catch (JsonException ex)
                {
                    Logging.SaveRecord(Logging.Severity.Error, nameof(GetSongsInDayForStation),
                        $"{briefLogInfo}. Error while deserializing json. {ex.Message}");
                    yield break;
                }


                foreach (PlaylistSong song in day.Data.Select(d => new PlaylistSong(d, station)))
                {
                    yield return song;
                }
            }
        }

        public static string GetPlaylistUri(DateOnly date, RozhlasStation station)
        {
            return new UriBuilder(Settings.RozhlasApi)
            {
                Path = Path.Combine(
                Settings.RozhlasApiPlaylistDayPath,
                    date.Year.ToString(),
                    date.Month.ToString("D2"),
                date.Day.ToString("D2"),
                    Settings.RozhlasApiStation[station] +
                        Settings.RozhlasApiPlaylistJsonExtension
                )
            }.ToString();
        }

        private void SetSourceAllStations()
        {
            SourceStations = new();

            foreach (RozhlasStation station in Enum.GetValues(typeof(RozhlasStation)))
            {
                SourceStations.Add(station);
            }
        }
    }
}
