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
        public string LastMessage => this.Messages.LastOrDefault()?.Sender + ": " + this.Messages.LastOrDefault()?.Message;

        public List<DChatMessage> Messages = new List<DChatMessage>();
    }

    public class DChatMessage
    {
        public string Sender { get; set; }
        public string Message { get; set; }
    }
}
