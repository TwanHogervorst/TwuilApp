using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

        private object writeLock = new object();

        public ChatManager(Client client)
        {
            this.client = client;

            FileInfo userPrivateChatDb = new FileInfo("chatdb/" + this.client.Username + "_private.json");
            if (userPrivateChatDb.Exists)
            {
                try
                {
                    using (StreamReader reader = userPrivateChatDb.OpenText())
                    {
                        this.chatByUsername = JsonConvert.DeserializeObject<Dictionary<string, DChatItem>>(reader.ReadToEnd());
                    }
                }
                catch
                {
                    // jammer dan
                }
            }

            FileInfo userGroupChatDb = new FileInfo("chatdb/" + this.client.Username + "_group.json");
            if (userGroupChatDb.Exists)
            {
                try
                {
                    using (StreamReader reader = userGroupChatDb.OpenText())
                    {
                        this.groupByGroupName = JsonConvert.DeserializeObject<Dictionary<string, DChatItem>>(reader.ReadToEnd());
                    }
                }
                catch
                {
                    // jammer dan
                }
            }

            this.client.OnPrivateMessageReceived += Client_OnPrivateMessageReceived;

            this.client.OnGroupJoin += Client_OnGroupJoin;
            this.client.OnGroupMesssageReceived += Client_OnGroupMesssageReceived;
        }

        private void Client_OnPrivateMessageReceived(Client sender, string messageSender, string message)
        {
            DChatItem chat = null;

            if (!this.chatByUsername.TryGetValue(messageSender, out chat)) this.chatByUsername.Add(messageSender, chat = new DChatItem { ChatName = messageSender, IsGroup = false });

            chat.Messages.Add(new DChatMessage { Sender = messageSender, Message = message });
            this.OnChatUpdate?.Invoke(this, messageSender);

            this.Save();
        }

        private void Client_OnGroupMesssageReceived(Client sender, string messageSender, string groupName, string message)
        {
            DChatItem chat = null;

            if (!this.groupByGroupName.TryGetValue(groupName, out chat)) this.groupByGroupName.Add(groupName, chat = new DChatItem { ChatName = groupName, IsGroup = true });

            chat.Messages.Add(new DChatMessage { Sender = messageSender, Message = message });
            this.OnChatUpdate?.Invoke(this, groupName);

            this.Save();
        }

        private void Client_OnGroupJoin(Client sender, string groupName, List<string> usersInGroup, string welcomeMessage)
        {
            DChatItem chat = null;

            if (!this.groupByGroupName.TryGetValue(groupName, out chat)) this.groupByGroupName.Add(groupName, chat = new DChatItem { ChatName = groupName, IsGroup = true });

            chat.Messages.Add(new DChatMessage { Message = welcomeMessage });
            this.OnChatUpdate?.Invoke(this, groupName);

            this.Save();
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

            chat.Messages.Add(new DChatMessage { Sender = this.client.Username, Message = message });
            this.OnChatUpdate?.Invoke(this, username);

            this.Save();
        }

        public void SendGroupMessage(string groupName, string message)
        {
            this.client.SendGroupMessage(groupName, message);

            DChatItem chat = null;

            if (!this.groupByGroupName.TryGetValue(groupName, out chat)) chat = new DChatItem { ChatName = groupName, IsGroup = true };

            chat.Messages.Add(new DChatMessage { Sender = this.client.Username, Message = message });
            this.OnChatUpdate?.Invoke(this, groupName);

            this.Save();
        }

        public DChatItem CreatePrivateChat(string username)
        {
            DChatItem chat = null;

            if (!this.chatByUsername.ContainsKey(username)) this.chatByUsername.Add(username, chat = new DChatItem { ChatName = username, IsGroup = false });

            this.OnChatUpdate?.Invoke(this, username);

            this.Save();

            return chat;
        }

        private void Save()
        {
            try
            {
                lock(this.writeLock)
                {
                    if (!Directory.Exists("chatdb")) Directory.CreateDirectory("chatdb");

                    using (StreamWriter writer = new StreamWriter(new FileStream("chatdb/" + this.client.Username + "_private.json", FileMode.Create, FileAccess.Write)))
                    {
                        writer.WriteLine(JsonConvert.SerializeObject(this.chatByUsername));
                        writer.Flush();
                    }

                    using (StreamWriter writer = new StreamWriter(new FileStream("chatdb/" + this.client.Username + "_group.json", FileMode.Create, FileAccess.Write)))
                    {
                        writer.WriteLine(JsonConvert.SerializeObject(this.groupByGroupName));
                        writer.Flush();
                    }
                }
            }
            catch
            {
                // jammer dan
            }
        }
    }

    public delegate void ChatManagerChatUpdate(ChatManager sender, string chatName); 
}
