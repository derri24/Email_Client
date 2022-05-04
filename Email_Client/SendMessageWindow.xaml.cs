using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Email_Client
{
    public partial class SendMessageWindow : Window
    {
        private string path;

        public SendMessageWindow()
        {
            InitializeComponent();
        }

        private void AttachmentButton_Click(object sender, RoutedEventArgs e)
        {
            path = "";
            var dialog = new Microsoft.Win32.OpenFileDialog();
            if (dialog.ShowDialog() == true)
                path = dialog.FileName;
            AttachmentBtn.Background = Brushes.LightGray;
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReceiverTextBox.Text != "" && MessageTextBox.Text != "")
            {
                MainWindow mainWindow = new MainWindow();
                SettingsWindow settingsWindow = new SettingsWindow();
                try
                {
                    Sender.Authorization(settingsWindow.SendHostTextBox.Text,
                        Convert.ToInt32(settingsWindow.SendPortTextBox.Text), mainWindow.EmailTextBox.Text,
                        mainWindow.PasswordTextBox.Text);

                    Sender.SendMessage(ReceiverTextBox.Text, SubjectTextBox.Text, MessageTextBox.Text, path);
                    MessageBox.Show("Сообщение успешно отправлено!");
                    AttachmentBtn.Background = Brushes.Turquoise;
                    Close();
                }
                catch
                {
                    MessageBox.Show("Ошибка при отправке сообщения! Проверьте настройки(хост, порт).");
                }
            }
            else
            {
                MessageBox.Show("Заполненение полей <Получатель>, <Текст сообщения> обязательно!");
            }
        }
    }
}