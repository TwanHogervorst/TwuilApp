using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwuilApp.Data
{
    public class DChatItem
    {
        public string ChatName { get; set; }
        public bool IsGroup { get; set; }
        public string LastMessage => this.Messages.LastOrDefault()?.sender + ": " + this.Messages.LastOrDefault()?.message;

        public List<DChatMessage> Messages = new List<DChatMessage>();
    }

    public class DChatMessage
    {
        public string sender;
        public string message;
    }
}
