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
            ReceiveMessageWindow receiveMessageWindow = new ReceiveMessageWindow();
            receiveMessageWindow.ShowDialog();
        }
    }
}