using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace SCADACMC.Class
{
    [StandardModule]
    internal sealed class PublicModule
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        public static void WriteLog(ref string Message)
        {
            string strNow = Strings.Format(DateTime.Now, "yyyy/mm/dd HH:Mm:ss");
            string strFileName = Strings.Format(DateTime.Now, "yyyymmdd");
        }

        public static string ByteArrayToHex(ref byte[] ByteArray)
        {
            long lb = (long)Information.LBound(ByteArray, 1);
            long ub = (long)Information.UBound(ByteArray, 1);
            checked
            {
                long lonRetLen = (ub - lb + 1L) * 3L;
                string strRet = Strings.Space((int)lonRetLen);
                long lonPos = 1L;
                long num = lb;
                long num2 = ub;
                for (long i = num; i <= num2; i += 1L)
                {
                    string strHex = Conversion.Hex(ByteArray[(int)i]);
                    bool flag = Strings.Len(strHex) == 1;
                    if (flag)
                    {
                        strHex = "0" + strHex;
                    }
                    bool flag2 = i != ub;
                    if (flag2)
                    {
                        StringType.MidStmtStr(ref strRet, (int)lonPos, 3, strHex + " ");
                        lonPos += 3L;
                    }
                    else
                    {
                        StringType.MidStmtStr(ref strRet, (int)lonPos, 3, strHex);
                    }
                }
                return strRet;
            }
        }

        public static bool ValidateIPv4(string input)
        {
            bool flag = string.IsNullOrWhiteSpace(input);
            bool ValidateIPv4;
            if (flag)
            {
                ValidateIPv4 = false;
            }
            else
            {
                IPAddress ipTemp = null;
                bool flag2 = IPAddress.TryParse(input, out ipTemp);
                ValidateIPv4 = flag2;
            }
            return ValidateIPv4;
        }

        public static Collection ColSNformat = new Collection();

        public static string Apth;

        public static string Bpth;

        public static bool g_IsComPort;

        public static bool g_IsScannerJugeFlag;
    }
}
