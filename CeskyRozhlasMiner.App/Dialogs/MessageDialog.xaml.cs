using System.Windows;

namespace CeskyRozhlasMiner.Dialogs
{
    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : Window
    {
        public string Message { get; private set; }

        public MessageDialog(string title, string message)
        {
            InitializeComponent();
            Title = $"{title} - {App.Title}";
            Message = message;
            MessageBlock.Text = message;
        }


        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
