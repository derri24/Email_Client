using System;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace Email_Client
{
    public partial class AuthorizationWindow
    {
        public AuthorizationWindow()
        {
            InitializeComponent();
        }

        private bool openAccess = true;

        private async void GoButton_Click(object sender, RoutedEventArgs e)
        {
            if (openAccess)
            {
                openAccess = false;
                if (ReceiptHostTextBox.Text == "" ||
                    ReceiptPortTextBox.Text == "" ||
                    EmailTextBox.Text == "" ||
                    PasswordTextBox.Password == "")
                {
                    MessageBox.Show("Ошибка! Заполнены не все поля!");
                    return;
                }

                try
                {
                    await Receiver.Authorization(ReceiptHostTextBox.Text, Convert.ToInt32(ReceiptPortTextBox.Text),
                        EmailTextBox.Text, PasswordTextBox.Password, (bool) Ssl.IsChecked);
                    await Sender.Authenticate(EmailTextBox.Text, PasswordTextBox.Password);

                    ReceiveMessageWindow receiveMessageWindow = new ReceiveMessageWindow();
                    receiveMessageWindow.ShowDialog();
                }
                catch(Exception)
                {
                    MessageBox.Show("Ошибка авторизации!\nПроверьте заполненные поля и подключение к интернету!");
                }
                openAccess = true;
            }
        }

        private void AuthorizationWindow_OnClosed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        
        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Для авторизации и получения сообщения необходимо подключиться к IMAP серверу. Для этого нужно заполнить поля, представленные в окне.");
        }
    }
}