using Microsoft.VisualBasic.CompilerServices;

namespace SCADACMC.Class.Constant
{
    [StandardModule]
    internal sealed class HCONSTANT
    {
        public static readonly string ORACLE_CONNECTION_STRING_DB_TEST = "Data Source=VNAP.TEST;User Id=SYSTEM;Password=123456;Min Pool Size=2;Max Pool Size=10";

        public static readonly string ORACLE_CONNECTION_STRING_DB_PRODUCTION1 = "Data Source=VNAP.PRODUCTION.CPEI;User Id=AP2;Password=NSDAP2LOGPD0522;Min Pool Size=2";

        public static readonly string ORACLE_CONNECTION_STRING_DB_PRODUCTION2 = "Data Source=VNAP.PRODUCTION.CPEII;User Id=AP2;Password=NSDAP2LOGPD0522;Min Pool Size=2";

        public static bool IS_DEBUG = true;

        public static readonly bool IS_TESTRELAY = false;

        public static string ORACLE_CONNECTION_STRING_DB;
    }
}
