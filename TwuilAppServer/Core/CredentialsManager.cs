using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace TwuilAppServer.Core
{
    public class CredentialsManager
    {
        private Server server;

        private Dictionary<string, string> credentials = new Dictionary<string, string>();

        private object writeLock = new object();

        public CredentialsManager(Server server)
        {
            this.server = server;

            FileInfo credsFile = new FileInfo("creds.json");
            if(credsFile.Exists)
            {
                try
                {
                    using (StreamReader reader = credsFile.OpenText())
                    {
                        this.credentials = JsonConvert.DeserializeObject<Dictionary<string, string>>(reader.ReadToEnd());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception while loading creds.json! {ex.GetType().Name}: {ex.Message}");
                }
            }
            else
            {
                this.Save();
            }
        }

        public (bool, string) Authenticate(string username, string password)
        {
            bool result = true;
            string errorMessage = null;

            if (!this.UserExists(username))
            {
                result = false;
                errorMessage = "Username does not exists.";
            }
            if(result && this.credentials.TryGetValue(username, out string correctPassword) && correctPassword != password)
            {
                result = false;
                errorMessage = "Password is incorrect.";
            }
            if(result && this.server.ServerClientManager.UserIsOnline(username))
            {
                result = false;
                errorMessage = $"The user '{username}' is already logged in.";
            }

            return (result, errorMessage);
        }

        public bool UserExists(string username) => this.credentials.ContainsKey(username);

        public (bool, string) CreateAccount(string username, string password)
        {
            bool result = true;
            string errorMessage = null;

            if (this.UserExists(username))
            {
                result = false;
                errorMessage = $"The user '{username}' already exists";
            }

            if(result)
            {
                this.credentials.Add(username, password);
                if (!this.Save())
                {
                    result = false;
                    errorMessage = "There was an internal server error while creating you're account. Please try again later.";
                }
            }

            return (result, errorMessage);
        }

        private bool Save()
        {
            bool result = false;

            try
            {
                lock(writeLock)
                {
                    using (StreamWriter writer = new StreamWriter(new FileStream("creds.json", FileMode.Create, FileAccess.Write)))
                    {
                        writer.Write(JsonConvert.SerializeObject(this.credentials));
                        writer.Flush();
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An Exception occured while saving creds.json. {ex.GetType().Name}: {ex.Message}");
            }

            return result;
        }
    }
}
