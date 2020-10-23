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
using TwuilApp.Data;
using TwuilAppClient.Core;

namespace TwuilApp
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {

        private Client client;

        private bool currentOpenIsGroupChat = false;

        public ChatWindow(Client client)
        {
            InitializeComponent();

            this.client = client;

            List<DChatItem> chatItemList = new List<DChatItem>();
            this.ChatItemControl.ItemsSource = chatItemList;

            // responses from server
            this.client.OnServerClosing += Client_OnServerClosing;
            this.client.OnPrivateMessageSendResponse += Client_OnServerResponse;
            this.client.OnGroupCreatedResponse += Client_OnServerResponse;
            this.client.OnGroupMessageSendResponse += Client_OnServerResponse;

            // private message
            this.client.OnPrivateMessageReceived += Client_OnPrivateMessageReceived;

            // groupchats
            this.client.OnGroupJoin += Client_OnGroupJoin;
            this.client.OnGroupMesssageReceived += Client_OnGroupMesssageReceived;
        }

        private void Client_OnServerResponse(Client sender, bool success, string errorMessage)
        {
            if(!string.IsNullOrEmpty(errorMessage))
            {

            }
        }

        private void Client_OnServerClosing(Client sender, string reason)
        {
        }

        private void Client_OnPrivateMessageReceived(Client sender, string messageSender, string message)
        {
        }

        private void Client_OnGroupJoin(Client sender, string groupName, List<string> usersInGroup, string welcomeMessage)
        {
        }

        private void Client_OnGroupMesssageReceived(Client sender, string messageSender, string groupName, string message)
        {
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            this.SendMessage();
        }

        private void ChatTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) this.SendMessage();
        }

        private void SendMessage()
        {
            string message = this.ChatTextBox.Text;
            string receiver = "test"; // todo

            if (message.EndsWith(Environment.NewLine)) message = message.Substring(0, message.Length - Environment.NewLine.Length);

            if (this.currentOpenIsGroupChat) this.client.SendGroupMessage(receiver, message);
            else this.client.SendPrivateMessage(receiver, message);

            this.ChatTextBox.Text = "";
        }
    }
}
