using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using TwuilApp.Data;
using TwuilAppClient.Core;

namespace TwuilApp.Core
{
    public class ChatManager
    {

        public List<DChatItem> ChatItems
        {
            get
            {
                List<DChatItem> result = new List<DChatItem>();

                result.AddRange(this.chatByUsername.Values);
                result.AddRange(this.groupByGroupName.Values);

                return result;
            }
        }

        public List<string> ChatUsernames => new List<string>(this.chatByUsername.Keys);

        public event ChatManagerChatUpdate OnChatUpdate;

        private Client client;

        private Dictionary<string, DChatItem> chatByUsername = new Dictionary<string, DChatItem>();
        private Dictionary<string, DChatItem> groupByGroupName = new Dictionary<string, DChatItem>();

        public ChatManager(Client client)
        {
            this.client = client;

            this.client.OnPrivateMessageReceived += Client_OnPrivateMessageReceived;

            this.client.OnGroupJoin += Client_OnGroupJoin;
            this.client.OnGroupMesssageReceived += Client_OnGroupMesssageReceived;
        }

        private void Client_OnPrivateMessageReceived(Client sender, string messageSender, string message)
        {
            DChatItem chat = null;

            if (!this.chatByUsername.TryGetValue(messageSender, out chat)) this.chatByUsername.Add(messageSender, chat = new DChatItem { ChatName = messageSender, IsGroup = false });

            chat.Messages.Add(new DChatMessage { sender = messageSender, message = message });
            this.OnChatUpdate?.Invoke(this, messageSender);
        }

        private void Client_OnGroupMesssageReceived(Client sender, string messageSender, string groupName, string message)
        {
            DChatItem chat = null;

            if (!this.groupByGroupName.TryGetValue(groupName, out chat)) this.groupByGroupName.Add(groupName, chat = new DChatItem { ChatName = groupName, IsGroup = true });

            chat.Messages.Add(new DChatMessage { sender = messageSender, message = message });
            this.OnChatUpdate?.Invoke(this, groupName);
        }

        private void Client_OnGroupJoin(Client sender, string groupName, List<string> usersInGroup, string welcomeMessage)
        {
            DChatItem chat = null;

            if (!this.groupByGroupName.TryGetValue(groupName, out chat)) this.groupByGroupName.Add(groupName, chat = new DChatItem { ChatName = groupName, IsGroup = true });

            chat.Messages.Add(new DChatMessage { message = welcomeMessage });
            this.OnChatUpdate?.Invoke(this, groupName);
        }

        public DChatItem GetPrivateChat(string username)
        {
            DChatItem result;

            this.chatByUsername.TryGetValue(username, out result);

            return result;
        }

        public DChatItem GetGroupChat(string groupName)
        {
            DChatItem result;

            this.groupByGroupName.TryGetValue(groupName, out result);

            return result;
        }

        public void SendPrivateMessage(string username, string message)
        {
            this.client.SendPrivateMessage(username, message);

            DChatItem chat = null;

            if (!this.chatByUsername.TryGetValue(username, out chat)) chat = new DChatItem { ChatName = username, IsGroup = false };

            chat.Messages.Add(new DChatMessage { sender = this.client.Username, message = message });
            this.OnChatUpdate?.Invoke(this, username);
        }

        public void SendGroupMessage(string groupName, string message)
        {
            this.client.SendGroupMessage(groupName, message);

            DChatItem chat = null;

            if (!this.groupByGroupName.TryGetValue(groupName, out chat)) chat = new DChatItem { ChatName = groupName, IsGroup = true };

            chat.Messages.Add(new DChatMessage { sender = this.client.Username, message = message });
            this.OnChatUpdate?.Invoke(this, groupName);
        }

        public DChatItem CreatePrivateChat(string username)
        {
            DChatItem chat = null;

            if (!this.chatByUsername.ContainsKey(username)) this.chatByUsername.Add(username, chat = new DChatItem { ChatName = username, IsGroup = false });

            this.OnChatUpdate?.Invoke(this, username);

            return chat;
        }
    }

    public delegate void ChatManagerChatUpdate(ChatManager sender, string chatName); 
}
