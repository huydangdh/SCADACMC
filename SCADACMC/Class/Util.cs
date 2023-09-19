using System;
using System.Linq;
using System.Text;

namespace SCADACMC.Class
{
    public class Util
    {
        public static bool ValidateIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;
            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        public sealed class HexIP
        {
            public static short StringToBytes(ref string ip_str, ref byte[] out_bytes)
            {
                out_bytes = Encoding.ASCII.GetBytes(ip_str);
                return (short)out_bytes.Length;
            }
        }
    }
}
