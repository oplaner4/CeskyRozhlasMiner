using Microsoft.Win32;
using CeskyRozhlasMiner.WpfApp.Dialogs;
using CeskyRozhlasMiner.Lib.Playlist;
using CeskyRozhlasMiner.Lib.Playlist.DataProcessing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using CeskyRozhlasMiner.WpfApp.Models;
using CeskyRozhlasMiner.Time;

namespace CeskyRozhlasMiner.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _autoFetchTimer;
        private DispatcherTimer _songPlayingNowTimer;
        private PlaylistMiner _miner;
        private readonly ObservableCollection<PlaylistSong> _songs;
        private readonly List<PlaylistSong> _songsPlayingNow;
        private IEnumerator<PlaylistSong> _songsPlayingNowEnumerator;
        private SettingsDialogModel _settingsModel;
        private bool _shouldRefetch;

        public MainWindow()
        {
            InitializeComponent();
            Title = App.Title;
            _miner = new(new RealTimeProvider());
            _settingsModel = new(_miner);
            _songs = new();
            _songsPlayingNow = new List<PlaylistSong>();
            _songsPlayingNowEnumerator = _songsPlayingNow.GetEnumerator();

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                LoadData(args[1]);
            }
            else
            {
                FetchSongs();
            }

            FetchSongsPlayingNow();
        }

        private void UpdateUi()
        {
            FetchBtn.Foreground = _shouldRefetch ? FetchProgress.Foreground : Brushes.Black;

            var filteredSongs = _songs.Where(song =>
                _settingsModel.MatchesCriteria(song)).OrderByDescending(
                    song => song.PlayedAt);

            DataList.ItemsSource = filteredSongs;

            _songsPlayingNowEnumerator = _songsPlayingNow.Where(song =>
                _settingsModel.MatchesCriteria(song)).GetEnumerator();

            SongsFetched.Text = filteredSongs.Count().ToString();
            FetchDateRange.Text = _settingsModel.ToString();
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
            _songs.Clear();

            try
            {
                using (ZipArchive archive = new(File.OpenRead(fileName), ZipArchiveMode.Read))
                {
                    ZipArchiveEntry songsEntry = archive.GetEntry(App.CsvSongsEntryName);

                    using (StreamReader reader = new(songsEntry.Open()))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            _songs.Add(PlaylistSong.FromCsvRow(new RealTimeProvider(), line));
                        }
                    }

                    ZipArchiveEntry settingsEntry = archive.GetEntry(App.JsonSettingsEntryName);
                    using (Stream stream = settingsEntry.Open())
                    {
                        _settingsModel = JsonSerializer.Deserialize<SettingsDialogModel>(stream);
                    }
                }

                UpdateUi();
            }
            catch (InvalidDataException)
            {
                 new MessageDialog("Invalid/corrupted file", $"Unable to open {fileName}")
                 {
                     Owner = this,
                 }.ShowDialog();
            }
        }

        private void SaveData(string fileName)
        {
            using (ZipArchive archive = new(File.OpenWrite(fileName), ZipArchiveMode.Create))
            {
                ZipArchiveEntry songsEntry = archive.CreateEntry(App.CsvSongsEntryName);

                using (StreamWriter writer = new(songsEntry.Open()))
                {
                    foreach (PlaylistSong song in _songs)
                    {
                        writer.WriteLine(song.ToCsvRow());
                    }
                }

                ZipArchiveEntry settingsEntry = archive.CreateEntry(App.JsonSettingsEntryName);
                using (Stream stream = settingsEntry.Open())
                {
                    JsonSerializer.Serialize(stream, _settingsModel);
                }            
            }

            new MessageDialog("Done", $"Data successfully saved into {fileName}")
            {
                Owner = this,
            }.ShowDialog();
        }

        private void Fetch_Click(object sender, RoutedEventArgs e)
        {
            FetchSongs(true);
        }

        private async void FetchSongs(bool inform = false)
        {
            FetchBtn.IsEnabled = false;
            _shouldRefetch = false;
            FetchSpinner.Visibility = Visibility.Visible;

            _songs.Clear();

            await foreach (PlaylistSong song in _miner.GetSongs(percent =>
            {
                FetchProgress.Value = percent;
            }))
            {
                _songs.Add(song);
            }

            FetchProgress.Value = 0;
            LastFetched.Text = DateTime.Now.ToString();
            UpdateUi();

            FetchBtn.IsEnabled = true;
            FetchSpinner.Visibility = Visibility.Hidden;

            FetchSongsPlayingNow();

            if (inform)
            {
                new MessageDialog("Done", "Successfully fetched")
                {
                    Owner = this,
                }.ShowDialog();
            }
        }

        private async void FetchSongsPlayingNow()
        {
            if (_songPlayingNowTimer != null)
            {
                _songPlayingNowTimer.Stop();
            }

            if (_autoFetchTimer != null)
            {
                _autoFetchTimer.Stop();
            }

            _songsPlayingNow.Clear();

            await foreach (PlaylistSong song in _miner.GetSongsNow())
            {
                _songsPlayingNow.Add(song);
            }

            UpdateUi();

            SetNextSongPlayingNow();
            SetSongPlayingNowTimer();

            SetAutoFetchTimer();
        }

        private void SetAutoFetchTimer()
        {         
            _autoFetchTimer = new()
            {
                Interval = App.AutoFetchInterval,
            };

            _autoFetchTimer.Tick += AutoFetchTimer_Tick;
            _autoFetchTimer.Start();
        }

        private void AutoFetchTimer_Tick(object sender, EventArgs e)
        {
            FetchSongsPlayingNow();
        }

        private void SetSongPlayingNowTimer()
        {
            _songPlayingNowTimer = new()
            {
                Interval = App.ChangeSongPlayingNowInterval,
            };

            _songPlayingNowTimer.Tick += SetSongPlayingNowTimer_Tick;
            _songPlayingNowTimer.Start();
        }

        private void SetSongPlayingNowTimer_Tick(object sender, EventArgs e)
        {
            SetNextSongPlayingNow();
        }

        private void SetNextSongPlayingNow()
        {
            if (!_songsPlayingNowEnumerator.MoveNext()) {
                _songsPlayingNowEnumerator = _songsPlayingNow.GetEnumerator();
                _songsPlayingNowEnumerator.MoveNext();
            }

            PlaylistSong song = _songsPlayingNowEnumerator.Current;

            if (song == null)
            {
                PlayingNow.Text = $"no songs";
            }
            else
            {
                PlayingNow.Text = $"{song} ({song.SourceStation})";
            }
            
        }

        private void EditSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsDialogModel settings = new(_miner)
            {
                PlayedFrom = _settingsModel.PlayedFrom,
                PlayedTo = _settingsModel.PlayedTo,
                DaysOfWeek = _settingsModel.DaysOfWeek.ToHashSet(),
                Artists = _settingsModel.Artists?.ToHashSet(),
                Tracks = _settingsModel.Tracks?.ToHashSet(),
            };

            EditSettingsDialog(settings);
        }

        private void EditSettingsDialog(SettingsDialogModel settings)
        {
            SettingsDialog dialog = new(settings)
            {
                Owner = this,
            };

            if (dialog.ShowDialog() == true)
            {
                _settingsModel = dialog.Model;
                UpdateShouldRefetch();
                _miner = new PlaylistMiner(new RealTimeProvider(), _settingsModel.From.ToUniversalTime(), _settingsModel.To.ToUniversalTime(), _settingsModel.SourceStations);
                UpdateUi();
            }
        }

        private void UpdateShouldRefetch()
        {
            _shouldRefetch = _settingsModel.From < _miner.From.ToDateTime(new TimeOnly(0))
                || _settingsModel.To > _miner.To.ToDateTime(new TimeOnly(23, 59, 59, 999))
                || _settingsModel.SourceStations.Any(st =>
                    !_miner.SourceStations.Contains(st));
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
            LeaderBoardDialog dialog = new(_songs.Where(song =>
                _settingsModel.MatchesCriteria(song)))
            {
                Owner = this,
            };

            dialog.ShowDialog();
        }

        private void DataListItem_Click(
            object sender, DataGridBeginningEditEventArgs e)
        {
            PlaylistSong song = (PlaylistSong)e.Row.DataContext;

            e.Cancel = true;

            SettingsDialogModel settings = new(_miner)
            {
                PlayedFrom = _settingsModel.PlayedFrom,
                PlayedTo = _settingsModel.PlayedTo,
                DaysOfWeek = _settingsModel.DaysOfWeek.ToHashSet(),
                Artists = new() { song.Artist },
                Tracks = new() { song.Title },
            };
            EditSettingsDialog(settings);
        }
    }
}
