using RadiozurnalMiner.Lib.Playlist;
using RadiozurnalMiner.Lib.Playlist.DataProcessing;
using System.Collections.Generic;
using System.Windows;

namespace RadiozurnalMiner.Dialogs
{
    /// <summary>
    /// Interaction logic for LeaderBoardDialog.xaml
    /// </summary>
    public partial class LeaderBoardDialog : Window
    {
        private readonly PlaylistStatistics Statistics;

        public LeaderBoardDialog(IEnumerable<PlaylistSong> songs)
        {
            InitializeComponent();
            Title = $"Leader board - {App.Title}";
            Statistics = new(songs);
            TakeSongs.Text = App.LeaderBoardTakeSongs.ToString();
            DataList.ItemsSource = Statistics.GetLeaderBoard(
                App.LeaderBoardTakeSongs);
        }

        private void TakeSongsSetBtn_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TakeSongs.Text, out int takeSongs) && takeSongs > 0)
            {
                DataList.ItemsSource = Statistics.GetLeaderBoard(takeSongs);
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DataList_BeginningEdit(object sender, System.Windows.Controls.DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
