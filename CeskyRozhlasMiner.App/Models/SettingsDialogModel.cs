using RadiozurnalMiner.Lib.Common;
using RadiozurnalMiner.Lib.Playlist;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RadiozurnalMiner.Models
{
    public class SettingsDialogModel
    {
        public SettingsDialogModel(PlaylistMiner miner)
        {
            DaysOfWeek = new()
            {
                DayOfWeek.Sunday,
                DayOfWeek.Monday,
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday,
                DayOfWeek.Friday,
                DayOfWeek.Saturday,
            };

            From = miner.From;
            To = miner.To;
            SourceStations = miner.SourceStations.ToHashSet();

            PlayedFrom = new TimeSpan(0, 0, 0);
            PlayedTo = new TimeSpan(23, 59, 59);
        }

        public DateOnly From { get; set; }
        public DateOnly To { get; set; }

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
            return $"{From} - {To}";
        }

        public bool MatchesCriteria(PlaylistSong song)
        {
            DateOnly playedAt = DateOnly.FromDateTime(song.PlayedAt);

            return playedAt >= From && playedAt <= To
                && song.PlayedAt.TimeOfDay >= PlayedFrom
                && song.PlayedAt.TimeOfDay <= PlayedTo
                && DaysOfWeek.Contains(song.PlayedAt.DayOfWeek)
                && (Artists == null || Artists.Contains(song.Artist))
                && (Tracks == null || Tracks.Contains(song.Title))
                && SourceStations.Contains(song.SourceStation);
        }
    }
}
