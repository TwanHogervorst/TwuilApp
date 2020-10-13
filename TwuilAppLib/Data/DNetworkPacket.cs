using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwuilAppLib.Data
{
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

    public class DMessagePacket : DAbstract
    {
        public string sender;
        public string receiver;
        public string message;
    }
}
