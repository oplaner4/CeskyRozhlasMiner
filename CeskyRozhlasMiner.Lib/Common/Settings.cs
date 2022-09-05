using System.Collections.Generic;
using System.Text.Json;

namespace RadiozurnalMiner.Lib.Common
{
    public class Settings
    {
        public const string RozhlasApi = "https://api.rozhlas.cz";
        public const string RozhlasApiPlaylistDayPath = "data/v2/playlist/day";

        public static readonly Dictionary<RozhlasStation, string>
            RozhlasApiStation = new()
        {
            { RozhlasStation.Zurnal, "radiozurnal" },
            { RozhlasStation.Dvojka, "dvojka" },
            { RozhlasStation.Vltava, "vltava" },
            { RozhlasStation.ZurnalSport, "radiozurnal-sport" },
            { RozhlasStation.Wave, "radiowave" },
            { RozhlasStation.Ddur, "d-dur" },
            { RozhlasStation.Jazz, "jazz" },
            { RozhlasStation.Pohoda, "pohoda" },
        };

        public const string RozhlasApiPlaylistJsonExtension = ".json";

        public static readonly JsonSerializerOptions SerializeSettings =
            new() { PropertyNameCaseInsensitive = true };

        public const string LoggingCsvFile = "../../../Diagnostics/Log.csv";
        public const char CsvSeparator = ';';
    }
}
