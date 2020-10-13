using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwuilAppLib.Data
{
    public abstract class DAbstract
    {

        public string ToJson(bool indent = false)
        {
            return JsonConvert.SerializeObject(this, indent ? Formatting.Indented : Formatting.None);
        }

    }
}
