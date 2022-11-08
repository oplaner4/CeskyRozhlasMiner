using System;
using System.Windows;

namespace CeskyRozhlasMiner.WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string Title = "CeskyRozhlas Miner";
        public const int LeaderBoardTakeSongs = 10;

        public static readonly string AcceptedFileDialogFilter =
            string.Format("*.{0}|*.{0}|All files (*.*)|*.*", "crm");

        public static readonly TimeSpan AutoFetchInterval = TimeSpan.FromMinutes(1);
        public static readonly TimeSpan ChangeSongPlayingNowInterval = TimeSpan.FromSeconds(5);

        public const string SourceRepository = "https://github.com/oplaner4/CeskyRozhlasMiner";

        public const string CsvSongsEntryName = "songs.csv";
        public const string JsonSettingsEntryName = "settings.json";
    }
}
