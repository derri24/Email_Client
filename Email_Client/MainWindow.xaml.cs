using System;
using System.Windows;

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

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReceiptHostTextBox.Text != "" && ReceiptPortTextBox.Text != "" &&
                EmailTextBox.Text != "" & PasswordTextBox.Password != "")
            {
                try
                {
                    Receiver.Authorization(ReceiptHostTextBox.Text, Convert.ToInt32(ReceiptPortTextBox.Text),
                        EmailTextBox.Text, PasswordTextBox.Password,(bool)Ssl.IsChecked);
                    
                    ReceiveMessageWindow receiveMessageWindow = new ReceiveMessageWindow();
                    receiveMessageWindow.ShowDialog();
                }
                catch
                {
                    MessageBox.Show("Ошибка авторизации!\nПроверьте заполненные поля и подключение к интернету!");
                }
            }
            else
                MessageBox.Show("Ошибка! Заполнены не все поля!");
        }
    }
}