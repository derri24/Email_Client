using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        private const int countOfMessagesOnPage = 3;
        private int Counter = 3;
        private int Number = 1;
        
        private void LoadListBox(object sender, RoutedEventArgs e)
        {
            CountMessages = Receiver.GetCountMessages();

            List<string> listOfMessages = Receiver.ReceiveHeaders(0, Counter);
            for (int i = 0; i < listOfMessages.Count && i < Counter; i++)
                ListBox.Items.Add(listOfMessages[i]);
            PageNumberLabel.Content = "1";
            ComeBackButton.Visibility = Visibility.Hidden;
            
        }
        

        //пересмотреть клики на кнопки
        private void LeftArrowClick(object sender, RoutedEventArgs e)
        {
            ListBox.Items.Clear();
            if (Counter % countOfMessagesOnPage == 0 && Counter - countOfMessagesOnPage >= 0)
            {
                List<string> listOfMessages = Receiver.ReceiveHeaders(Counter - countOfMessagesOnPage, Counter);
                for (int i = 0; i < listOfMessages.Count && i < Counter; i++)
                    ListBox.Items.Add(listOfMessages[i]);
                Counter -= countOfMessagesOnPage;
            }
            else if (Counter % countOfMessagesOnPage != 0 && Counter - countOfMessagesOnPage >= 0 )
            {
                List<string> listOfMessages = Receiver.ReceiveHeaders(Counter - (Counter % countOfMessagesOnPage), Counter);
                for (int i = 0; i < listOfMessages.Count && i < Counter; i++)
                    ListBox.Items.Add(listOfMessages[i]);
                Counter -= Counter % countOfMessagesOnPage;
            }
            Number--;
            PageNumberLabel.Content = Number.ToString();
        }

        private void RightArrowClick(object sender, RoutedEventArgs e)
        {
            if (Counter + countOfMessagesOnPage <= CountMessages)
            {
                ListBox.Items.Clear();
                List<string> listOfMessages = Receiver.ReceiveHeaders(Counter, Counter + countOfMessagesOnPage);
                for (int i = 0; i < listOfMessages.Count; i++)
                    ListBox.Items.Add(listOfMessages[i]);
                Counter += countOfMessagesOnPage;
                Number++;
            }
            else if (Counter!= CountMessages)
            {
                ListBox.Items.Clear();
                List<string> listOfMessages = Receiver.ReceiveHeaders(Counter, Counter + CountMessages % Counter);
                for (int i = 0; i < listOfMessages.Count; i++)
                    ListBox.Items.Add(listOfMessages[i]);
                Counter += CountMessages % Counter;
                Number++;
            }
            PageNumberLabel.Content = Number.ToString();
        }

        private void ComeBackClick(object sender, RoutedEventArgs e)
        {
            MyWebBrowser.Visibility = Visibility.Hidden;
            ComeBackButton.Visibility = Visibility.Hidden;
            ListBox.Visibility = Visibility.Visible;
            PageNumberLabel.Visibility = Visibility.Visible;
            LeftArrowButton.Visibility = Visibility.Visible;
            RightArrowButton.Visibility = Visibility.Visible;
        }
        
        private void ListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MyWebBrowser.Visibility = Visibility.Visible;
            ComeBackButton.Visibility = Visibility.Visible;
            ListBox.Visibility = Visibility.Hidden;
            PageNumberLabel.Visibility = Visibility.Hidden;
            LeftArrowButton.Visibility = Visibility.Hidden;
            RightArrowButton.Visibility = Visibility.Hidden;
            MyWebBrowser.NavigateToString(Receiver.GetMessageBodyByIndex(ListBox.SelectedIndex).ToString());

        }
        
    }
}