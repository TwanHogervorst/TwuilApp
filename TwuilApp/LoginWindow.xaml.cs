using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TwuilAppClient;
using TwuilAppClient.Core;

namespace TwuilApp
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private Client client = null;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            string ip = this.ServerIpTextBox.Text;
            string username = this.UsernameTextBox.Text;
            string password = this.PasswordTextBox.Password;

            try
            {
                this.client = new Client(ip, Constants.SERVER_PORT);

                this.client.OnLoginResponseReceived += Client_OnLoginResponseReceived;
                this.client.OnServerClosing += Client_OnServerClosing;

                this.client.Login(username, password);
            }
            catch (Exception ex)
            {
                this.ShowError($"Something went wrong while connecting to the server. {ex.Message}");
            }
        }

        private void Client_OnServerClosing(Client sender, string reason)
        {
            MessageBox.Show(reason, "Server closed", MessageBoxButton.OK, MessageBoxImage.Warning);
            this.client = null;

            this.Dispatcher.Invoke(() =>
            {
                this.Close();
            });
        }

        private void Client_OnLoginResponseReceived(Client sender, bool success, string errorMessage)
        {
            if(success)
            {
                MessageBox.Show("Login successfull!", "Login success", MessageBoxButton.OK);
            }
            else
            {
                this.ShowError(errorMessage);
            }
        }

        private void ShowError(string message)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.ErrorMessageContentControl.Content = message;
                this.ErrorMessageLabel.Visibility = Visibility.Visible;
            });
        }
    }
}
