using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace SCADACMC.Class
{
    internal sealed class HSCADAFLAG
	{
		public static void InitializeFlag()
		{
			StringBuilder retstr = new StringBuilder(256);
			string FileName = Application.StartupPath + "\\SCADAGW28.ini";
			int ret = PublicModule.GetPrivateProfileString("DATABASE", "PWD", "", retstr, 256, FileName);
			bool flag = ret != 0;
			if (flag)
			{
				HSCADAFLAG.PWD = Strings.Left(retstr.ToString(), ret);
				HSCADAFLAG.PWD = HSCADAFLAG.Decrypt(ref HSCADAFLAG.PWD);
			}
			ret = PublicModule.GetPrivateProfileString("DATABASE", "UID", "", retstr, 256, FileName);
			bool flag2 = ret != 0;
			if (flag2)
			{
				HSCADAFLAG.UID = Strings.Left(retstr.ToString(), ret);
				HSCADAFLAG.UID = HSCADAFLAG.Decrypt(ref HSCADAFLAG.UID);
			}
			ret = PublicModule.GetPrivateProfileString("DATABASE", "DSN", "", retstr, 256, FileName);
			bool flag3 = ret != 0;
			if (flag3)
			{
				HSCADAFLAG.DSN = Strings.Left(retstr.ToString(), ret);
				HSCADAFLAG.DSN = HSCADAFLAG.Decrypt(ref HSCADAFLAG.DSN);
			}
			ret = PublicModule.GetPrivateProfileString("INTERVAL", "RECONNECT_DB_INTERVAL", "", retstr, 256, FileName);
			bool flag4 = ret != 0;
			checked
			{
				if (flag4)
				{
					HSCADAFLAG.G_RECONNECT_DB_INTERVAL = Conversions.ToInteger(Strings.Left(retstr.ToString(), ret));
					HSCADAFLAG.G_RECONNECT_DB_INTERVAL *= 1000;
				}
				else
				{
					HSCADAFLAG.G_RECONNECT_DB_INTERVAL = 0;
				}
				ret = PublicModule.GetPrivateProfileString("INTERVAL", "GETGW28_STATUS_INTERVAL", "", retstr, 256, FileName);
				bool flag5 = ret != 0;
				if (flag5)
				{
					HSCADAFLAG.G_GETGW28_STATUS_INTERVAL = Conversions.ToInteger(Strings.Left(retstr.ToString(), ret));
					HSCADAFLAG.G_GETGW28_STATUS_INTERVAL *= 1000;
				}
				else
				{
					HSCADAFLAG.G_GETGW28_STATUS_INTERVAL = 0;
				}
				ret = PublicModule.GetPrivateProfileString("INTERVAL", "GETFB_ALARM_INTERVAL", "", retstr, 256, FileName);
				bool flag6 = ret != 0;
				if (flag6)
				{
					HSCADAFLAG.G_GETFB_ALARM_INTERVAL = Conversions.ToInteger(Strings.Left(retstr.ToString(), ret));
					HSCADAFLAG.G_GETFB_ALARM_INTERVAL *= 1000;
				}
				else
				{
					HSCADAFLAG.G_GETFB_ALARM_INTERVAL = 0;
				}
				ret = PublicModule.GetPrivateProfileString("SCADA", "COM_PORT", "", retstr, 256, FileName);
				bool flag7 = ret != 0;
				if (flag7)
				{
				}
				ret = PublicModule.GetPrivateProfileString("SCADA", "APTH", "", retstr, 256, FileName);
				bool flag8 = ret != 0;
				if (flag8)
				{
					PublicModule.Apth = Strings.Left(retstr.ToString(), ret);
					long i = unchecked((long)Strings.InStr(4, PublicModule.Apth, "\\", CompareMethod.Binary));
					bool flag9 = Operators.CompareString(FileSystem.Dir(Strings.Left(PublicModule.Apth, (int)i), FileAttribute.Directory), "", false) == 0;
					if (flag9)
					{
						FileSystem.MkDir(Strings.Left(PublicModule.Apth, (int)i));
					}
					bool flag10 = Operators.CompareString(FileSystem.Dir(PublicModule.Apth, FileAttribute.Directory), "", false) == 0;
					if (flag10)
					{
						FileSystem.MkDir(PublicModule.Apth);
					}
				}
				ret = PublicModule.GetPrivateProfileString("SCADA", "BPTH", "", retstr, 256, FileName);
				bool flag11 = ret != 0;
				if (flag11)
				{
					PublicModule.Bpth = Strings.Left(retstr.ToString(), ret);
					long i = unchecked((long)Strings.InStr(4, PublicModule.Bpth, "\\", CompareMethod.Binary));
					bool flag12 = Operators.CompareString(FileSystem.Dir(Strings.Left(PublicModule.Bpth, (int)i), FileAttribute.Directory), "", false) == 0;
					if (flag12)
					{
						FileSystem.MkDir(Strings.Left(PublicModule.Bpth, (int)i));
					}
					bool flag13 = Operators.CompareString(FileSystem.Dir(PublicModule.Bpth, FileAttribute.Directory), "", false) == 0;
					if (flag13)
					{
						FileSystem.MkDir(PublicModule.Bpth);
					}
				}
				ret = PublicModule.GetPrivateProfileString("INTERVAL", "GET_ALARM_INTERVAL", "", retstr, 256, FileName);
				bool flag14 = ret != 0;
				if (flag14)
				{
					HSCADAFLAG.G_GET_ALARM_INTERVAL = Conversions.ToInteger(Strings.Left(retstr.ToString(), ret));
				}
				else
				{
					HSCADAFLAG.G_GET_ALARM_INTERVAL = 0;
				}
				ret = PublicModule.GetPrivateProfileString("SCADA", "PCNAME", "", retstr, 256, FileName);
				bool flag15 = ret != 0;
				if (flag15)
				{
					HSCADAFLAG.G_PcName = Strings.Left(retstr.ToString(), ret);
				}
				ret = PublicModule.GetPrivateProfileString("SCADA", "USE_ALARM_PRG_ME", "0", retstr, 256, FileName);
				bool flag16 = ret != 0;
				if (flag16)
				{
					HSCADAFLAG.IsUsePrgAlarmME = Conversions.ToBoolean(Strings.Left(retstr.ToString(), ret));
				}
				ret = PublicModule.GetPrivateProfileString("SCADA", "USE_STOP_CMCAUTOSCAN", "0", retstr, 256, FileName);
                bool flag17 = ret != 0;
                if (flag17)
                {
                    HSCADAFLAG.IsUseStopCMCAutoscan = Conversions.ToBoolean(Strings.Left(retstr.ToString(), ret));
                }
                ret = PublicModule.GetPrivateProfileString("DATABASE", "TEST_FLAG", "F", retstr, 256, FileName);
                bool flag18 = ret != 0;
                if (flag18)
                {
                    HSCADAFLAG.IsUseFlagDatabaseTest = Strings.Left(retstr.ToString(), ret);
                }
            }
		}

		private static string Decrypt(ref string strKEY)
		{
			strKEY = Strings.Trim(strKEY);
			string tempKEY = "";
			short i = 1;
			checked
			{
				while ((double)i <= (double)Strings.Len(strKEY) / 3.0)
				{
					tempKEY += Conversions.ToString(Strings.Chr(Conversions.ToInteger(Strings.Right(Strings.Left(strKEY, (int)(i * 3)), 3))));
					i += 1;
				}
				return tempKEY;
			}
		}

		public static string G_PcName;

		public static string[] G_ListPCName;

        public static Dictionary<string, string> G_ListScadaID;


		public static int G_GETGW28_STATUS_INTERVAL;

		public static int G_RECONNECT_DB_INTERVAL;

		public static int G_GETFB_ALARM_INTERVAL;

		public static int G_GET_ALARM_INTERVAL;

		public static string[] G_AlarmMessage = new string[0];

		public static string PWD;

		public static string UID;

		public static string DSN;

		public static bool G_TrSnNotOnline32minsAlarm;

		public static short G_TrSnNotOnlineTime;

		public static bool G_IsStopMachineWhenOverTime;

		public static bool G_StopMachineOvertimeMarialConfirm;

		public static bool G_StopMachineMaterialOfflineOver5mins;

		public static bool G_StopMachineStencil30minsNoOnline;

		public static bool G_StopMachineStencilOver4hNoOffline;


		public static bool G_StopMachineNeedOnlineStencilAfterOffline10Mins;

		public static bool G_StopMachineStencilOffline30minsNoWash;

		public static bool G_StopMachinFlag11;

		public static bool G_StopMachineSolder24hNoOffline;


		public static bool G_StopMachinePCBAMultiPassAutoScanner;

		public static bool G_StopMachineCheckMSDOvertime;

		public static bool G_CheckMaterialRecordStation;

		public static bool G_CMNotConfirmFlag;

		public static short G_CMNotConfirmTime;

		public static bool G_CMaterialconfirmFlag;

		public static short G_CMaterialconfirmTime;

		public static bool G_DeductMaterialFlag;

		public static bool G_PCBA_ERROR_LINE_Flag;

		public static bool G_Material_NotFull_StopMachine_Flag;

        public static List<string> G_AlarmPrgMEIp = new List<string>();

		public static bool IsUsePrgAlarmME;
        public static bool IsUseStopCMCAutoscan;
        public static bool G_IsStart;

        public static string IsUseFlagDatabaseTest { get; private set; }
    }
}
