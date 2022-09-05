using System;
using System.Windows;

namespace RadiozurnalMiner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string Title = "CeskyRozhlas Miner";
        public const int LeaderBoardTakeSongs = 10;

        public static readonly string AcceptedFileDialogFilter =
            string.Format("*.{0}|*.{0}|All files (*.*)|*.*", "csv");

        public static readonly TimeSpan AutoFetchInterval = TimeSpan.FromMinutes(5);
    }
}
