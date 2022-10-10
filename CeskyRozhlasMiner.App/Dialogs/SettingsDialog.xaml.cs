using CeskyRozhlasMiner.App.Models.Controls;
using CeskyRozhlasMiner.Lib.Common;
using CeskyRozhlasMiner.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CeskyRozhlasMiner.Dialogs
{
    /// <summary>
    /// Interaction logic for SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : Window
    {
        private ObservableCollection<CheckBoxModel<RozhlasStation>> _sourceStationsObservable;
        private ObservableCollection<CheckBoxModel<DayOfWeek>> _daysOfWeekObservable;
        private const char _hashsetSeparator = ',';

        public SettingsDialogModel Model { get; private set; }

        public SettingsDialog(SettingsDialogModel model)
        {
            InitializeComponent();
            Title = $"Settings - {App.Title}";

            Model = model ?? throw new ArgumentNullException(nameof(model));
            SetByModel();
        }

        private void SetByModel()
        {
            MineFrom.SelectedDate = new DateTime(
                Model.From.Year, Model.From.Month, Model.From.Day);
            MineTo.SelectedDate = new DateTime(
                Model.To.Year, Model.To.Month, Model.To.Day);

            SetSourceStationsByModel();
            SetDaysInWeekByModel();

            PlayedFromHour.Text = Model.PlayedFrom.Hours.ToString();
            PlayedFromMinute.Text = Model.PlayedFrom.Minutes.ToString();
            PlayedFromSecond.Text = Model.PlayedFrom.Seconds.ToString();

            PlayedToHour.Text = Model.PlayedTo.Hours.ToString();
            PlayedToMinute.Text = Model.PlayedTo.Minutes.ToString();
            PlayedToSecond.Text = Model.PlayedTo.Seconds.ToString();

            ArtistsBox.Text = Model.Artists == null ? string.Empty :
                string.Join($"{_hashsetSeparator} ", Model.Artists);

            TracksBox.Text = Model.Tracks == null ? string.Empty :
                string.Join($"{_hashsetSeparator} ", Model.Tracks);
        }

        private void SetDaysInWeekByModel()
        {
            _daysOfWeekObservable = new();

            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                _daysOfWeekObservable.Add(new CheckBoxModel<DayOfWeek>()
                {
                    Label = day.ToString(),
                    IsChecked = Model.DaysOfWeek.Contains(day),
                    Value = day
                });
            }

            DaysOfWeek.ItemsSource = _daysOfWeekObservable;
        }

        private void SetSourceStationsByModel()
        {
            _sourceStationsObservable = new();

            foreach (RozhlasStation station in
                Enum.GetValues(typeof(RozhlasStation)))
            {
                _sourceStationsObservable.Add(new CheckBoxModel<RozhlasStation>()
                {
                    Label = station.ToString(),
                    IsChecked = Model.SourceStations.Contains(station),
                    Value = station
                });
            }

            SourceStations.ItemsSource = _sourceStationsObservable;
        }

        private bool Validate()
        {
            if (string.IsNullOrEmpty(MineFrom.Text)
                || MineFrom.SelectedDate.Value > DateTime.Now
                || MineFrom.SelectedDate.Value > MineTo.SelectedDate.Value)
            {
                MineFrom.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(MineTo.Text)
                || MineTo.SelectedDate.Value > DateTime.Now
                || MineTo.SelectedDate.Value < MineFrom.SelectedDate.Value)
            {
                MineTo.Focus();
                return false;
            }

            return ValidatePLayedTimes();
        }

        private bool ValidatePLayedTimes()
        {
            foreach (TextBox hourBox in new TextBox[]
            {
                PlayedFromHour,
                PlayedToHour,
            })
            {
                if (!int.TryParse(hourBox.Text, out int hour) || hour < 0 || hour > 23)
                {
                    hourBox.Focus();
                    return false;
                }
            }

            foreach (TextBox timeBox in new TextBox[]
            {
                PlayedFromMinute,
                PlayedFromSecond,
                PlayedToMinute,
                PlayedToSecond,
            })
            {

                if (!int.TryParse(timeBox.Text, out int n) || n < 0 || n > 59)
                {
                    timeBox.Focus();
                    return false;
                }
            }

            return true;
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                UpdateModelDateRange();
                UpdateModelSourceStations();
                UpdateModelDaysOfWeek();
                UpdateModelPlayedRange();
                UpdateModelArtists();
                UpdateModelTracks();
                DialogResult = true;
            }
        }

        private void UpdateModelPlayedRange()
        {
            Model.PlayedFrom = new TimeSpan(
                int.Parse(PlayedFromHour.Text),
                int.Parse(PlayedFromMinute.Text),
                int.Parse(PlayedFromSecond.Text)
            );

            Model.PlayedTo = new TimeSpan(
                int.Parse(PlayedToHour.Text),
                int.Parse(PlayedToMinute.Text),
                int.Parse(PlayedToSecond.Text)
            );
        }

        private void UpdateModelDateRange()
        {
            Model.From = MineFrom.SelectedDate.Value;
            Model.To = MineTo.SelectedDate.Value.AddDays(1).AddMilliseconds(-1);
        }

        private void UpdateModelSourceStations()
        {
            Model.SourceStations = _sourceStationsObservable
                .Where(st => st.IsChecked).Select(st => st.Value).ToHashSet();
        }

        private void UpdateModelDaysOfWeek()
        {
            Model.DaysOfWeek = _daysOfWeekObservable
                .Where(st => st.IsChecked).Select(d => d.Value).ToHashSet();
        }

        private void UpdateModelArtists()
        {
            if (string.IsNullOrEmpty(ArtistsBox.Text))
            {
                Model.Artists = null;
            }
            else
            {
                Model.Artists = ArtistsBox.Text.Split(_hashsetSeparator)
                    .Select(artist => artist.Trim())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
            }
        }

        private void UpdateModelTracks()
        {
            if (string.IsNullOrEmpty(TracksBox.Text))
            {
                Model.Tracks = null;
            }
            else
            {
                Model.Tracks = TracksBox.Text.Split(_hashsetSeparator)
                    .Select(artist => artist.Trim())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            Model = new SettingsDialogModel(new());
            SetByModel();
        }
    }
}
