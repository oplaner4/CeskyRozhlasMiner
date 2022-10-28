using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CeskyRozhlasMiner.Lib.Common
{
    public class Settings
    {
        internal const string RozhlasApi = "https://api.rozhlas.cz";
        internal const string RozhlasApiPlaylistDayPath = "data/v2/playlist/day";
        internal const string RozhlasApiPlaylistNowPath = "data/v2/playlist/now";

        internal static readonly Dictionary<RozhlasStation, string>
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

        internal const string RozhlasApiPlaylistJsonExtension = ".json";

        internal static readonly JsonSerializerOptions DeserializeSettings = new() { PropertyNameCaseInsensitive = true, };

        internal const string LoggingCsvFile = "../../../Diagnostics/Log.csv";
        internal const char CsvSeparator = ';';

        public static readonly TimeZoneInfo RozhlasTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
    }
}
