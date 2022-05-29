﻿using System;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace Email_Client
{
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
                catch(Exception)
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