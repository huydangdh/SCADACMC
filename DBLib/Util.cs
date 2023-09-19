using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * @author V0940534
 * */
namespace DBLib.Util
{
    public static class Util
    {
        public static string HHEncrypt(ref string plainText)
        {
            string _res = string.Empty;
            char[] _arrChar = new char[plainText.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                _arrChar[i] = (char)~(plainText[i] ^ '^');
                _arrChar[i] = (char)(plainText[i] ^ '#');
            }

            var _bytes = System.Text.Encoding.UTF8.GetBytes(_arrChar);
            _res = System.Convert.ToBase64String(_bytes, Base64FormattingOptions.None);

            return _res;
        }

        public static string HHDecrypt(ref string cipherText)
        {
            try
            {
                var _bytes = System.Convert.FromBase64String(cipherText);
                char[] _arrChar = new char[_bytes.Length];

                for (int i = 0; i < _bytes.Length; i++)
                {
                    _arrChar[i] = (char)~(_bytes[i] ^ '^');
                    _arrChar[i] = (char)(_bytes[i] ^ '#');
                }
                cipherText = new string(_arrChar).Trim();
            }
            catch (Exception ex)
            {
                string mzFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string strLogFile = System.IO.Path.Combine(mzFolder, "log_file.log");
                System.IO.File.AppendAllText(
                    strLogFile,
                    string.Format("[E] {0}:{1} - {2}",
                    System.Reflection.MethodBase.GetCurrentMethod(),
                    ex.ToString(), DateTime.Now.ToString()));

                throw new Exception(ex.ToString());
            }

            return cipherText;
        }

    }
}
