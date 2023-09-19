using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.IO;
using System.Windows.Forms;

namespace SCADA.Class.Logger
{
    public class SimpleLogger
    {
        private static object lockObj = new object();
        [System.Flags]
        private enum LogLevel
        {
            TRACE,
            INFO,
            DEBUG,
            WARNING,
            ERROR,
            FATAL
        }

        public static void Writeback(string scadaName, string gw28_Name, string strDate, string strMessage)
        {
            lock (lockObj)
            {

                string Mypath = Path.Combine(Application.StartupPath, "Backup");
                //thu muc
                bool flag2 = Operators.CompareString(FileSystem.Dir(Mypath, FileAttribute.Directory), "", false) == 0;
                if (flag2)
                {
                    FileSystem.MkDir(Mypath);
                }
                //ngay
                Mypath = Path.Combine(Mypath, strDate);
                bool flag3 = Operators.CompareString(FileSystem.Dir(Mypath, FileAttribute.Directory), "", false) == 0;
                if (flag3)
                {
                    FileSystem.MkDir(Mypath);
                }
                string Mylog = Path.Combine(Mypath, gw28_Name + ".log");
                bool isappend = true;
                if (!System.IO.File.Exists(Mylog))
                {
                    isappend = false;
                }

                LogPrint(Mylog, isappend, strMessage);

            }
        }

        private static void LogPrint(string logfileName, bool append, string text)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logfileName, append, System.Text.Encoding.UTF8))
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        writer.WriteLine(text);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
