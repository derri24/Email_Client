using System.Windows;

namespace Email_Client
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SaveSettings_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (SendHostTextBox.Text != "" || SendPortTextBox.Text != "")
            {
                // Sender.Authorization();
                MessageBox.Show("Данные успешно сохранены!");
                Close();
            }
            else
                MessageBox.Show("Ошибка! Заполнены не все поля!");
        }
    }
}