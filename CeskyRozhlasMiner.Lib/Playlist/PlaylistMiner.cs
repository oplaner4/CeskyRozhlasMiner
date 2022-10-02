using CeskyRozhlasMiner.Lib.Common;
using CeskyRozhlasMiner.Lib.Playlist.Json.Day;
using CeskyRozhlasMiner.Lib.Playlist.Json.Now;
using CeskyRozhlasMiner.Lib.Playlist.Json.Now.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CeskyRozhlasMiner.Lib.Playlist
{
    /// <summary>
    /// Fetches songs from Rozhlas api
    /// </summary>
    public class PlaylistMiner
    {
        private static DateOnly _today = DateOnly.FromDateTime(DateTime.Now);
        private DateOnly _from = _today;
        private DateOnly _to = _today;

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

        /// <summary>
        /// Fetches songs using given settings with ability to monitor progress.
        /// </summary>
        /// <param name="onProgress">Action to handle progress. Its first argument 
        /// <typeparam name="int"/> means count of percent done.</param>
        /// <returns>Asynchronous enumerable collection of songs</returns>
        public async IAsyncEnumerable<PlaylistSong> GetSongs(Action<int> onProgress)
        {
            int stationOrder = 1;

            foreach (RozhlasStation station in SourceStations)
            {
                DateOnly inspected = From;

                while (inspected <= To)
                {
                    await foreach (PlaylistSong song in GetSongsInDayForStation(inspected, station))
                    {
                        yield return song;
                    }

                    onProgress(CountPercentDone(inspected, stationOrder));
                    inspected = inspected.AddDays(1);
                }

                stationOrder++;
            }
        }

        /// <summary>
        /// Fetches currently played songs using given settings on the stations
        /// with status 'onair'.
        /// </summary>
        /// <returns>Asynchronous enumerable collection of songs</returns>
        public async IAsyncEnumerable<PlaylistSong> GetSongsNow()
        {
            int stationOrder = 1;

            foreach (RozhlasStation station in SourceStations)
            {
                PlaylistSong song = await GetSongNowForStation(station);

                if (song != null)
                {
                    yield return song;
                }

                stationOrder++;
            }
        }

        /// <summary>
        /// Sets all available stations as a source.
        /// </summary>
        public void SetSourceAllStations()
        {
            SourceStations = new();

            foreach (RozhlasStation station in Enum.GetValues(typeof(RozhlasStation)))
            {
                SourceStations.Add(station);
            }
        }

        private static async Task<PlaylistSong> GetSongNowForStation(RozhlasStation station)
        {
            string briefLogInfo = $"{nameof(GetSongsInDayForStation)} Station: {station}";
            string uri = new PlaylistUriConstructor(station).Now();

            JsonMiner<PlaylistNow> miner = new(uri, briefLogInfo);
            (bool success, PlaylistNow now) = await miner.Fetch();

            if (success && now.Data.InterpretStatus() == PlaylistNowDataStatus.OnAir)
            {
                return new PlaylistSong(now.Data.Interpret, now.Data.Track, now.Data.Since, station);
            }

            return null;
        }

        public static async IAsyncEnumerable<PlaylistSong> GetSongsInDayForStation(DateOnly date, RozhlasStation station)
        {
            string uri = new PlaylistUriConstructor(station).Day(date);
            string briefLogInfo = $"{nameof(GetSongsInDayForStation)} Date: {date} Station: {station}";

            JsonMiner<PlaylistDay> miner = new(uri, briefLogInfo);
            (bool success, PlaylistDay day) = await miner.Fetch();

            if (success)
            {
                foreach (PlaylistSong song in day.Data.Select(d => new PlaylistSong(d, station)))
                {
                    yield return song;
                }
            }
        }

        private int CountPercentDone(DateOnly date, int stationOrder)
        {
            int daysTotal = To.DayNumber - From.DayNumber + 1;
            int daysUntilDate = date.DayNumber + 1 - From.DayNumber;

            double daysDone = (stationOrder - 1) * daysTotal + daysUntilDate;

            double done = daysDone;
            double total = SourceStations.Count * daysTotal;
            return (int)Math.Round(100 * done / total);
        }
    }
}
