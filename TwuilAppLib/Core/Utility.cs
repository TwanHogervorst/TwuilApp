using System.Collections.Generic;
using System.Linq;

namespace TwuilAppLib.Core
{
    public static class Utility
    {

        public static byte CalculateChecksum(IEnumerable<byte> byteList)
        {
            byte result = 0;

            if (byteList != null) result = byteList.Aggregate(result, (accu, elem) => (byte)(accu ^ elem));

            return result;
        }

    }
}
