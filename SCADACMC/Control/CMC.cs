using System;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Sockets;
using System.Diagnostics;
using SCADACMC.Class;
using static SCADACMC.Class.Util;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic;
using DBLib;
using Oracle.ManagedDataAccess.Client;
using SCADACMC.Class.Constant;
using System.Runtime.CompilerServices;
using System.IO;
using System.Reflection;
using SCADA.Class.Logger;
using Treorisoft.Net;

namespace SCADACMC.Control
{
    public enum MyStateSocket
    {
        sckClosed = 0,
        sckOpen = 1,
        sckListening = 2,
        sckConnectionPending = 3,
        sckResolvingHost = 4,
        sckHostResolved = 5,
        sckConnecting = 6,
        sckConnected = 7,
        sckClosing = 8,
        sckError = 9
    }

    public partial class CMCControl : UserControl
    {
        public OSWINSCK.Winsock Socket;

        public MyStateSocket State { get; private set; }

        public string IpAddress { get; set; }
        public int PortNumber { get; set; }
        public int ID { get; set; }

        public string ScadaId;

        public string ScadaName;

        public string StationNo;

        public string LineName;

        public string StationName;

        public string PointCode;

        public string IP;

        public string StoredProcedure;

        public string LanguageFlag;

        public bool IsConnected;

        public bool IsStart;

        public bool IsComPort;

        public long BeginExecute;

        public long EndExecute;

        public bool IsBusy;

        public short FailTimes;

        public Stopwatch Stopwatch;

        public bool isPending;

        private readonly object _objLock = new object();

        //****//

        public bool Received;


        public CMCControl()
        {
            InitializeComponent();
            Socket = new OSWINSCK.Winsock();
            Socket.CloseWinsock();
            Socket.OnConnect += Socket2_OnConnect;
            Socket.OnDataArrival += Socket2_OnDataArrival;
            Socket.OnClose += Socket2_OnClose;
            Socket.OnError += Socket2_OnError;
        }

        private void Socket2_OnError(short Number, ref string Description, int Scode, string Source, string HelpFile, int HelpContext, ref bool CancelDisplay)
        {
            if (Socket.State == OSWINSCK.StateConstants.sckClosing)
                Socket.CloseWinsock();

            this.NamePanel.ForeColor = Color.Black;
            this.NamePanel.BackColor = Color.FromArgb(255, 61, 0);
            this.State = MyStateSocket.sckClosed;
            this.BeginExecute = 0;
            this.IsConnected = false;
        }

        private void Socket2_OnClose()
        {
            this.NamePanel.ForeColor = Color.Black;
            this.NamePanel.BackColor = Color.FromArgb(255, 61, 0);
            this.State = MyStateSocket.sckClosed;
            this.BeginExecute = 0;
            this.IsConnected = false;
        }

        private void Socket2_OnDataArrival(int bytesTotal)
        {
            this.NamePanel.ForeColor = Color.Black;
            this.NamePanel.BackColor = Color.FromArgb(246, 229, 141);
            this.State = MyStateSocket.sckConnected;
            CMCControlMessage msg = new CMCControlMessage();
            msg.Index = this.ID;
            msg.Name = this.StationName;
            object buffer = new object();
            Socket.GetData(ref buffer, 8192 + 17);
            msg.Data = (byte[])buffer;
            this.BeginExecute = Environment.TickCount;
            MainForm.ConQueue.Enqueue(msg);
            this.NamePanel.ForeColor = Color.Black;
            this.NamePanel.BackColor = Color.FromArgb(118, 255, 3);
            this.State = MyStateSocket.sckConnected;
        }

        private void Socket2_OnConnect()
        {
            this.NamePanel.ForeColor = Color.Black;
            this.NamePanel.BackColor = Color.FromArgb(118, 255, 3);
            this.State = MyStateSocket.sckConnected;

            IsConnected = true;
            this.isPending = false;
            this.FailTimes = 0;
            this.BeginExecute = Environment.TickCount;
            string nextInput = this.ShowNextInput();
            // AUTO ALIVE 
            //byte[] autolive = { 0x1b, 0x11, 0x03 };
            //args.BaseSocket.Send(autolive);
            //this.Socket.SendData(autolive);
            this.ShowOnConnectedData(0, nextInput, "OnConnected", 0);
        }

        public void Connect()
        {
            if (string.IsNullOrEmpty(IP) || PortNumber <= 0)
                throw new Exception(string.Format("Thiếu thông tin IP vs Port"));

            if (Socket != null && Socket.State == OSWINSCK.StateConstants.sckConnected)
            {
                Socket.CloseWinsock();
                this.NamePanel.ForeColor = Color.Black;
                this.NamePanel.BackColor = Color.FromArgb(255, 61, 0);
                this.State = MyStateSocket.sckClosed;
                this.IsConnected = false;
            }

            NamePanel.ForeColor = Color.Black;
            NamePanel.BackColor = Color.FromArgb(255, 234, 0);
            this.State = MyStateSocket.sckConnecting;
            Socket.Connect(this.IP, this.PortNumber);
        }

        /// <summary>
        /// SEND DATA
        /// </summary>
        /// <param name="index"></param>
        /// <param name="GWip"></param>
        /// <param name="Result"></param>
        /// <param name="AlermMess"></param>
        /// <param name="status"></param>
        public void SendData(int index, string GWip, string Result, string AlermMess, short status)
        {
            lock (_objLock)
            {
                byte[] sendBuffer = new byte[1024];
                bool flag = Result.Contains("OK");
                if (flag || AlermMess.ToUpper() == "ONCONNECTED" || AlermMess.ToUpper() == "UNDO")
                {
                    string text = "\u001b\f\u0002\u0001\u0001" + Result + "\r\n";

                    short cnt = HexIP.StringToBytes(ref text, ref sendBuffer);
                }
                else
                {
                    string text = "\u001b\f\u0002\u0001\u0002\r\n" + Result + "\r\n";

                    short cnt = HexIP.StringToBytes(ref text, ref sendBuffer);
                }

                //Socket.SendData(Encoding.ASCII.GetBytes(Result + "\r\n"));
                Socket.SendData(sendBuffer);

            }
        }


        /// <summary>
        /// Send Alarm 
        /// </summary>
        /// <param name="index">Index GW28</param>
        /// <param name="GWip">IP GW28</param>
        /// <param name="Result">Cảnh báo 1</param>
        /// <param name="AlermMess">cảnh báo 2</param>
        /// <param name="status">trạng thái</param>
        // Token: 0x060001E1 RID: 481 RVA: 0x00013000 File Offset: 0x00011200
        public void SendAlarm(int index, string GWip, string Result, string AlermMess, short status)
        {
            lock (_objLock)
            {
                byte[] sndbuf = new byte[1025];
                bool flag = Operators.CompareString(Result, "", false) != 0;
                short cnt;
                if (flag)
                {
                    string text = "\u001b\f\u0002\u0001\u0006" + Result + "\r\n";

                    cnt = HexIP.StringToBytes(ref text, ref sndbuf);
                }
                else
                {
                    string text = "\u001b\f\u0002\u0001\u0006";

                    cnt = HexIP.StringToBytes(ref text, ref sndbuf);
                }
                Socket.SendData(sndbuf);
                //Socket.SendData(Encoding.ASCII.GetBytes(Result + "\r\n"));
            }
        }

        /// <summary>
        /// Send Alarm 1
        /// </summary>
        /// <param name="index">Index GW28</param>
        /// <param name="GWip">IP GW28</param>
        /// <param name="Result">Cảnh báo 1</param>
        /// <param name="AlermMess">cảnh báo 2</param>
        /// <param name="status">trạng thái</param>
        // Token: 0x060001E2 RID: 482 RVA: 0x00013078 File Offset: 0x00011278
        public void SendAlarm1(int index, string GWip, string Result, string AlermMess, short status)
        {
            lock (_objLock)
            {
                byte[] sndbuf = new byte[1025];
                bool flag = Operators.CompareString(Strings.Mid(GWip, 1, 3), "B31", false) == 0;
                if (flag)
                {
                    bool flag2 = Operators.CompareString(Result, "", false) != 0;
                    short cnt;
                    if (flag2)
                    {
                        string text = "\u001b\f\u0002\u0001\u0006\u001b\t\u0001" + Result + "\r\n";

                        cnt = HexIP.StringToBytes(ref text, ref sndbuf);
                    }
                    else
                    {
                        string text = "\u001b\f\u0002\u0001\u0006\u001b\t\u0001\r\n";

                        cnt = HexIP.StringToBytes(ref text, ref sndbuf);
                    }
                    Socket.SendData(sndbuf);
                    //Socket.SendData(Encoding.ASCII.GetBytes(Result + "\r\n"));
                }
                else
                {
                    bool flag3 = Operators.CompareString(Result, "", false) != 0;
                    short cnt;
                    if (flag3)
                    {
                        string text = "\u001b\f\u0002\u0001\u0006\u001b\t\u0001" + Result + "\r\n";

                        cnt = HexIP.StringToBytes(ref text, ref sndbuf);
                    }
                    else
                    {
                        string text = "\u001b\f\u0002\u0001\u0006\u001b\t\u0001\r\n";

                        cnt = HexIP.StringToBytes(ref text, ref sndbuf);
                    }
                    Socket.SendData(sndbuf);
                    //Socket.SendData(Encoding.ASCII.GetBytes(Result + "\r\n"));
                }
            }
        }

        /// <summary>
        /// Send Alarm 2
        /// </summary>
        /// <param name="index">Index GW28</param>
        /// <param name="GWip">IP GW28</param>
        /// <param name="Result">Cảnh báo 1</param>
        /// <param name="AlermMess">cảnh báo 2</param>
        /// <param name="status">trạng thái</param>
        // Token: 0x060001E3 RID: 483 RVA: 0x00013178 File Offset: 0x00011378
        public void SendAlarm2(short index, string GWip, string Result, string AlermMess, short status, ClientSocket insocket = null)
        {
            lock (_objLock)
            {
                byte[] sndbuf = new byte[1025];
                bool flag = Operators.CompareString(Strings.Mid(GWip, 1, 3), "B31", false) == 0;
                if (flag)
                {
                    bool flag2 = Operators.CompareString(Result, "", false) != 0;
                    short cnt;
                    if (flag2)
                    {
                        string text = "\u001b\f\u0002\u0001\u0006\u001b\t\u0001" + Result + "\r\n";

                        cnt = HexIP.StringToBytes(ref text, ref sndbuf);
                    }
                    else
                    {
                        string text = "\u001b\f\u0002\u0001\u0006\u001b\t\u0001\r\n";

                        cnt = HexIP.StringToBytes(ref text, ref sndbuf);
                    }
                    if (insocket != null)
                    {
                        insocket.Send(sndbuf);
                    }
                    else
                    {
                        Socket.SendData(sndbuf);

                    }
                }
                else
                {
                    bool flag3 = Operators.CompareString(Result, "", false) != 0;
                    short cnt;
                    if (flag3)
                    {
                        string text = "\u001b\t\0" + Result + "\r\n";

                        cnt = HexIP.StringToBytes(ref text, ref sndbuf);
                    }
                    else
                    {
                        string text = "\u001b\t\0";

                        cnt = HexIP.StringToBytes(ref text, ref sndbuf);
                    }
                    if (insocket != null)
                    {
                        insocket.Send(sndbuf);
                    }
                    else
                    {
                        Socket.SendData(sndbuf);

                    }
                }

            }
        }

        /// <summary>
        /// Send Alarm 3
        /// </summary>
        /// <param name="index">Index GW28</param>
        /// <param name="GWip">IP GW28</param>
        /// <param name="Result">Cảnh báo 1</param>
        /// <param name="AlermMess">cảnh báo 2</param>
        /// <param name="status">trạng thái</param>
        // Token: 0x060001E4 RID: 484 RVA: 0x0001327C File Offset: 0x0001147C
        public void SendAlarm3(int index, string GWip, string Result, string AlermMess, int status)
        {
            lock (_objLock)
            {
                byte[] l_sendBuffer = new byte[1025];
                bool flag = Operators.CompareString(Strings.Mid(GWip, 1, 3), "B31", false) == 0;
                if (flag)
                {
                    bool flag2 = Operators.CompareString(Result, "", false) != 0;
                    int cnt;
                    if (flag2)
                    {
                        string text = "\u001b\f\u0002\u0001\u0006\u001b\t\u0001" + Result + "\r\n";

                        cnt = (int)HexIP.StringToBytes(ref text, ref l_sendBuffer);
                    }
                    else
                    {
                        string text = "\u001b\f\u0002\u0001\u0006\u001b\t\u0001\r\n";

                        cnt = (int)HexIP.StringToBytes(ref text, ref l_sendBuffer);
                    }
                    Socket.SendData(l_sendBuffer);

                    //Socket.SendData(Encoding.ASCII.GetBytes(Result + "\r\n"));
                }
                else
                {
                    if (Result.Contains("OK"))
                    {
                        //UNLock CMCAUTOSCAN
                        byte[] byteUnlock = { 0x1b, 0x09, 0x00, 0x0d };
                        string text;
                        if (HSCADAFLAG.IsUseStopCMCAutoscan)
                        {
                            text = Encoding.ASCII.GetString(byteUnlock) + "\u001b\f\u0002\u0001\u0001" + Result + "\r\n";
                        }
                        else
                        {
                            text = "\u001b\f\u0002\u0001\u0001" + Result + "\r\n";
                        }


                        HexIP.StringToBytes(ref text, ref l_sendBuffer);
                    }
                    else
                    {
                        //Lock CMCAUTOSCAN
                        byte[] byteLock = { 0x1b, 0x0C, 0x02, 0x01, 0x06, 0x1b, 0x09, 0x01 };
                        string text;
                        if (HSCADAFLAG.IsUseStopCMCAutoscan)
                        {
                            text = Encoding.ASCII.GetString(byteLock) + "\u001b\f\u0002\u0001\u0002" + Result + "\r\n";
                        }
                        else
                        {
                            text = "\u001b\f\u0002\u0001\u0002" + Result + "\r\n";
                        }

                        HexIP.StringToBytes(ref text, ref l_sendBuffer);
                    }

                    //if (HCONSTANT.IS_TESTRELAY)
                    //{
                    //    MainForm.cmcControl.Send(l_sendBuffer);
                    //}
                    //Socket.SendData(Encoding.ASCII.GetBytes(Result + "\r\n"));
                    Socket.SendData(l_sendBuffer);
                }
            }
        }

        public string ShowNextInput()
        {
            string next_input = string.Empty;
            string strSql = "select max(to_number(data3)) ma  from mes4.r_ap_temp where data1='SCADA-GW28' and data2='" + this.StationNo + "'";
            DataTable tmpDataTable = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strSql, new OracleParameter[0]);
            bool flag = tmpDataTable.Rows.Count > 0;
            if (flag)
            {
                bool flag2 = Information.IsDBNull(RuntimeHelpers.GetObjectValue(tmpDataTable.Rows[0]["MA"]));
                if (flag2)
                {
                    next_input = "/EMP ?";
                }
                else
                {
                    strSql = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("select data6 from mes4.r_ap_temp where data1='SCADA-GW28' and data2='" + this.StationNo + "' and data3='", tmpDataTable.Rows[0]["MA"]), "'"));
                    tmpDataTable = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strSql, new OracleParameter[0]);
                    bool flag3 = tmpDataTable.Rows.Count > 0;
                    if (flag3)
                    {
                        next_input = Conversions.ToString(Operators.AddObject(Operators.AddObject("/", tmpDataTable.Rows[0]["data6"]), " ?"));
                    }
                }
            }
            else
            {
                next_input = "/EMP ?";
            }
            return next_input;
        }

        public void ShowOnConnectedData(int index, string data, string AlarmMess, short status)
        {
            SendData(index, this.IP, data, AlarmMess, status);
        }

        /// <summary>
        /// Convert Message
        /// </summary>
        /// <param name="msg">Msg Input</param>
        /// <param name="gw28index">GW28 index</param>
        /// <returns></returns>
        // Token: 0x060001DF RID: 479 RVA: 0x00012D08 File Offset: 0x00010F08
        private string Message_convert(string msg, int gw28index, object a)
        {
            checked
            {
                string message_convert = string.Empty;
                bool flag = Operators.CompareString(LanguageFlag, "1", false) == 0;
                if (flag)
                {
                    bool flag2 = Strings.InStr(1, msg, "/", CompareMethod.Binary) > 1;
                    if (flag2)
                    {
                        string[] tempstr = Strings.Split(msg, "/", 2, CompareMethod.Binary);
                        int i = 0;
                        do
                        {
                            string strsql = "select LOCAL_LANGUAGE from mes1.C_SCADA_MESSAGE where ENGLISH='" + tempstr[i] + "'";
                            DataTable l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql, new OracleParameter[0]);
                            bool flag4 = l_dtTemp.Rows.Count > 0;
                            if (flag4)
                            {
                                bool flag5 = i == 0;
                                if (flag5)
                                {
                                    message_convert = Strings.Trim(Conversions.ToString(l_dtTemp.Rows[0]["LOCAL_LANGUAGE"]));
                                }
                                else
                                {
                                    message_convert = Strings.Trim(message_convert) + "/" + Strings.Trim(Conversions.ToString(l_dtTemp.Rows[0]["LOCAL_LANGUAGE"]));
                                }
                            }
                            else
                            {
                                bool flag6 = i == 0;
                                if (flag6)
                                {
                                    message_convert = Strings.Trim(tempstr[i]);
                                }
                                else
                                {
                                    message_convert = Strings.Trim(message_convert) + "/" + Strings.Trim(tempstr[i]);
                                }
                            }
                            i++;
                        }
                        while (i <= 1);
                    }
                    else
                    {
                        string strsql = "select LOCAL_LANGUAGE from mes1.C_SCADA_MESSAGE where ENGLISH='" + msg + "'";
                        DataTable l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql, new OracleParameter[0]);
                        bool flag7 = l_dtTemp.Rows.Count > 0;
                        if (flag7)
                        {
                            message_convert = Conversions.ToString(l_dtTemp.Rows[0]["LOCAL_LANGUAGE"]);
                        }
                        else
                        {
                            message_convert = msg;
                        }
                    }
                }
                else
                {
                    message_convert = msg;
                }
                return message_convert;
            }
        }

        /// <summary>
        /// Undo GW28
        /// </summary>
        /// <param name="index">Index GW28</param>
        // Token: 0x060001E8 RID: 488 RVA: 0x00013620 File Offset: 0x00011820
        public void DoUndo(int index)
        {
            StringBuilder Sql = new StringBuilder();
            Sql.Append(" SELECT DISTINCT m.tr_sn, CEIL ((SYSDATE - m.work_time) * 24 * 60) diff_time, ");
            Sql.Append("        n.station_name , n.ip ");
            Sql.Append("   FROM mes4.r_tr_sn_wip m, ");
            Sql.Append("        mes1.c_gw28_config n, ");
            Sql.Append("        mes4.r_station_wip l, ");
            Sql.Append("        mes1.c_smt_ap_list k ");
            Sql.Append("  WHERE m.work_flag = '0'  ");
            Sql.Append("    AND m.station_flag = '1' ");
            Sql.Append("    AND (SYSDATE - m.work_time) * 24 * 60 > " + HSCADAFLAG.G_TrSnNotOnlineTime.ToString() + " ");
            Sql.Append("    AND SUBSTR (m.station, 6, 2) <> 'CT' ");
            Sql.Append("    AND SUBSTR (m.emp_no, 0, 1) <> '+' ");
            Sql.Append("    AND m.station = n.station_name ");
            Sql.Append("    AND m.wo = l.wo ");
            Sql.Append("    AND m.station = l.station ");
            Sql.Append("    AND l.tr_sn is not null ");
            Sql.Append("    AND l.smt_code = k.smt_code ");
            Sql.Append("    AND m.kp_no IN k.kp_no ");
            Sql.Append("    AND l.smt_code IS NOT NULL ");
            Sql.Append("    and NOT EXISTS (SELECT 1 FROM mes4.r_station_wip WHERE L.FEEDER_TYPE=FEEDER_TYPE AND FEEDER_TYPE='TRAY') ");

            DataTable l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, Sql.ToString(), new OracleParameter[0]);
            if (l_dtTemp.Rows.Count == 0)
            {
                var info = "UNDO";
                //int indexA = -1; int indexB = -1;
                //ScadaModel l_scadaModel = ScadaManger.Instance.FindScadaByIdOrName(this.ScadaId);
                //l_scadaModel.getG3Ex(this.StationName, ref indexA, ref indexB);
                //if (indexA != -1 && l_scadaModel.m_stationModel.Gw28Model[indexA].IsConnected)
                //{
                //    SendAlarm2(0, l_scadaModel.m_stationModel.Gw28Model[indexA].IP, "", "", 0, l_scadaModel.m_stationModel.Gw28Model[indexA].Socket);
                //}
                //if (indexB != -1 && l_scadaModel.m_stationModel.Gw28Model[indexB].IsConnected)
                //{
                //    SendAlarm2(0, l_scadaModel.m_stationModel.Gw28Model[indexB].IP, "", "", 0, l_scadaModel.m_stationModel.Gw28Model[indexB].Socket);
                //}
            }

            Sql.Clear();
            Sql.Append("DELETE FROM MES4.R_AP_TEMP WHERE DATA1='SCADA-GW28' AND DATA2='" + this.StationNo + "'");
            try
            {
                OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, Sql.ToString(), null);
                this.SendData((short)index, this.IP, ShowNextInput(), "UNDO", 0);

            }
            catch (Exception ex)
            {
                string mzFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string strLogFile = Path.Combine(mzFolder, "log_file.log");
                string mzLogText = string.Format("ID={0},SCADAID={1},IP={2},STATIONNAME={3},STATIONNO={4},STATE={5}",
                                                 this.ID, this.ScadaId, this.IP, this.StationName, this.StationNo, this.State);
                File.AppendAllText(strLogFile, string.Format("[E] {0}:{1} - {2} {3}", new object[]
                {
                                            MethodBase.GetCurrentMethod(),
                                            ex.ToString() + "\n" + mzLogText,
                                            DateTime.Now.ToString(),
                                            Environment.NewLine
                }));
                throw;
            }
        }

        public string CheckIsCOMPort(byte[] aa, long lLen, ref bool isComportFlag)
        {
            string sbuf = string.Empty;
            bool flag = lLen <= 0L;
            string GetCOMPortFlag = string.Empty;
            if (flag)
            {
                GetCOMPortFlag = "";
            }
            else
            {
                int num = (int)lLen;
                bool comPortFlag = GetComPortFlag(aa, num);

                if (this.StationName.Contains("AW"))
                    comPortFlag = true;

                lLen = unchecked((long)num);
                bool flag2 = !comPortFlag;
                if (flag2)
                {
                    isComportFlag = false;
                    long num2 = lLen - 1L;
                    for (long index = 0L; index <= num2; index += 1L)
                    {
                        bool flag3 = aa[(int)index] < 0 | aa[(int)index] > 31;
                        if (flag3)
                        {
                            sbuf += Conversions.ToString(Strings.Chr((int)aa[(int)index]));
                        }
                    }
                }
                else
                {
                    isComportFlag = true;
                    bool flag4 = aa[1] == 40;
                    if (flag4)
                    {
                        return GetCOMPortFlag;
                    }
                    long num3 = lLen - 1L;
                    for (long i = 0L; i <= num3; i += 1L)
                    {
                        bool flag5 = aa[(int)i] < 0 | aa[(int)i] > 31;
                        if (flag5)
                        {
                            sbuf += Conversions.ToString(Strings.Chr((int)aa[(int)i]));
                        }
                    }
                }
                GetCOMPortFlag = sbuf;
            }
            return GetCOMPortFlag;

        }

        private bool GetComPortFlag(byte[] aa, int ilen)
        {
            bool _result_getcomflag = false;
            short i = 0;
            if (ilen > 3)
            {
                while ((int)i <= ilen - 1)
                {

                    bool flag = aa[(int)i] == 27;
                    if (flag)
                    {
                        return true;
                    }
                    i += 1;
                }
            }
            return _result_getcomflag;

        }

        public string OnProcessDataArrival(string inData, bool bIsComPort)
        {
            string sReturnMessage = string.Empty;
            if (!string.IsNullOrEmpty(inData))
            {
                string strSQL = "SELECT * From mes1.c_program_parameter  WHERE program_name = 'SCADA'  AND FUNCTION_NAME ='JUDGE_HH_LABEL_SN_IN_AUTOSCANNER' AND ROWNUM=1   AND FUNCTION_VALUE1 ='Y'  ";
                DataTable l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strSQL, new OracleParameter[0]);
                if (l_dtTemp.Rows.Count > 0) PublicModule.g_IsScannerJugeFlag = true;
                else PublicModule.g_IsScannerJugeFlag = false;

                if (bIsComPort)
                {
                    bool flag3 = Strings.InStr(1, inData, "@", CompareMethod.Binary) > 0;
                    if (inData.Contains("@"))
                    {
                        string[] arrTempSN = inData.Split('@');
                        for (int index = 0; index <= arrTempSN.Length; index++)
                        {
                            if (arrTempSN[index] != "NOREAD")
                            {
                                string outData = FillSmtMaterialNew(0, arrTempSN[index], this.LineName, this.StationName);
                                sReturnMessage = outData;
                                if (!outData.Contains("OK"))
                                {
                                    outData = DateTime.Now.ToString("yyyyMMdd");
                                    string text2 = string.Concat(new string[]
                                    {
                                            Conversions.ToString(DateAndTime.Now),
                                            "||",
                                            arrTempSN[index],
                                            "||",
                                            outData
                                    });
                                    SimpleLogger.Writeback(this.ScadaName, this.StationName, outData, text2);
                                    this.SendAlarm3(this.ID, this.IP, inData + "||" + outData, "", 0);
                                }
                                else
                                {
                                    string text2 = DateTime.Now.ToString("yyyyMMdd");
                                    outData = Conversions.ToString(DateAndTime.Now) + "||" + arrTempSN[index] + "||OK";
                                    SimpleLogger.Writeback(this.ScadaName, this.StationName, text2, outData);
                                    this.SendAlarm3(this.ID, this.IP, inData + "||OK", "", 0);
                                }
                                if (this.StationName.Contains("AP1"))
                                {
                                    bool g_DeductMaterialFlag = HSCADAFLAG.G_DeductMaterialFlag;
                                    //if (g_DeductMaterialFlag)
                                    //{
                                    //    //If CheckPsnPassScanner(strdata) = "NG" Then
                                    //    //   strTemp1 = DeductSmtMaterial(gw28_list(index).station_name, strdata)
                                    //    //    If strTemp1<> "OK" Then
                                    //    //       Writeback gw28_list(index).station_name, Format(Of Date, "yyyymmdd"), Now & "||" & strdata + " Deduct Material: ||" + strTemp1
                                    //    //        'Luongvd007 20140804
                                    //    //        myid = getG3(gw28_list(index).ip)
                                    //    //        SendAlarm3 myid, gw28_list(index).station_name, strPsn(i) + " Deduct Material: ||" + strTemp1, "", 0
                                    //    //    Else
                                    //    //        Writeback gw28_list(index).station_name, Format(Of Date, "yyyymmdd"), Now & "||" & strdata + "||Deduct Material OK"
                                    //    //        'Luongvd007 20140804
                                    //    //        myid = getG3(gw28_list(index).ip)
                                    //    //        SendAlarm3 myid, gw28_list(index).station_name, strPsn(i) + "||Deduct Material OK", "", 0
                                    //    //    End If
                                    //    //End If
                                    //}
                                }
                            }
                        }
                        strSQL = "SELECT * From mes1.c_program_parameter  WHERE program_name = 'SCADA' AND function_name = 'NoRecordDoubleLink'    AND function_object = 'NoRecordDoubleLink'    AND function_value1 = 'Y'    AND data1 = 'NORMAL' and rownum=1";
                        l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strSQL, new OracleParameter[0]);
                        bool flag7 = l_dtTemp.Rows.Count == 0;
                        if (flag7)
                        {
                            //TODO :  strTemp = RecordDoubleLink(strdata, strdata, gw28_list(index).station_name)

                        }
                    }
                    else
                    {
                        string outdata1 = FillSmtMaterialNew(0, inData, this.LineName, this.StationName);
                        sReturnMessage = outdata1;
                        if (!outdata1.Contains("OK"))
                        {
                            string stationName3 = this.StationName;
                            string text2 = string.Concat(new string[]
                            {
                                    Conversions.ToString(DateAndTime.Now),
                                    "||",
                                    inData,
                                    "||",
                                    outdata1
                            });
                            SimpleLogger.Writeback(this.ScadaName, stationName3, DateTime.Now.ToString("yyyyMMdd"), text2);
                            this.SendAlarm3(this.ID, this.IP, inData + "||" + outdata1, "", 0);
                        }
                        else
                        {
                            if (inData.Length != 7)
                            {
                                var outdata2 = string.Concat(new string[]
                                {
                                        Conversions.ToString(DateAndTime.Now),
                                        "||",
                                        inData,
                                        "||",
                                        outdata1
                                });
                                SimpleLogger.Writeback(this.ScadaName, this.StationName, DateTime.Now.ToString("yyyyMMdd"), outdata2);
                                this.SendAlarm3(this.ID, this.IP, inData + "||OK", "", 0);
                            }
                            else
                            {
                                bool g_IsScannerJugeFlag = PublicModule.g_IsScannerJugeFlag;
                                if (!g_IsScannerJugeFlag)
                                {
                                    string text2 = Conversions.ToString(DateAndTime.Now) + "||" + inData + "||OK";
                                    SimpleLogger.Writeback(this.ScadaName, this.StationName, DateTime.Now.ToString("yyyyMMdd"), text2);
                                    this.SendAlarm3(this.ID, this.IP, inData + "||OK_HH_LABEL", "", 0);
                                }
                            }
                        }
                        if (this.StationName.Contains("AP1"))
                        {
                            bool g_DeductMaterialFlag2 = HSCADAFLAG.G_DeductMaterialFlag;
                            if (g_DeductMaterialFlag2)
                            {
                            }
                        }
                        strSQL = "SELECT * From mes1.c_program_parameter  WHERE program_name = 'SCADA' AND function_name = 'NoRecordDoubleLink'    AND function_object = 'NoRecordDoubleLink'    AND function_value1 = 'Y'    AND data1 = 'NORMAL' and rownum=1";
                        l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strSQL, new OracleParameter[0]);
                        bool flag11 = l_dtTemp.Rows.Count == 0;
                        if (flag11)
                        {
                        }
                    }
                }
                else
                {
                    if (inData == "UNDO")
                    {
                        DoUndo(0);
                    }
                    else
                    {
                        strSQL = "select *  from mes4.r_ap_temp where data1='SCADA-GW28' and data2='" + this.StationNo + "' and data3='1'";
                        l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strSQL, new OracleParameter[0]);
                        bool flag13 = l_dtTemp.Rows.Count > 0;
                        if (flag13)
                        {
                            OracleParameter[] arrOraParams = new OracleParameter[3];
                            arrOraParams[0] = new OracleParameter("mydata", inData);
                            arrOraParams[0].Direction = ParameterDirection.Input;
                            arrOraParams[1] = new OracleParameter("g_stationno", this.StationNo);
                            arrOraParams[1].Direction = ParameterDirection.Input;
                            arrOraParams[2] = new OracleParameter();
                            arrOraParams[2].ParameterName = "res";
                            arrOraParams[2].Size = 1024;
                            arrOraParams[2].Direction = ParameterDirection.Output;
                            using (OracleConnection l_conn = new OracleConnection(HCONSTANT.ORACLE_CONNECTION_STRING_DB))
                            {
                                try
                                {
                                    l_conn.Open();
                                    OracleTransaction trans = l_conn.BeginTransaction();
                                    OracleHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "MES1." + this.StoredProcedure, arrOraParams);
                                    sReturnMessage = Conversions.ToString(Interaction.IIf(Information.IsNothing(RuntimeHelpers.GetObjectValue(arrOraParams[2].Value)), "N/A", RuntimeHelpers.GetObjectValue(arrOraParams[2].Value)));
                                    trans.Commit();
                                    if (sReturnMessage.Contains("ALARM REMOVE OK"))
                                    {
                                        arrOraParams = new OracleParameter[4];
                                        arrOraParams[0] = new OracleParameter("mydata", "END");
                                        arrOraParams[0].Direction = ParameterDirection.Input;
                                        arrOraParams[1] = new OracleParameter("g_machine", this.StationNo);
                                        arrOraParams[1].Direction = ParameterDirection.Input;
                                        arrOraParams[2] = new OracleParameter("g_emp", "FEEDBACK");
                                        arrOraParams[2].Direction = ParameterDirection.Input;
                                        arrOraParams[3] = new OracleParameter();
                                        arrOraParams[3].ParameterName = "res";
                                        arrOraParams[3].Size = 1024;
                                        arrOraParams[3].Direction = ParameterDirection.Output;
                                        OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.StoredProcedure, "MES1.CHECK_END", arrOraParams);
                                        sReturnMessage = Conversions.ToString(Operators.ConcatenateObject("ALARM REMOVE OK/", Interaction.IIf(Information.IsNothing(RuntimeHelpers.GetObjectValue(arrOraParams[3].Value)), "N/A", RuntimeHelpers.GetObjectValue(arrOraParams[3].Value))));
                                        this.SendData(0, this.IP, Message_convert(sReturnMessage, 0, this) + "\r\n\r\n" + ShowNextInput(), "", 0);
                                    }
                                    else
                                    {
                                        this.SendData(0, this.IP, Message_convert(sReturnMessage, 0, this) + "\r\n\r\n" + ShowNextInput(), "", 0);
                                    }

                                    l_conn.Close();
                                }
                                catch (Exception ex)
                                {
                                    l_conn.Close();
                                    string mzFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                                    string strLogFile = Path.Combine(mzFolder, "log_file.log");
                                    File.AppendAllText(strLogFile, string.Format("[E] {0}:{1} - {2} {3}", new object[]
                                    {
                                            MethodBase.GetCurrentMethod(),
                                            ex.ToString(),
                                            DateTime.Now.ToString(),
                                            Environment.NewLine
                                    }));
                                }
                            }
                        }
                        else
                        {
                            OracleParameter[] arrOraParams2 = new OracleParameter[3];
                            arrOraParams2[0] = new OracleParameter("mydata", inData);
                            arrOraParams2[0].Direction = ParameterDirection.Input;
                            arrOraParams2[1] = new OracleParameter("stationno", this.StationNo);
                            arrOraParams2[1].Direction = ParameterDirection.Input;
                            arrOraParams2[2] = new OracleParameter();
                            arrOraParams2[2].ParameterName = "res";
                            arrOraParams2[2].Size = 1024;
                            arrOraParams2[2].Direction = ParameterDirection.Output;
                            try
                            {
                                OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.StoredProcedure, "MES1.PUB_CHECK_EMP", arrOraParams2);
                                sReturnMessage = Conversions.ToString(arrOraParams2[2].Value);
                                string text2 = sReturnMessage + "\r\n\r\n" + ShowNextInput();
                                this.SendData(0, this.IP, text2, "", 0);
                            }
                            catch (Exception ex2)
                            {
                                string mzFolder2 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                                string strLogFile2 = Path.Combine(mzFolder2, "log_file.log");
                                string strLog2 = string.Format("[+] Debug variable: {0},{1},{2}", inData, this.IP, Environment.NewLine);
                                File.AppendAllText(strLogFile2, string.Format("[E] {0}:{1} - {2} {3}", new object[]
                                {
                                        MethodBase.GetCurrentMethod(),
                                        ex2.ToString() + strLog2,
                                        DateTime.Now.ToString(),
                                        Environment.NewLine
                                }));
                            }
                        }
                    }
                }
            }

            return sReturnMessage;
        }

        internal void Disconnect(string v)
        {
            if (Socket != null)
            {
                Socket.CloseWinsock();
            }
        }

        /// <summary>
        /// AUTO SCAN
        /// </summary>
        /// <param name="iIndex"></param>
        /// <param name="strData"></param>
        /// <param name="strline"></param>
        /// <param name="strStation"></param>
        /// <returns></returns>
        private string FillSmtMaterialNew(short iIndex, string strData, string strline, string strStation)
        {
            OracleParameter[] arrOraParams = new OracleParameter[4];
            arrOraParams[0] = new OracleParameter("G_PSN", strData);
            arrOraParams[0].Direction = ParameterDirection.InputOutput;
            arrOraParams[1] = new OracleParameter("G_LINE", strline);
            arrOraParams[1].Direction = ParameterDirection.Input;
            arrOraParams[2] = new OracleParameter("G_STATION", strStation);
            arrOraParams[2].Direction = ParameterDirection.Input;
            arrOraParams[3] = new OracleParameter();
            arrOraParams[3].ParameterName = "RES";
            arrOraParams[3].Size = 1024;
            arrOraParams[3].Direction = ParameterDirection.Output;
            string FillSmtMaterialNew = string.Empty;
            using (OracleConnection l_connection = new OracleConnection(HCONSTANT.ORACLE_CONNECTION_STRING_DB))
            {
                try
                {
                    l_connection.Open();
                    OracleTransaction trans = l_connection.BeginTransaction();
                    OracleHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "MES1.CHECK_PSN_SMT_MATERIAL", arrOraParams);
                    FillSmtMaterialNew = Conversions.ToString(Interaction.IIf(Information.IsNothing(RuntimeHelpers.GetObjectValue(arrOraParams[3].Value)), "N/A", RuntimeHelpers.GetObjectValue(arrOraParams[3].Value)));
                    trans.Commit();
                    l_connection.Close();
                }
                catch (Exception ex)
                {
                    l_connection.Close();
                    string mzFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string strLogFile = Path.Combine(mzFolder, "log_file.log");
                    string log = "--> DEBUG VARIABLE : " + strData + "," + strline + "," + FillSmtMaterialNew + Environment.NewLine;

                    File.AppendAllText(strLogFile, string.Format("[E] {0}:{1} - {2} {3}", new object[]
                    {
                        MethodBase.GetCurrentMethod(),
                        ex.ToString() + log + Environment.NewLine,
                        DateTime.Now.ToString(),
                        Environment.NewLine
                    }));
                }
            }
            return FillSmtMaterialNew;
        }

    }
}
