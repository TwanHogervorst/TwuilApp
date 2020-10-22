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
                try
                {
                    using(StreamWriter writer = new StreamWriter(credsFile.Open(FileMode.Create, FileAccess.Write)))
                    {
                        writer.Write(JsonConvert.SerializeObject(this.credentials));
                        writer.Flush();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An Exception occured while creating creds.json. {ex.GetType().Name}: {ex.Message}");
                }
            }
        }

        public (bool, string) Authenticate(string username, string password)
        {
            bool result = true;
            string errorMessage = null;

            if (!this.credentials.ContainsKey(username))
            {
                result = false;
                errorMessage = "Username does not exists.";
            }
            if(result && this.credentials.TryGetValue(username, out string correctPassword) && correctPassword != password)
            {
                result = false;
                errorMessage = "Password is incorrect.";
            }
            if(result && this.server.ClientManager.Contains(username))
            {
                result = false;
                errorMessage = $"The user '{username}' is already logged in.";
            }

            return (result, errorMessage);
        }
    }
}
