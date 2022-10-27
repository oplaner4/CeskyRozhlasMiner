using CeskyRozhlasMiner.Lib.Common;
using CeskyRozhlasMiner.Lib.Diagnostics;
using CeskyRozhlasMiner.Lib.Playlist.Json.Day.Data;
using System;

namespace CeskyRozhlasMiner.Lib.Playlist
{
    public class PlaylistSong : IEquatable<PlaylistSong>
    {
        internal PlaylistSong(PlaylistDayDataSong song,
            RozhlasStation sourceStation)
        {
            Artist = song.Interpret;
            Title = song.Track;
            PlayedAt = TimeZoneInfo.ConvertTimeToUtc(song.Since, Settings.RozhlasTimeZoneInfo);
            SourceStation = sourceStation;
        }

        /// <summary>
        /// Initializes class.
        /// </summary>
        /// <param name="artist">Artist of the song.</param>
        /// <param name="title">Title of the song.</param>
        /// <param name="playedAt">Date and time the song was played at in the time zone specified
        /// in the Settings (the 'Kind' property must be set to 'Unspecified' or 'Utc').</param>
        /// <param name="sourceStation">Source station</param>
        public PlaylistSong(string artist, string title,
            DateTime playedAt, RozhlasStation sourceStation)
        {
            Artist = artist;
            Title = title;

            if (playedAt.Kind == DateTimeKind.Utc)
            {
                PlayedAt = playedAt;
            }
            else
            {
                PlayedAt = TimeZoneInfo.ConvertTimeToUtc(playedAt, Settings.RozhlasTimeZoneInfo);
            }
            
            SourceStation = sourceStation;
        }

        public string Artist { get; private set; }
        public string Title { get; private set; }

        /// <summary>
        /// UTC date and time the song was played at.
        /// </summary>
        public DateTime PlayedAt { get; private set; }
        public RozhlasStation SourceStation { get; private set; }

        public override bool Equals(object other)
        {
            if (other is PlaylistSong song)
            {
                return Equals(song);
            }

            return false;
        }

        public bool Equals(PlaylistSong other)
        {
            if (other == null)
            {
                return false;
            }

            return Artist == other.Artist &&
                Title == other.Title;
        }

        public override string ToString()
        {
            return $"{Artist} - {Title}";
        }

        public string ToCsvRow(char separator)
        {
            return $"{Artist}{separator}{Title}{separator}" +
                $"{PlayedAt}{separator}{SourceStation}";
        }

        public static PlaylistSong FromCsvRow(string row, char separator)
        {
            string[] parts = row.Split(separator);

            if (parts.Length >= 4
                && DateTime.TryParse(parts[2], out DateTime playedAt)
                && Enum.TryParse(typeof(RozhlasStation), parts[3],
                    out object station)
            )
            {
                return new(parts[0], parts[1], playedAt, (RozhlasStation)station);
            }

            string message = $"Unable to parse csv row: {row}";
            Logging.SaveRecord(Logging.Severity.Error, nameof(FromCsvRow),
                message);
            throw new FormatException(message);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
