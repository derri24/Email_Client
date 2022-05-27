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
        private List<string> _listOfPath;

        public SendMessageWindow()
        {
            InitializeComponent();
            _listOfPath = new List<string>();
        }

        private void AttachmentButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                _listOfPath.Add(path);
            }

            if (_listOfPath.Count > 0)
                AttachmentBtn.Background = Brushes.LightGray;
        }


        private string ChangeMessageText()
        {
            string resultMessageText = MessageTextBox.Text;
            var matches = Regex.Matches(MessageTextBox.Text, @"<a>((?:(?!<\/a>).)+)<\/a>");
            for (int i = 0; i < matches.Count; i++)
            {
                string link = matches[i].Groups[1].Value;
                resultMessageText = resultMessageText.Replace($"<a>{link}", $"<a href=\"{link}\">{link}");
            }

            resultMessageText = resultMessageText.Replace("\r\n", "<br>");
            return resultMessageText;
        }

        private async void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (Sender.IsConnected == false)
            {
                MessageBox.Show("Ошибка подключения! Пожалуйста, заполните настройки.");
                SettingsWindow settingsWindow = new SettingsWindow();
                settingsWindow.ShowDialog();
                return;
            }

            if (ReceiverTextBox.Text == "")
            {
                MessageBox.Show("Заполненение поля <Получатель> обязательно!");
                return;
            }

            try
            {
               await Sender.SendMessage(ReceiverTextBox.Text, SubjectTextBox.Text, ChangeMessageText(), _listOfPath);
                MessageBox.Show("Сообщение успешно отправлено!");
                AttachmentBtn.Background = Brushes.Turquoise;
                Close();
            }
            catch
            {
                MessageBox.Show("Ошибка при отправке сообщения! Проверьте настройки(хост, порт).");
            }
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            MessageTextBox.Text += "<b>Введите текст тут</b>";
        }

        private void CursiveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageTextBox.Text += "<i>Введите текст тут</i>";
        }

        private void LinkButton_Click(object sender, RoutedEventArgs e)
        {
            MessageTextBox.Text += "<a>Введите ссылку тут</a>";
        }
    }
}