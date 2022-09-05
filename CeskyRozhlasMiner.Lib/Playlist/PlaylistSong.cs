using RadiozurnalMiner.Lib.Common;
using RadiozurnalMiner.Lib.Diagnostics;
using RadiozurnalMiner.Lib.Playlist.Json;
using System;

namespace RadiozurnalMiner.Lib.Playlist
{
    public class PlaylistSong : IEquatable<PlaylistSong>
    {
        public PlaylistSong()
        {
        }

        public PlaylistSong(PlaylistDayDataSong song,
            RozhlasStation sourceStation)
        {
            Artist = song.Interpret;
            Title = song.Track;
            PlayedAt = song.Since;
            SourceStation = sourceStation;
        }

        public string Artist { get; set; }
        public string Title { get; set; }
        public DateTime PlayedAt { get; set; }
        public RozhlasStation SourceStation { get; set; }

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
                return new()
                {
                    Artist = parts[0],
                    Title = parts[1],
                    PlayedAt = playedAt,
                    SourceStation = (RozhlasStation)station,
                };
            }
            else
            {
                string message = $"Unable to parse csv row: {row}";
                Logging.SaveRecord(Logging.Severity.Error, nameof(FromCsvRow),
                    message);
                throw new FormatException(message);
            }
        }
    }
}
