using System.Collections.Generic;
using System.Text.Json;

namespace CeskyRozhlasMiner.Lib.Common
{
    public class Settings
    {
        public const string RozhlasApi = "https://api.rozhlas.cz";
        public const string RozhlasApiPlaylistDayPath = "data/v2/playlist/day";
        public const string RozhlasApiPlaylistNowPath = "data/v2/playlist/now";

        public static readonly Dictionary<RozhlasStation, string>
            RozhlasApiStation = new()
        {
            { RozhlasStation.Zurnal,          "radiozurnal"       },
            { RozhlasStation.Dvojka,          "dvojka"            },
            { RozhlasStation.Vltava,          "vltava"            },
            { RozhlasStation.ZurnalSport,     "radiozurnal-sport" },
            { RozhlasStation.Wave,            "radiowave"         },
            { RozhlasStation.Ddur,            "d-dur"             },
            { RozhlasStation.Jazz,            "jazz"              },
            { RozhlasStation.Pohoda,          "pohoda"            },
            { RozhlasStation.Plus,            "plus"              },
            { RozhlasStation.Junior,          "radiojunior"       },
            { RozhlasStation.Cro7,            "cro7"              },
            { RozhlasStation.CeskeBudejovice, "cb"                },
            { RozhlasStation.HradecKralove,   "hradec"            },
            { RozhlasStation.KarlovyVary,     "kv"                },
            { RozhlasStation.Liberec,         "liberec"           },
            { RozhlasStation.Olomouc,         "olomouc"           },
            { RozhlasStation.Ostrava,         "ostrava"           },
            { RozhlasStation.Pardubice,       "pardubice"         },
            { RozhlasStation.Plzen,           "plzen"             },
            { RozhlasStation.Regina,          "regina"            },
            { RozhlasStation.StredniCechy,    "strednicechy"      },
            { RozhlasStation.Sever,           "sever"             },
            { RozhlasStation.Vysocina,        "vysocina"          },
            { RozhlasStation.Zlin,            "zlin"              },
        };

        public const string RozhlasApiPlaylistJsonExtension = ".json";

        public static readonly JsonSerializerOptions SerializeSettings =
            new() { PropertyNameCaseInsensitive = true };

        public const string LoggingCsvFile = "../../../Diagnostics/Log.csv";
        public const char CsvSeparator = ';';
    }
}
