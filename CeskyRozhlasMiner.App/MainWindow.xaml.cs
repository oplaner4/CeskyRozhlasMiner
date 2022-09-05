using Microsoft.Win32;
using RadiozurnalMiner.Dialogs;
using RadiozurnalMiner.Lib.Common;
using RadiozurnalMiner.Lib.Playlist;
using RadiozurnalMiner.Lib.Playlist.DataProcessing;
using RadiozurnalMiner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace RadiozurnalMiner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public PlaylistMiner Miner { get; private set; }
        public ObservableCollection<PlaylistSong> Songs { get; private set; }

        public SettingsDialogModel SettingsModel { get; private set; }

        public bool ShouldRefetch { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Title = App.Title;
            Miner = new();
            SettingsModel = new(Miner);
            Songs = new();

            DispatcherTimer timer = new()
            {
                Interval = App.AutoFetchInterval,
            };

            timer.Tick += Timer_Tick;
            timer.Start();

            Fetch();
        }

        private void UpdateUi()
        {
            FetchBtn.Foreground = ShouldRefetch ? Brushes.DarkGreen : Brushes.Black;

            var filteredSongs = Songs.Where(song =>
                SettingsModel.MatchesCriteria(song)).OrderByDescending(
                    song => song.PlayedAt);

            DataList.ItemsSource = filteredSongs;

            SongsFetched.Text = filteredSongs.Count().ToString();
            FetchDateRange.Text = SettingsModel.ToString();
            MostPlayed.Text = "-";
            MostFrequentArtist.Text = "-";

            if (!filteredSongs.Any())
            {
                return;
            }

            PlaylistStatistics statistics = new(filteredSongs);
            KeyValuePair<string, int> mostPlayed = statistics.GetMostPlayedSong();
            MostPlayed.Text = $"{mostPlayed.Key} ({mostPlayed.Value}x)";

            KeyValuePair<string, int> artist = statistics.GetMostFrequentArtist();
            MostFrequentArtist.Text = $"{artist.Key} ({artist.Value}x)";
        }

        private void LoadData(string fileName)
        {
            Songs.Clear();

            using (StreamReader reader = new(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Songs.Add(PlaylistSong.FromCsvRow(line, Settings.CsvSeparator));
                }
            }

            UpdateUi();
        }

        private void SaveData(string fileName)
        {
            using (StreamWriter writer = new(fileName))
            {
                foreach (PlaylistSong song in Songs)
                {
                    writer.WriteLine(song.ToCsvRow(Settings.CsvSeparator));
                }
            }
        }

        private void Fetch_Click(object sender, RoutedEventArgs e)
        {
            Fetch();
        }

        private async void Fetch()
        {
            FetchBtn.IsEnabled = false;
            ShouldRefetch = false;
            FetchSpinner.Visibility = Visibility.Visible;

            Songs.Clear();

            await foreach (PlaylistSong song in Miner.GetSongs())
            {
                Songs.Add(song);
            }

            LastFetched.Text = DateTime.Now.ToString();
            UpdateUi();

            FetchBtn.IsEnabled = true;
            FetchSpinner.Visibility = Visibility.Hidden;
        }

        private void EditSettings_Click(object sender, RoutedEventArgs e)
        {
            EditSettingsDialog();
        }

        private void EditSettingsDialog()
        {
            SettingsDialog dialog = new(SettingsModel)
            {
                Owner = this,
            };

            if (dialog.ShowDialog() == true)
            {
                SettingsModel = dialog.Model;
                UpdateShouldRefetch();
                Miner = new PlaylistMiner(SettingsModel.From, SettingsModel.To)
                    .SetSourceStations(SettingsModel.SourceStations);
                UpdateUi();
            }
        }

        private void UpdateShouldRefetch()
        {
            ShouldRefetch = SettingsModel.From < Miner.From
                || SettingsModel.To > Miner.To
                || SettingsModel.SourceStations.Any(st =>
                    !Miner.SourceStations.Contains(st));
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = App.AcceptedFileDialogFilter
            };

            if (dialog.ShowDialog() == true)
            {
                LoadData(dialog.FileName);
            }
        }

        private void FileSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new()
            {
                Filter = App.AcceptedFileDialogFilter
            };

            if (dialog.ShowDialog() == true)
            {
                SaveData(dialog.FileName);
            }
        }

        private void LeaderBoard_Click(object sender, RoutedEventArgs e)
        {
            LeaderBoardDialog dialog = new(Songs.Where(song =>
                SettingsModel.MatchesCriteria(song)))
            {
                Owner = this,
            };

            dialog.ShowDialog();
        }

        private void AboutWebsite_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("");
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            Fetch();
        }

        private void DataListItem_Click(
            object sender, System.Windows.Controls.DataGridBeginningEditEventArgs e)
        {
            PlaylistSong song = (PlaylistSong)e.Row.DataContext;

            SettingsModel.Tracks = new() { song.Title };
            SettingsModel.Artists = new() { song.Artist };

            e.Cancel = true;
            EditSettingsDialog();
        }
    }
}
