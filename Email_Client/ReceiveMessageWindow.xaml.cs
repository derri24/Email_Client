using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using MimeKit;
using mshtml;
using MessageBox = System.Windows.MessageBox;

namespace Email_Client
{
    public partial class ReceiveMessageWindow : Window
    {
        private int countMessages;

        //if < 24 ,,counter - 24>0 индекс получения и стрелки 
        private const int CountOfMessagesOnPage = 24;
        private int counter = 24;
        private int number = 1;
        private int index;

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

      
        private void LoadWindow(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            EmailLabel.Content = mainWindow.EmailTextBox.Text;
            
            Receiver.TypeMessage = MessageType.Sent;
            countMessages = Receiver.GetCountMessages();
            SentMessagesBtn.Content = $"Исходящие: {countMessages}";
            Receiver.TypeMessage = MessageType.Received;
            countMessages = Receiver.GetCountMessages();
            ReceivedMessagesBtn.Content = $"Входящие: {countMessages}";
            GetMessages();
            ReceivedMessagesBtn.Background = Brushes.LightGray;
        }

        private void LeftArrowButton_Click(object sender, RoutedEventArgs e)
        {
            if (counter % CountOfMessagesOnPage == 0 && counter - CountOfMessagesOnPage > 0)
            {
                MessagesListBox.Items.Clear();
                counter -= CountOfMessagesOnPage;
                List<string> listOfMessages = Receiver.GetHeaders(counter - CountOfMessagesOnPage, counter);
                for (int i = 0; i < listOfMessages.Count && i < counter; i++)
                    MessagesListBox.Items.Add(listOfMessages[i]);
                number--;
                PageNumberLabel.Content = number.ToString();
            }
            else if (counter % CountOfMessagesOnPage != 0 && counter - CountOfMessagesOnPage > 0)
            {
                MessagesListBox.Items.Clear();
                List<string> listOfMessages = Receiver.GetHeaders(
                    counter - (counter % CountOfMessagesOnPage) - CountOfMessagesOnPage,
                    counter - (counter % CountOfMessagesOnPage));
                for (int i = 0; i < listOfMessages.Count && i < counter; i++)
                    MessagesListBox.Items.Add(listOfMessages[i]);
                counter -= counter % CountOfMessagesOnPage;
                number--;
                PageNumberLabel.Content = number.ToString();
            }
        }

        private void RightArrowButton_Click(object sender, RoutedEventArgs e)
        {
            if (counter + CountOfMessagesOnPage <= countMessages)
            {
                MessagesListBox.Items.Clear();
                List<string> listOfMessages = Receiver.GetHeaders(counter, counter + CountOfMessagesOnPage);
                for (int i = 0; i < listOfMessages.Count; i++)
                    MessagesListBox.Items.Add(listOfMessages[i]);
                counter += CountOfMessagesOnPage;
                number++;
                PageNumberLabel.Content = number.ToString();
            }
            else if (counter != countMessages)
            {
                MessagesListBox.Items.Clear();
                List<string> listOfMessages = Receiver.GetHeaders(counter, counter + countMessages % counter);
                for (int i = 0; i < listOfMessages.Count; i++)
                    MessagesListBox.Items.Add(listOfMessages[i]);
                counter += countMessages % counter;
                number++;
                PageNumberLabel.Content = number.ToString();
            }
        }

        private void ComeBackButton_Click(object sender, RoutedEventArgs e)
        {
            ShowListBoxOfMessages();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Receiver.Update();
                ShowListBoxOfMessages();
                countMessages = Receiver.GetCountMessages();
                if (countMessages < CountOfMessagesOnPage)
                    counter = countMessages;
                else
                    counter = CountOfMessagesOnPage;
                if (Receiver.TypeMessage == MessageType.Received)
                    ReceivedMessagesBtn.Content = $"Входящие: {countMessages}";
                else
                    SentMessagesBtn.Content = $"Исходящие: {countMessages}";
                MessagesListBox.Items.Clear();
                GetMessages();
            }
            catch
            {
                MessageBox.Show("Ошибка переподключения!\nПроверьте подключение к интернету!");
            }
        }

        private void ShowListBoxOfMessages()
        {
            MyWebBrowser.Visibility = Visibility.Hidden;
            ComeBackBtn.Visibility = Visibility.Hidden;
            SaveAttachmentsBtn.Visibility = Visibility.Hidden;
            MessagesListBox.Visibility = Visibility.Visible;
            PageNumberLabel.Visibility = Visibility.Visible;
            LeftArrowBtn.Visibility = Visibility.Visible;
            RightArrowBtn.Visibility = Visibility.Visible;
        }

    

        private void ListBoxItemDouble_Click(object sender, MouseButtonEventArgs e)
        {
            MyWebBrowser.Visibility = Visibility.Visible;
            ComeBackBtn.Visibility = Visibility.Visible;
            SaveAttachmentsBtn.Visibility = Visibility.Visible;
            MessagesListBox.Visibility = Visibility.Hidden;
            PageNumberLabel.Visibility = Visibility.Hidden;
            LeftArrowBtn.Visibility = Visibility.Hidden;
            RightArrowBtn.Visibility = Visibility.Hidden;

            if (counter < CountOfMessagesOnPage)
                index = MessagesListBox.SelectedIndex;
            else
                index = (number - 1) * CountOfMessagesOnPage + MessagesListBox.SelectedIndex;
            var message = Receiver.GetMessageByIndex(index);
            Stream stream = new MemoryStream(Encoding.Default.GetBytes(message));
            MyWebBrowser.NavigateToStream(stream);
        }


        private void SaveAttachmentsButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            var attachments = Receiver.GetAttachments(index);
            if (attachments.Count() > 0)
            {
                MessageBox.Show("Вложений нет!");
                return;
            }

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            foreach (MimeEntity attachment in attachments)
            {
                using (var stream = File.Create($"{dialog.SelectedPath}\\{attachment.ContentDisposition.FileName}"))
                {
                    if (attachment is MessagePart)
                    {
                        var part = (MessagePart) attachment;
                        part.Message.WriteTo(stream);
                    }
                    else
                    {
                        var part = (MimePart) attachment;
                        part.Content.DecodeTo(stream);
                    }
                }
            }

            MessageBox.Show("Вложения успешно скачаны!");
        }
        


        //количества может ыть меньше 24
        private void GetMessages()
        {
            number = 1;
            List<string> listOfMessages;
            listOfMessages = Receiver.GetHeaders(0, counter);
            for (int i = 0; i < listOfMessages.Count && i < counter; i++)
                MessagesListBox.Items.Add(listOfMessages[i]);
            PageNumberLabel.Content = "1";
            ComeBackBtn.Visibility = Visibility.Hidden;
        }

        private void ReceivedMessagesButton_Click(object sender, RoutedEventArgs e)
        {
            counter = CountOfMessagesOnPage;
            ShowListBoxOfMessages();
            MessagesListBox.Items.Clear();
            ReceivedMessagesBtn.Background = Brushes.LightGray;
            SentMessagesBtn.Background = Brushes.Turquoise;
            Receiver.TypeMessage = MessageType.Received;
            countMessages = Receiver.GetCountMessages();
            if (countMessages < counter)
                counter = countMessages;
            GetMessages();
        }

        private void SentMessagesButton_Click(object sender, RoutedEventArgs e)
        {
            counter = CountOfMessagesOnPage;
            ShowListBoxOfMessages();
            MessagesListBox.Items.Clear();
            SentMessagesBtn.Background = Brushes.LightGray;
            ReceivedMessagesBtn.Background = Brushes.Turquoise;
            Receiver.TypeMessage = MessageType.Sent;
            countMessages = Receiver.GetCountMessages();
            if (countMessages < counter)
                counter = countMessages;
            GetMessages();
        }
    }
}