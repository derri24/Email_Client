using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace Email_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
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
                catch
                {
                    MessageBox.Show("Ошибка авторизации!\nПроверьте заполненные поля и подключение к интернету!");
                }

                openAccess = true;
            }
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}