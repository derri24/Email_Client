using System.Windows;

namespace Email_Client
{
    public partial class ReceiveMessageWindow : Window
    {
        public ReceiveMessageWindow()
        {
            InitializeComponent();
        }
        private void WriteButton_Click(object sender, RoutedEventArgs e)
        {
             SendMessageWindow sendMessageWindow = new SendMessageWindow();
             sendMessageWindow.ShowDialog();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }
    }
}