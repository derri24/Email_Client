using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Email_Client
{
    public partial class SendMessageWindow : Window
    {
        private List<string> listOfPath;

        public SendMessageWindow()
        {
            InitializeComponent();
            listOfPath = new List<string>();
        }

        private void AttachmentButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                listOfPath.Add(path);
            }

            if (listOfPath.Count > 0)
                AttachmentBtn.Background = Brushes.LightGray;
        }


        private string ChangeMessageText()
        {
            string resultMessageText = MessageTextBox.Text;
            var matches = Regex.Matches(MessageTextBox.Text, @"<a>((?:(?!<\/a>).)+)<\/a>");
            for (int i = 0; i < matches.Count; i++)
                resultMessageText = resultMessageText.Replace($"<a>{matches[i].Groups[1].Value}",
                    $"<a href=\"{matches[i].Groups[1].Value}\">{matches[i].Groups[1].Value}");
            resultMessageText = resultMessageText.Replace("\r\n", "<br>");
            return resultMessageText;
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReceiverTextBox.Text != "")
            {
                MainWindow mainWindow = new MainWindow();
                SettingsWindow settingsWindow = new SettingsWindow();
                try
                {
                    Sender.Authorization(settingsWindow.SendHostTextBox.Text,
                        Convert.ToInt32(settingsWindow.SendPortTextBox.Text), mainWindow.EmailTextBox.Text,
                        mainWindow.PasswordTextBox.Password, (bool) settingsWindow.Ssl.IsChecked);

                    Sender.SendMessage(ReceiverTextBox.Text, SubjectTextBox.Text, ChangeMessageText(), listOfPath);
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
                MessageBox.Show("Заполненение поля <Получатель> обязательно!");
            }
        }

        private void BoldButtonClick(object sender, RoutedEventArgs e)
        {
            MessageTextBox.Text += "<b>Введите текст тут</b>";
        }

        private void CursiveButtonClick(object sender, RoutedEventArgs e)
        {
            MessageTextBox.Text += "<i>Введите текст тут</i>";
        }

        private void LinkButtonClick(object sender, RoutedEventArgs e)
        {
            MessageTextBox.Text += "<a>Введите ссылку тут</a>";
        }
    }
}