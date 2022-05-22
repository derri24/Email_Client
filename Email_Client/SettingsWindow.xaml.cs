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


        private string host;
        private string port;

        // private void Load(object sender, RoutedEventArgs e)
        // {
        //     if (host != "" && port != "")
        //     {
        //         SendHostTextBox.Text = host;
        //         SendPortTextBox.Text = port;
        //     }
        // }

        private void SaveSettings_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (SendHostTextBox.Text != "" || SendPortTextBox.Text != "")
            {
                MainWindow mainWindow = new MainWindow();
                try
                {
                    Sender.Authorization(SendHostTextBox.Text, Convert.ToInt32(SendPortTextBox.Text),
                        mainWindow.EmailTextBox.Text, mainWindow.PasswordTextBox.Password,(bool)Ssl.IsChecked);
                    host = SendHostTextBox.Text;
                    port = SendPortTextBox.Text;
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