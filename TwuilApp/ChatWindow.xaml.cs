using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TwuilApp.Core;
using TwuilApp.Data;
using TwuilAppClient.Core;

namespace TwuilApp
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {

        private DChatItem _currentOpenChat = null;
        public DChatItem CurrentOpenChat
        {
            get
            {
                return this._currentOpenChat;
            }
            set
            {
                this._currentOpenChat = value;

                if(this._currentOpenChat != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.ChatTitleTextBlock.Text = this._currentOpenChat.ChatName;
                        this.ChatItemControl.SelectedItem = this._currentOpenChat;
                        this.ShowChats();
                    });
                }
            }
        }

        private Client client;
        private ChatManager chatManager;

        public ChatWindow(Client client)
        {
            InitializeComponent();

            this.client = client;

            this.chatManager = new ChatManager(this.client);
            this.chatManager.OnChatUpdate += ChatManager_OnChatUpdate;

            this.ChatItemControl.ItemsSource = this.chatManager.ChatItems;

            // responses from server
            this.client.OnServerClosing += Client_OnServerClosing;
            this.client.OnPrivateMessageSendResponse += Client_OnServerResponse;
            this.client.OnGroupCreatedResponse += Client_OnServerResponse;
            this.client.OnGroupMessageSendResponse += Client_OnServerResponse;
        }

        private void ChatManager_OnChatUpdate(ChatManager sender, string chatName)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.ChatItemControl.ItemsSource = sender.ChatItems;
                if (chatName == this.CurrentOpenChat?.ChatName) this.ShowChats();
            });
        }

        private void Client_OnServerResponse(Client sender, bool success, string errorMessage)
        {
            if(!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Client_OnServerClosing(Client sender, string reason)
        {
            this.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(reason, "Server closed", MessageBoxButton.OK, MessageBoxImage.Error);

                try
                {
                    this.client.Dispose();
                }
                catch
                {

                }

                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();

                this.Close();
            });
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
            if(this.CurrentOpenChat != null)
            {
                string message = this.ChatTextBox.Text;
                string receiver = this.CurrentOpenChat.ChatName;

                if (message.EndsWith(Environment.NewLine)) message = message.Substring(0, message.Length - Environment.NewLine.Length);

                if (this.CurrentOpenChat.IsGroup) this.chatManager.SendGroupMessage(receiver, message);
                else this.chatManager.SendPrivateMessage(receiver, message);

                this.ChatTextBox.Text = "";
            }
        }

        private void AddChatButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewChatWindow newChatDialog = new AddNewChatWindow();
            bool? result = newChatDialog.ShowDialog().HasValue;

            if (newChatDialog.Result)
            {
                string username = newChatDialog.UserTextBox.Text;

                DChatItem chat = this.chatManager.CreatePrivateChat(username);
                this.CurrentOpenChat = chat;
            }
        }

        private void AddGroupChatButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewGroupchatWindow newGroupusersWindow = new AddNewGroupchatWindow(this.chatManager.ChatUsernames);
            newGroupusersWindow.ShowDialog();

            if(newGroupusersWindow.UsernameListView.SelectedItems.Count > 0)
            {
                List<string> usersToAdd = newGroupusersWindow.UsernameListView.SelectedItems
                    .OfType<string>()
                    .ToList();

                if (!usersToAdd.Contains(this.client.Username)) usersToAdd.Add(this.client.Username);

                MakeNewGroupchatWindow newGroupWindow = new MakeNewGroupchatWindow(usersToAdd);
                newGroupWindow.ShowDialog();

                if(newGroupWindow.Result)
                {
                    this.client.CreateGroup(newGroupWindow.GroupNameTextBox.Text, usersToAdd, $"{this.client.Username} created the group {newGroupWindow.GroupNameTextBox.Text}");
                }
            }
        }

        private void ChatItemControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.CurrentOpenChat = (DChatItem)this.ChatItemControl.SelectedItem;
        }

        private void ShowChats()
        {
            this.ChatMessagesItemControl.ItemsSource = null;
            this.ChatMessagesItemControl.ItemsSource = this.CurrentOpenChat.Messages;

            this.MessageScrollViewer.ScrollToBottom();
        }
    }
}
