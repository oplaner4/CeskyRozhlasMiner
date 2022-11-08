using CeskyRozhlasMiner.Lib.Common;
using CeskyRozhlasMiner.Lib.Playlist;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CeskyRozhlasMiner.WpfApp.Models
{
    [Serializable]
    public class SettingsDialogModel
    {
        /// <summary>
        /// Constructor used just for deserialization
        /// </summary>
        public SettingsDialogModel()
        {}

        public SettingsDialogModel(PlaylistMiner miner)
        {
            DaysOfWeek = new();

            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                DaysOfWeek.Add(day);
            }

            From = miner.From.ToDateTime(TimeOnly.MinValue);
            To = miner.To.ToDateTime(new TimeOnly(23, 59, 59, 999));
            SourceStations = miner.SourceStations.ToHashSet();

            PlayedFrom = new TimeSpan(0, 0, 0);
            PlayedTo = new TimeSpan(23, 59, 59);
        }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public HashSet<RozhlasStation> SourceStations { get; set; }

        public HashSet<DayOfWeek> DaysOfWeek { get; set; }

        public TimeSpan PlayedFrom { get; set; }

        public TimeSpan PlayedTo { get; set; }

        /// <summary>
        /// Set of artists. If null, all are considered.
        /// </summary>
        public HashSet<string> Artists { get; set; }

        /// <summary>
        /// Set of tracks. If null, all are considered.
        /// </summary>
        public HashSet<string> Tracks { get; set; }

        public override string ToString()
        {
            return $"{From:d} - {To:d}";
        }

        public bool MatchesCriteria(PlaylistSong song)
        {
            return song.PlayedAt >= From && song.PlayedAt <= To
                && song.PlayedAt.TimeOfDay >= PlayedFrom
                && song.PlayedAt.TimeOfDay <= PlayedTo
                && DaysOfWeek.Contains(song.PlayedAt.DayOfWeek)
                && (Artists == null || Artists.Contains(song.Artist))
                && (Tracks == null || Tracks.Contains(song.Title))
                && SourceStations.Contains(song.SourceStation);
        }
    }
}
