using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using MimeKit;
using mshtml;
using WpfAnimatedGif;
using MessageBox = System.Windows.MessageBox;

namespace Email_Client
{
    public partial class ReceiveMessageWindow : Window
    {
        private int countMessages;

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

        private void ChangeSearchBoxText(object sender, TextChangedEventArgs e)
        {
            if (SearchBox.Text == "")
            {
                Receiver.IsSearch = false;
            }
        }

        private bool openAccess = false;

        private void AddLoading()
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("loading.gif");
            image.EndInit();
            ImageBehavior.SetAnimatedSource(new Image(), image);
        }
        private async void LoadWindow(object sender, RoutedEventArgs e)
        {
            // AddLoading();
            openAccess = false;
            MessagesListBox.IsEnabled = false;
            Receiver.TypeMessage = MessageType.Sent;
            countMessages = await Receiver.GetCountMessages();

            MainWindow mainWindow = new MainWindow();
            EmailLabel.Content = mainWindow.EmailTextBox.Text;
            SentMessagesBtn.Content = $"Исходящие: {countMessages}";

            Receiver.TypeMessage = MessageType.Received;
            countMessages = await Receiver.GetCountMessages();

            ReceivedMessagesBtn.Content = $"Входящие: {countMessages}";
            ReceivedMessagesBtn.Background = Brushes.LightGray;
            await GetMessages();
            openAccess = true;
            MessagesListBox.IsEnabled = true;
            
        }

        private async void LeftArrowButton_Click(object sender, RoutedEventArgs e)
        {
            if (!openAccess)
                return;
            openAccess = false;
            MessagesListBox.IsEnabled = false;
            if (counter % CountOfMessagesOnPage == 0 && counter - CountOfMessagesOnPage > 0)
            {
                MessagesListBox.Items.Clear();
                counter -= CountOfMessagesOnPage;
                List<string> listOfMessages;
                if (Receiver.IsSearch)
                    listOfMessages =
                        await Receiver.GetFoundHeaders(counter - CountOfMessagesOnPage, counter, SearchBox.Text);
                else
                    listOfMessages = await Receiver.GetHeaders(counter - CountOfMessagesOnPage, counter);

                for (int i = 0; i < listOfMessages.Count && i < counter; i++)
                    MessagesListBox.Items.Add(listOfMessages[i]);
                number--;
                PageNumberLabel.Content = number.ToString();
            }
            else if (counter % CountOfMessagesOnPage != 0 && counter - CountOfMessagesOnPage > 0)
            {
                MessagesListBox.Items.Clear();
                List<string> listOfMessages;
                if (Receiver.IsSearch)
                    listOfMessages = await Receiver.GetFoundHeaders(
                        counter - (counter % CountOfMessagesOnPage) - CountOfMessagesOnPage,
                        counter - (counter % CountOfMessagesOnPage), SearchBox.Text);
                else
                    listOfMessages = await Receiver.GetHeaders(
                        counter - (counter % CountOfMessagesOnPage) - CountOfMessagesOnPage,
                        counter - (counter % CountOfMessagesOnPage));

                for (int i = 0; i < listOfMessages.Count && i < counter; i++)
                    MessagesListBox.Items.Add(listOfMessages[i]);
                counter -= counter % CountOfMessagesOnPage;
                number--;
                PageNumberLabel.Content = number.ToString();
            }

            openAccess = true;
            MessagesListBox.IsEnabled = true;
        }

        private async void RightArrowButton_Click(object sender, RoutedEventArgs e)
        {
            if (!openAccess)
                return;
            openAccess = false;
            MessagesListBox.IsEnabled = false;
            if (counter + CountOfMessagesOnPage <= countMessages)
            {
                MessagesListBox.Items.Clear();
                List<string> listOfMessages;
                if (Receiver.IsSearch)
                    listOfMessages =
                        await Receiver.GetFoundHeaders(counter, counter + CountOfMessagesOnPage, SearchBox.Text);
                else listOfMessages = await Receiver.GetHeaders(counter, counter + CountOfMessagesOnPage);

                for (int i = 0; i < listOfMessages.Count; i++)
                    MessagesListBox.Items.Add(listOfMessages[i]);
                counter += CountOfMessagesOnPage;
                number++;
                PageNumberLabel.Content = number.ToString();
            }
            else if (counter != countMessages)
            {
                MessagesListBox.Items.Clear();
                List<string> listOfMessages;
                if (Receiver.IsSearch)
                    listOfMessages =
                        await Receiver.GetFoundHeaders(counter, counter + countMessages % counter, SearchBox.Text);
                else listOfMessages = await Receiver.GetHeaders(counter, counter + countMessages % counter);
                for (int i = 0; i < listOfMessages.Count; i++)
                    MessagesListBox.Items.Add(listOfMessages[i]);
                counter += countMessages % counter;
                number++;
                PageNumberLabel.Content = number.ToString();
            }

            openAccess = true;
            MessagesListBox.IsEnabled = true;
        }

        private void ComeBackButton_Click(object sender, RoutedEventArgs e)
        {
            ShowListBoxOfMessages();
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!openAccess)
                return;
            openAccess = false;
            MessagesListBox.IsEnabled = false;
            try
            {
                await Receiver.Update();
                ShowListBoxOfMessages();
                SearchBox.Text = "";

                countMessages = await Receiver.GetCountMessages();
                if (countMessages < CountOfMessagesOnPage)
                    counter = countMessages;
                else
                    counter = CountOfMessagesOnPage;
                if (Receiver.TypeMessage == MessageType.Received)
                    ReceivedMessagesBtn.Content = $"Входящие: {countMessages}";

                else if (Receiver.TypeMessage == MessageType.Sent)
                    SentMessagesBtn.Content = $"Исходящие: {countMessages}";

                MessagesListBox.Items.Clear();
                await GetMessages();
            }
            catch
            {
                MessageBox.Show("Ошибка переподключения!\nПроверьте подключение к интернету!");
            }

            openAccess = true;
            MessagesListBox.IsEnabled = true;
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


        private async void ListBoxItemDouble_Click(object sender, MouseButtonEventArgs e)
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
            var attachments = Receiver.GetAttachments(index);
            if (attachments.Count() > 0)
                SaveAttachmentsBtn.Visibility = Visibility.Visible;
            else
                SaveAttachmentsBtn.Visibility = Visibility.Hidden;
            var message = Receiver.GetMessageByIndex(index);
            Stream stream = new MemoryStream(Encoding.Default.GetBytes(message));
            MyWebBrowser.NavigateToStream(stream);
        }


        private async void SaveAttachmentsButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            var attachments = Receiver.GetAttachments(index);

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


        private async Task GetMessages()
        {
            number = 1;
            List<string> listOfHeaders;
            if (Receiver.IsSearch)
                listOfHeaders = await Receiver.GetFoundHeaders(0, counter, SearchBox.Text);
            else
                listOfHeaders = await Receiver.GetHeaders(0, counter);
            for (int i = 0; i < listOfHeaders.Count && i < counter; i++)
                MessagesListBox.Items.Add(listOfHeaders[i]);
            PageNumberLabel.Content = number.ToString();
            ComeBackBtn.Visibility = Visibility.Hidden;
        }

        private async void ReceivedMessagesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!openAccess)
                return;
            openAccess = false;
            MessagesListBox.IsEnabled = false;
            SearchBox.Text = "";
            counter = CountOfMessagesOnPage;
            ShowListBoxOfMessages();
            MessagesListBox.Items.Clear();
            ReceivedMessagesBtn.Background = Brushes.LightGray;
            SentMessagesBtn.Background = Brushes.Turquoise;
            Receiver.TypeMessage = MessageType.Received;
            countMessages = await Receiver.GetCountMessages();
            if (countMessages < counter)
                counter = countMessages;
            await GetMessages();
            openAccess = true;
            MessagesListBox.IsEnabled = true;
        }

        private async void SentMessagesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!openAccess)
                return;
            openAccess = false;
            MessagesListBox.IsEnabled = false;
            SearchBox.Text = "";
            counter = CountOfMessagesOnPage;
            ShowListBoxOfMessages();
            MessagesListBox.Items.Clear();
            SentMessagesBtn.Background = Brushes.LightGray;
            ReceivedMessagesBtn.Background = Brushes.Turquoise;
            Receiver.TypeMessage = MessageType.Sent;
            countMessages = await Receiver.GetCountMessages();
            if (countMessages < counter)
                counter = countMessages;
            await GetMessages();
            openAccess = true;
            MessagesListBox.IsEnabled = true;
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (!openAccess)
                return;
            openAccess = false;
            MessagesListBox.IsEnabled = false;
            if (SearchBox.Text != "")
            {
                counter = CountOfMessagesOnPage;
                ShowListBoxOfMessages();
                MessagesListBox.Items.Clear();
                Receiver.IsSearch = true;
                countMessages = await Receiver.GetCountFoundMessages(SearchBox.Text);
                if (countMessages < counter)
                    counter = countMessages;
                if (countMessages == 0)
                {
                    MessageBox.Show(
                        "Не нашлось ни одного письма, попробуйте сформулировать запрос иначе.");
                    Receiver.IsSearch = false;
                    SearchBox.Text = "";
                    countMessages = await Receiver.GetCountMessages();
                    if (countMessages < CountOfMessagesOnPage)
                        counter = countMessages;
                    else
                        counter = CountOfMessagesOnPage;
                    await GetMessages();
                }
                else
                    await GetMessages();
            }
            else
            {
                MessageBox.Show(
                    "Ваш запрос пуст, попробуйте сформулировать запрос иначе.");
            }

            openAccess = true;
            MessagesListBox.IsEnabled = true;
        }
        
    }
}