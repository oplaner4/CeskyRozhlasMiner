using CeskyRozhlasMiner.Lib.Common;
using CeskyRozhlasMiner.Lib.Diagnostics;
using CeskyRozhlasMiner.Lib.Playlist.Json.Day.Data;
using System;

namespace CeskyRozhlasMiner.Lib.Playlist
{
    public class PlaylistSong : IEquatable<PlaylistSong>
    {
        public PlaylistSong(PlaylistDayDataSong song,
            RozhlasStation sourceStation)
        {
            Artist = song.Interpret;
            Title = song.Track;
            PlayedAt = song.Since;
            SourceStation = sourceStation;
        }

        public PlaylistSong(string artist, string title,
            DateTime playedAt, RozhlasStation sourceStation)
        {
            Artist = artist;
            Title = title;
            PlayedAt = playedAt;
            SourceStation = sourceStation;
        }

        public string Artist { get; set; }
        public string Title { get; set; }
        public DateTime PlayedAt { get; set; }
        public RozhlasStation SourceStation { get; set; }

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
