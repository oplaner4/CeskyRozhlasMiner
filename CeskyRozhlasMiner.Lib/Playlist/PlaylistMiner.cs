﻿using CeskyRozhlasMiner.Lib.Common;
using CeskyRozhlasMiner.Lib.Playlist.Json.Day;
using CeskyRozhlasMiner.Lib.Playlist.Json.Now;
using CeskyRozhlasMiner.Lib.Playlist.Json.Now.Data;
using CeskyRozhlasMiner.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CeskyRozhlasMiner.Lib.Playlist
{
    /// <summary>
    /// Fetches songs from Rozhlas api.
    /// </summary>
    public class PlaylistMiner
    {
        private ITimeProvider _timeProvider;
        private DateOnly _today;
        private DateOnly _from;
        private DateOnly _to;

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
        /// <param name="stations">Enumerable stations to be set. If null 
        /// then all stations are considered.</param>
        public PlaylistMiner(ITimeProvider time, IEnumerable<RozhlasStation> stations = null)
        {
            SetSourceStations(stations);
            InitTime(time);
        }

        /// <summary>
        /// Initializes miner within the specific range.
        /// </summary>
        /// <param name="from">Mine from the specific date</param>
        /// <param name="to">Mine to the specific date</param>
        /// <param name="stations">Enumerable stations to be set. If null 
        /// then all stations are considered.</param>
        /// <param name="timeProvider">Injected time provider.</param>
        public PlaylistMiner(ITimeProvider timeProvider, DateOnly from, DateOnly to, IEnumerable<RozhlasStation> stations = null)
        {
            From = from;
            To = to;
            SetSourceStations(stations);
            InitTime(timeProvider);
        }


        /// <summary>
        /// Initializes miner within the specific range by deriving dates from UTC datetimes.
        /// </summary>
        /// <param name="fromUtc">Mine from the specific date which is derived by ignoring time 
        /// part ('Kind' property must be set to 'Utc').</param>
        /// <param name="toUtc">Mine to the specific date which is derived by ignoring time part. 
        ///  ('Kind' property must be set to 'Utc').</param>
        /// <param name="stations">Enumerable stations to be set. If null 
        /// then all stations are considered.</param>
        /// <param name="timeProvider">Injected time provider.</param>
        public PlaylistMiner(ITimeProvider timeProvider, DateTime fromUtc, DateTime toUtc, IEnumerable<RozhlasStation> stations = null)
        {
            From = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(fromUtc, Settings.RozhlasTimeZoneInfo));
            To = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(toUtc, Settings.RozhlasTimeZoneInfo));
            SetSourceStations(stations);
            InitTime(timeProvider);
        }

        /// <summary>
        /// Fetches songs using given settings with ability to monitor progress.
        /// </summary>
        /// <param name="onProgress">Action to handle progress. Its first argument 
        /// <typeparam name="int"/> means count of percent done. No action set by default.</param>
        /// <returns>Asynchronous enumerable collection of songs</returns>
        public async IAsyncEnumerable<PlaylistSong> GetSongs(Action<int> onProgress = null)
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

                    onProgress?.Invoke(CountPercentDone(inspected, stationOrder));

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

        private async IAsyncEnumerable<PlaylistSong> GetSongsInDayForStation(DateOnly date, RozhlasStation station)
        {
            string uri = new PlaylistUriConstructor(station).Day(date);
            string briefLogInfo = $"{nameof(GetSongsInDayForStation)} Date: {date} Station: {station}";

            JsonMiner<PlaylistDay> miner = new(_timeProvider, uri, briefLogInfo);
            (bool success, PlaylistDay day) = await miner.Fetch();

            if (success)
            {
                foreach (PlaylistSong song in day.Data.Select(d => new PlaylistSong(d, station)))
                {
                    yield return song;
                }
            }
        }

        private async Task<PlaylistSong> GetSongNowForStation(RozhlasStation station)
        {
            string briefLogInfo = $"{nameof(GetSongsInDayForStation)} Station: {station}";
            string uri = new PlaylistUriConstructor(station).Now();

            JsonMiner<PlaylistNow> miner = new(_timeProvider, uri, briefLogInfo);
            (bool success, PlaylistNow now) = await miner.Fetch();

            if (success && now.Data.InterpretStatus() == PlaylistNowDataStatus.OnAir)
            {
                return new PlaylistSong(now.Data, station);
            }

            return null;
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

        private void SetSourceStations(IEnumerable<RozhlasStation> stations)
        {
            if (stations == null)
            {
                SetSourceAllStations();
            }
            else
            {
                SourceStations = stations.ToHashSet();
            }
        }

        private void InitTime(ITimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
            _today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(_timeProvider.UtcNow, Settings.RozhlasTimeZoneInfo));
            _from = _today;
            _to = _today;
        }
    }
}
