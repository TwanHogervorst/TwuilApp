using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using TwuilAppLib.Data;
using TwuilAppServer.Core;
using TwuilAppServer.Interface;

namespace TwuilAppTests
{
    [TestClass]
    public class CredentialsManagerTest
    {

        [TestInitialize]
        public void Setup()
        {
            FileInfo credsFile = new FileInfo("creds.json");
            if (credsFile.Exists)
            {
                credsFile.Delete();
            }
        }

        [TestMethod]
        public void TestCreateAccount()
        {
            ServerMock server = new ServerMock();
            CredentialsManager credentialsManager = new CredentialsManager(server);

            // Test add account
            (bool, string) createAccountResult = credentialsManager.CreateAccount("test", "unittest");

            Assert.IsTrue(createAccountResult.Item1);
            Assert.IsNull(createAccountResult.Item2);

            FileInfo credsFile = new FileInfo("creds.json");
            Assert.IsTrue(credsFile.Exists);

            Dictionary<string, string> credentials;
            using (StreamReader reader = credsFile.OpenText())
                credentials = JsonConvert.DeserializeObject<Dictionary<string, string>>(reader.ReadToEnd());

            Assert.IsTrue(credentials.ContainsKey("test"));
            Assert.AreEqual("unittest", credentials["test"]);

            // Test add account that already exists
            createAccountResult = credentialsManager.CreateAccount("test", "supersterkwachtwoord");

            Assert.IsFalse(createAccountResult.Item1);
            Assert.AreEqual("The user 'test' already exists", createAccountResult.Item2);
        }

        [TestMethod]
        public void TestUserExists()
        {
            ServerMock server = new ServerMock();
            CredentialsManager credentialsManager = new CredentialsManager(server);

            // add account
            (bool, string) createAccountResult = credentialsManager.CreateAccount("test", "unittest");

            Assert.IsTrue(createAccountResult.Item1);

            // test if it exists
            Assert.IsTrue(credentialsManager.UserExists("test"));

            // test if not existing user returns false
            Assert.IsFalse(credentialsManager.UserExists("test2"));
        }

        [TestMethod]
        public void TestAuthenticate()
        {
            ServerMock server = new ServerMock();
            CredentialsManager credentialsManager = new CredentialsManager(server);

            // add account
            (bool, string) createAccountResult = credentialsManager.CreateAccount("test", "unittest");

            Assert.IsTrue(createAccountResult.Item1);

            // test authenticate (correct user + correct password + user offline)
            (bool, string) authenticateResult = credentialsManager.Authenticate("test", "unittest");

            Assert.IsTrue(authenticateResult.Item1);
            Assert.IsNull(authenticateResult.Item2);

            // test authenticate (wrong user)
            authenticateResult = credentialsManager.Authenticate("test2", "unittest");

            Assert.IsFalse(authenticateResult.Item1);
            Assert.AreEqual("Username does not exists.", authenticateResult.Item2);

            // test authenticate (wrong password)
            authenticateResult = credentialsManager.Authenticate("test", "wrong");

            Assert.IsFalse(authenticateResult.Item1);
            Assert.AreEqual("Password is incorrect.", authenticateResult.Item2);

            // test authenticate (user is already online)
            (server.ServerClientManager as ServerClientManagerMock).MockUserOnline = true;

            authenticateResult = credentialsManager.Authenticate("test", "unittest");

            Assert.IsFalse(authenticateResult.Item1);
            Assert.AreEqual("The user 'test' is already logged in.", authenticateResult.Item2);
        }

        [TestCleanup]
        public void CleanUp()
        {
            FileInfo credsFile = new FileInfo("creds.json");
            if (credsFile.Exists)
            {
                credsFile.Delete();
            }
        }

        private class ServerMock : IServer
        {
            public IServerClientManager ServerClientManager { get; } = new ServerClientManagerMock();

            public ICredentialsManager CredentialsManager => throw new System.NotImplementedException();

            public IChatManager ChatManager => throw new System.NotImplementedException();

            public void SendToClients(List<ServerClient> receiverList, DAbstract data)
            {

            }
        }

        private class ServerClientManagerMock : IServerClientManager
        {

            public bool MockUserOnline { get; set; } = false;

            public List<ServerClient> ClientList => throw new System.NotImplementedException();

            public void Add(ServerClient client)
            {
                throw new System.NotImplementedException();
            }

            public bool Contains(ServerClient client)
            {
                throw new System.NotImplementedException();
            }

            public void Remove(ServerClient client)
            {
                throw new System.NotImplementedException();
            }

            public bool TryGetClientByUsername(string username, out ServerClient client)
            {
                throw new System.NotImplementedException();
            }

            public bool UserIsOnline(string username)
            {
                return this.MockUserOnline;
            }
        }
    }
}
