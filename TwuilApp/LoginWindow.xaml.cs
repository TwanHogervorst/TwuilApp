using System;
using System.Collections.Generic;
using System.Net;
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

            bool input = true;
            if (input && string.IsNullOrEmpty(ip))
            {
                input = false;
                this.ShowError("Please enter an ip address!");
            }
            if (input && string.IsNullOrEmpty(username))
            {
                input = false;
                this.ShowError("Please enter an username!");
            }
            if (input && string.IsNullOrEmpty(password))
            {
                input = false;
                this.ShowError("Please enter a password!");
            }
            if(input && !IPAddress.TryParse(ip, out IPAddress dummy))
            {
                input = false;
                this.ShowError("Please enter a valid ip address!");
            }

            try
            {
                if (input)
                {
                    if (this.client != null)
                    {
                        this.client.OnLoginResponseReceived -= this.Client_OnLoginResponseReceived;
                        this.client.OnServerClosing -= this.Client_OnServerClosing;
                        this.client.Dispose();
                    }

                    this.client = new Client(ip, Constants.SERVER_PORT);

                    this.client.OnLoginResponseReceived += Client_OnLoginResponseReceived;
                    this.client.OnServerClosing += Client_OnServerClosing;

                    this.client.Login(username, password);
                }
            }
            catch
            {
                this.ShowError($"Something went wrong while connecting to the server.");
            }
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            string ip = this.ServerIpTextBox.Text;
            string username = this.UsernameTextBox.Text;
            string password = this.PasswordTextBox.Password;

            bool input = true;
            if (input && string.IsNullOrEmpty(ip))
            {
                input = false;
                this.ShowError("Please enter an ip address!");
            }
            if (input && string.IsNullOrEmpty(username))
            {
                input = false;
                this.ShowError("Please enter an username!");
            }
            if (input && string.IsNullOrEmpty(password))
            {
                input = false;
                this.ShowError("Please enter a password!");
            }
            if (input && !IPAddress.TryParse(ip, out IPAddress dummy))
            {
                input = false;
                this.ShowError("Please enter a valid ip address!");
            }

            try
            {
                if (input)
                {
                    if (this.client != null)
                    {
                        this.client.OnLoginResponseReceived -= this.Client_OnLoginResponseReceived;
                        this.client.OnServerClosing -= this.Client_OnServerClosing;
                        this.client.Dispose();
                    }

                    this.client = new Client(ip, Constants.SERVER_PORT);

                    this.client.OnLoginResponseReceived += Client_OnLoginResponseReceived;
                    this.client.OnServerClosing += Client_OnServerClosing;

                    this.client.SignUp(username, password);
                }
            }
            catch
            {
                this.ShowError($"Something went wrong while connecting to the server.");
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
                this.client.OnLoginResponseReceived -= this.Client_OnLoginResponseReceived;
                this.client.OnServerClosing -= this.Client_OnServerClosing;

                this.Dispatcher.Invoke(() =>
                {
                    ChatWindow chatWindow = new ChatWindow(this.client);
                    chatWindow.Show();
                    this.Close();
                });
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
