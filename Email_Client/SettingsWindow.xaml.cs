using System;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Media.Animation;

namespace Email_Client
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            if (SettingsStorage.Host != null && SettingsStorage.Port != null)
            {
                SendHostTextBox.Text = SettingsStorage.Host;
                SendPortTextBox.Text = SettingsStorage.Port;
                Ssl.IsChecked = SettingsStorage.Ssl;
            }
        }

        private bool openAccess = true;
        private async void SaveSettings_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (openAccess )
            {
                openAccess = false;
                if (SendHostTextBox.Text == "" || SendPortTextBox.Text == "")
                {
                    MessageBox.Show("Ошибка! Заполнены не все поля!");
                    return;
                }

                try
                {
                    await Sender.Connect(SendHostTextBox.Text, Convert.ToInt32(SendPortTextBox.Text), (bool) Ssl.IsChecked);
                    SettingsStorage.Host = SendHostTextBox.Text;
                    SettingsStorage.Port = SendPortTextBox.Text;
                    SettingsStorage.Ssl = (bool) Ssl.IsChecked;
                    MessageBox.Show("Данные успешно сохранены!");
                    Close();
                }
                catch
                {
                    MessageBox.Show("Ошибка подключения! Проверьте заполненные поля!");
                } 
                openAccess = true;
            }
            
        }

        private async void ExitButton_Click(object sender, RoutedEventArgs e)
        {
             await Receiver.CloseConnection();
            await Sender.CloseConnection();

            Environment.Exit(0);
        }
    }
}