using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwuilAppLib.Data
{

    public struct PacketFlags
    {
        private byte flagByte;

        public PacketFlags(byte flagByte)
        {
            this.flagByte = flagByte;

            // defaults
            this.IsEncrypted = false;
        }

        public bool IsEncrypted
        {
            get => (flagByte & (0x1 << 0)) > 0;
            set
            {
                if (value) flagByte |= (0x1 << 0);
                else flagByte &= 0xFF ^ (0x1 << 0);
            }
        }

        public byte Result() => flagByte;
    }

    public class DNetworkPacket : DAbstract
    {
        public string type;
        [JsonProperty]
        private JObject data;

        public DNetworkPacket<T> DataAsType<T>() where T : DAbstract
        {
            return new DNetworkPacket<T>()
            {
                type = this.type,
                data = data?.ToObject<T>()
            };
        }
    }

    public abstract class DNetworkResponsePacket : DAbstract
    {
        public ResponsePacketStatus status;
        public string errorMessage;
    }

    public class DNetworkPacket<T> : DAbstract where T : DAbstract
    {
        public string type;
        public T data;
    }

    public class DLoginPacket : DAbstract
    {
        public string username;
        public string password;
    }

    public class DSignUpPacket : DAbstract
    {
        public string username;
        public string password;
    }

    public class DLoginResponsePacket : DNetworkResponsePacket
    {
        public string username;
    }

    public class DPrivateMessagePacket : DAbstract
    {
        public string sender;
        public string receiver;
        public string message;
    }

    public class DServerClosingPacket : DAbstract
    {
        public string reason;
    }

    public class DClientDisconnectPacket : DAbstract
    {

    }

    public class DPrivateMessageSendResponse : DNetworkResponsePacket
    {

    }

    public class DGroupCreatePacket : DAbstract
    {
        public string groupName;
        public List<string> usersToAdd = new List<string>();
        public string welcomeMessage;
    }

    public class DGroupCreateResponsePacket : DNetworkResponsePacket
    {

    }

    public class DGroupChatJoinPacket : DAbstract
    {
        public string groupName;
        public List<string> usersInGroup = new List<string>();
        public string welcomeMessage;
    }

    public class DGroupChatMessagePacket : DAbstract
    {
        public string sender;
        public string groupName;
        public string message;
    }

    public class DGroupMessageSendResponsePacket : DNetworkResponsePacket
    {

    }

    public enum ResponsePacketStatus
    {
        Success,
        Error
    }
}
