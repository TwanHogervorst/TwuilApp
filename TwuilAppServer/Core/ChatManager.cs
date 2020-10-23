using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using TwuilAppLib.Data;

namespace TwuilAppServer.Core
{
    public class ChatManager
    {
        private Server server;

        private Dictionary<string, List<string>> usersByGroupname = new Dictionary<string, List<string>>();

        private object writeLock = new object();

        public ChatManager(Server server)
        {
            this.server = server;

            FileInfo groupsFile = new FileInfo("groups.json");
            if (groupsFile.Exists)
            {
                try
                {
                    using (StreamReader reader = groupsFile.OpenText())
                    {
                        this.usersByGroupname = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(reader.ReadToEnd());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception while loading groups.json! {ex.GetType().Name}: {ex.Message}");
                }
            }
            else
            {
                this.Save();
            }
        }

        public (bool, string) SendPrivate(string from, string to, string message)
        {
            bool result = false;
            string errorMessage = null;

            try
            {
                DPrivateMessagePacket privateMessagePacket = new DPrivateMessagePacket
                {
                    sender = from,
                    receiver = to,
                    message = message
                };

                if (this.server.ServerClientManager.TryGetClientByUsername(to, out ServerClient clientReceiver))
                {
                    if (clientReceiver.Connected)
                    {
                        clientReceiver.Send(privateMessagePacket);
                        result = true;
                    }
                }
                else if(this.server.CredentialsManager.UserExists(to))
                {
                    errorMessage = "The receiver is not online.";
                }
                else
                {
                    errorMessage = "The receiver does not exist.";
                }
            }
            catch (Exception ex)
            {
                result = false;
                errorMessage = "Internal server error";

                Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            }

            return (result, errorMessage);
        }

        public (bool, string) CreateGroup(string groupName, List<string> users, string welcomeMessage)
        {
            bool result = false;
            string errorMessage = null;

            try
            {
                List<string> existingUserList = users.Where(u => this.server.CredentialsManager.UserExists(u)).ToList();
                List<string> onlineUserList = existingUserList.Where(u => this.server.ServerClientManager.UserIsOnline(u)).ToList();
                List<string> invalidUserList = users.Where(u => !existingUserList.Contains(u)).ToList();

                if (!this.usersByGroupname.ContainsKey(groupName))
                {
                    this.usersByGroupname.Add(groupName, existingUserList);

                    if(this.Save())
                    {
                        result = true;

                        try
                        {
                            DGroupChatJoinPacket groupChatJoinPacket = new DGroupChatJoinPacket
                            {
                                groupName = groupName,
                                usersInGroup = existingUserList,
                                welcomeMessage = welcomeMessage
                            };

                            foreach (string onlineUser in onlineUserList)
                            {
                                if (this.server.ServerClientManager.TryGetClientByUsername(onlineUser, out ServerClient client))
                                {
                                    client.Send(groupChatJoinPacket);
                                }
                            }
                        }
                        catch
                        {
                            // jammer dan
                        }

                        string invalidUsersString = string.Join(", ", invalidUserList);
                        errorMessage = $"Group created but couldn't add the following users: {invalidUsersString}";
                    }
                    else
                    {
                        result = false;
                        errorMessage = "Internal server error";
                    }
                }
                else
                {
                    result = false;
                    errorMessage = $"The group '{groupName}' already exists.";
                }
            }
            catch (Exception ex)
            {
                result = false;
                errorMessage = "Internal server error";

                Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            }

            return (result, errorMessage);
        }

        private bool Save()
        {
            bool result = false;

            try
            {
                lock (writeLock)
                {
                    using (StreamWriter writer = new StreamWriter(new FileStream("groups.json", FileMode.Create, FileAccess.Write)))
                    {
                        writer.Write(JsonConvert.SerializeObject(this.usersByGroupname));
                        writer.Flush();
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An Exception occured while saving groups.json. {ex.GetType().Name}: {ex.Message}");
            }

            return result;
        }

    }
}
