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

        private void SaveSettings_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (SendHostTextBox.Text != "" || SendPortTextBox.Text != "")
            {
                MainWindow mainWindow = new MainWindow();
                try
                {
                    Sender.Authorization(SendHostTextBox.Text, Convert.ToInt32(SendPortTextBox.Text),
                        mainWindow.EmailTextBox.Text, mainWindow.PasswordTextBox.Password,(bool)Ssl.IsChecked);
                    SettingsStorage.Host = SendHostTextBox.Text;
                    SettingsStorage.Port = SendPortTextBox.Text;
                    SettingsStorage.Ssl = (bool)Ssl.IsChecked;
                    MessageBox.Show("Данные успешно сохранены!");
                    Close();
                }
                catch
                {
                    MessageBox.Show("Ошибка подключения! Проверьте заполненные поля!");
                }
            }
            else
                MessageBox.Show("Ошибка! Заполнены не все поля!");
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Receiver.CloseConnection();
            Sender.CloseConnection();

            Environment.Exit(0);
        }
    }
}