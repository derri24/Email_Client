using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using MimeKit;
using mshtml;

namespace Email_Client
{
    public partial class ReceiveMessageWindow : Window
    {
        private int CountMessages;

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

//if < 24 ,,counter - 24>0 индекс получения и стрелки 
        private const int countOfMessagesOnPage = 24;
        private int Counter = 24;
        private int Number = 1;

        private void LoadListBox(object sender, RoutedEventArgs e)
        {
            Receiver.TypeMessage = 1; 
            CountMessages = Receiver.GetCountMessages();
            SentMessagesButton.Content = $"Исходящие: {CountMessages}";
            Receiver.TypeMessage = 0; //recei
            CountMessages = Receiver.GetCountMessages();
            RecievedMessagesButton.Content = $"Входящие: {CountMessages}";
            GetMessages();
            RecievedMessagesButton.Background = Brushes.LightGray;
        }


        private void LeftArrowClick(object sender, RoutedEventArgs e)
        {
            if (Counter % countOfMessagesOnPage == 0 && Counter - countOfMessagesOnPage > 0)
            {
                ListBox.Items.Clear();
                Counter -= countOfMessagesOnPage;
                List<string> listOfMessages = Receiver.GetHeaders(Counter - countOfMessagesOnPage, Counter);
                for (int i = 0; i < listOfMessages.Count && i < Counter; i++)
                    ListBox.Items.Add(listOfMessages[i]);
                Number--;
                PageNumberLabel.Content = Number.ToString();
            }
            else if (Counter % countOfMessagesOnPage != 0 && Counter - countOfMessagesOnPage > 0)
            {
                ListBox.Items.Clear();
                List<string> listOfMessages = Receiver.GetHeaders(
                    Counter - (Counter % countOfMessagesOnPage) - countOfMessagesOnPage,
                    Counter - (Counter % countOfMessagesOnPage));
                for (int i = 0; i < listOfMessages.Count && i < Counter; i++)
                    ListBox.Items.Add(listOfMessages[i]);
                Counter -= Counter % countOfMessagesOnPage;
                Number--;
                PageNumberLabel.Content = Number.ToString();
            }
        }

        private void RightArrowClick(object sender, RoutedEventArgs e)
        {
            if (Counter + countOfMessagesOnPage <= CountMessages)
            {
                ListBox.Items.Clear();
                List<string> listOfMessages = Receiver.GetHeaders(Counter, Counter + countOfMessagesOnPage);
                for (int i = 0; i < listOfMessages.Count; i++)
                    ListBox.Items.Add(listOfMessages[i]);
                Counter += countOfMessagesOnPage;
                Number++;
                PageNumberLabel.Content = Number.ToString();
            }
            else if (Counter != CountMessages)
            {
                ListBox.Items.Clear();
                List<string> listOfMessages = Receiver.GetHeaders(Counter, Counter + CountMessages % Counter);
                for (int i = 0; i < listOfMessages.Count; i++)
                    ListBox.Items.Add(listOfMessages[i]);
                Counter += CountMessages % Counter;
                Number++;
                PageNumberLabel.Content = Number.ToString();
            }
        }

        private void ComeBackClick(object sender, RoutedEventArgs e)
        {
            ShowListBoxOfMessages();
        }

        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Receiver.CloseConnection();
                MainWindow mainWindow = new MainWindow();
                Receiver.Authorization(mainWindow.ReceiptHostTextBox.Text, Convert.ToInt32(mainWindow.ReceiptPortTextBox.Text),
                    mainWindow.EmailTextBox.Text, mainWindow.PasswordTextBox.Password,(bool)mainWindow.Ssl.IsChecked);
                CountMessages = Receiver.GetCountMessages();
                if (CountMessages<countOfMessagesOnPage)
                    Counter = CountMessages;
                else
                    Counter = countOfMessagesOnPage;
                if (Receiver.TypeMessage==0)
                    RecievedMessagesButton.Content = $"Входящие: {CountMessages}";
                else
                    SentMessagesButton.Content = $"Исходящие: {CountMessages}";
                ListBox.Items.Clear();
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
            ComeBackButton.Visibility = Visibility.Hidden;
            ListBox.Visibility = Visibility.Visible;
            PageNumberLabel.Visibility = Visibility.Visible;
            LeftArrowButton.Visibility = Visibility.Visible;
            RightArrowButton.Visibility = Visibility.Visible;
        }

        private void ItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MyWebBrowser.Visibility = Visibility.Visible;
            ComeBackButton.Visibility = Visibility.Visible;
            ListBox.Visibility = Visibility.Hidden;
            PageNumberLabel.Visibility = Visibility.Hidden;
            LeftArrowButton.Visibility = Visibility.Hidden;
            RightArrowButton.Visibility = Visibility.Hidden;
            int index;
            int i;
            if (Counter < countOfMessagesOnPage)
                index = ListBox.SelectedIndex;
            else
                index=(Number-1)*countOfMessagesOnPage+ListBox.SelectedIndex;
            var message = Receiver.GetMessageByIndex(index);
            Stream stream = new MemoryStream(Encoding.Default.GetBytes(message));
            MyWebBrowser.NavigateToStream(stream);
        }

        private void LoadWindow(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            EmailLabel.Content = mainWindow.EmailTextBox.Text;
        }


        //количества может ыть меньше 24
        private void GetMessages()
        {
            Number = 1;
            List<string> listOfMessages;
            listOfMessages = Receiver.GetHeaders(0, Counter);
            for (int i = 0; i < listOfMessages.Count && i < Counter; i++)
                ListBox.Items.Add(listOfMessages[i]);
            PageNumberLabel.Content = "1";
            ComeBackButton.Visibility = Visibility.Hidden;
        }

        private void ReceivedMessagesClick(object sender, RoutedEventArgs e)
        {
            Counter = countOfMessagesOnPage;
            ShowListBoxOfMessages();
            ListBox.Items.Clear();
            RecievedMessagesButton.Background = Brushes.LightGray;
            SentMessagesButton.Background = Brushes.Turquoise;
            Receiver.TypeMessage = 0;
            CountMessages = Receiver.GetCountMessages();
            if (CountMessages < Counter)
                Counter = CountMessages;
            GetMessages();
        }

        private void SentMessagesClick(object sender, RoutedEventArgs e)
        {
            Counter = countOfMessagesOnPage;
            ShowListBoxOfMessages();
            ListBox.Items.Clear();
            SentMessagesButton.Background = Brushes.LightGray;
            RecievedMessagesButton.Background = Brushes.Turquoise;
            Receiver.TypeMessage = 1;
            CountMessages = Receiver.GetCountMessages();
            if (CountMessages < Counter)
                Counter = CountMessages;
            GetMessages();
        }
    }
}