using DBLib;
using Microsoft.VisualBasic.CompilerServices;
using Oracle.ManagedDataAccess.Client;
using SCADA.Class.Logger;
using SCADACMC.Class;
using SCADACMC.Class.Constant;
using SCADACMC.Control;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SCADACMC.Class.Plugin;

namespace SCADACMC
{
    public partial class MainForm : Form
    {
        private DataTable m_DtTemp;
        public static ConcurrentQueue<CMCControlMessage> ConQueue = new ConcurrentQueue<CMCControlMessage>();
        private Thread m_thread;
        private CancellationTokenSource m_cancelToken = new CancellationTokenSource();
        private string sSql;
        private CheckMaterialOver10Min plugCheckMaterialOver10Min = new CheckMaterialOver10Min();
        private CheckLossMaterial plugCheckLossMaterial = new CheckLossMaterial();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            HSCADAFLAG.InitializeFlag();
            this.Text = string.Concat(new string[]
            {
                this.Text,
                " - Version: ",
                Application.ProductVersion,
                " - PC_NAME: ",
                HSCADAFLAG.G_PcName
            });

            if (HSCADAFLAG.IsUseFlagDatabaseTest == "T")
            {
                HCONSTANT.ORACLE_CONNECTION_STRING_DB = "Data Source=VNAP.TEST;User Id=SYSTEM;Password=123456;Min Pool Size=2;Max Pool Size=10";
            }
            else
            {
                HCONSTANT.ORACLE_CONNECTION_STRING_DB = "Data Source=" + HSCADAFLAG.DSN + ";User Id=AP2;Password=NSDAP2LOGPD0522;Min Pool Size=2;Max Pool Size=10";
            }


        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            toolStripButton1.Enabled = false;
            StartSCADACenter();
            m_thread = new Thread(new ParameterizedThreadStart(StartDataCollector));
            m_thread.Start(m_cancelToken.Token);
            this.Cursor = Cursors.Default;
            toolBtnStop.Enabled = true;
            this.tmrCheckStatus.Enabled = true;

            plugCheckMaterialOver10Min.ScadaID = HSCADAFLAG.G_ListScadaID.First().Key;
            plugCheckMaterialOver10Min.ScadaName = HSCADAFLAG.G_ListScadaID.First().Value;
            plugCheckLossMaterial.ScadaID = HSCADAFLAG.G_ListScadaID.First().Key;
            plugCheckLossMaterial.ScadaName = HSCADAFLAG.G_ListScadaID.First().Value;

            tmrCheckMaterialOver10Minds.Enabled = true;
        }

        private void StartDataCollector(object cancelToken)
        {
            CancellationToken token = (CancellationToken)cancelToken;
            while (!token.IsCancellationRequested)
            {
                Thread.Sleep(100);
                if (ConQueue.Count > 0)
                {
                    CMCControlMessage msg = null;
                    if (ConQueue.TryDequeue(out msg))
                    {
                        Task.Run(new Action(() =>
                        {
                            CMCControl cmc = CMCManager.GetInstance().CMCs[msg.Index];
                            if (!cmc.IsBusy)
                            {
                                var isComPortFlag = false;
                                byte[] message = msg.Data;
                                string l_sReturnMessage = string.Empty;
                                cmc.NamePanel.Invoke(new Action(() => { cmc.NamePanel.BackColor = System.Drawing.Color.FromArgb(246, 229, 141); }));
                                cmc.IsBusy = true;
                                cmc.IsConnected = true;
                                cmc.BeginExecute = Environment.TickCount;

                                string sDataInput = cmc.CheckIsCOMPort(message, message.Length, ref isComPortFlag).ToUpper();
                                if (!string.IsNullOrEmpty(sDataInput))
                                {

                                    l_sReturnMessage = cmc.OnProcessDataArrival(sDataInput, isComPortFlag);

                                    string format = string.Format("{0}({1})-Receive DATA:{2} -Return Msg:{3} {4}", new object[]
                                    {
                                        cmc.StationName,
                                        cmc.IP,
                                        sDataInput,
                                        l_sReturnMessage,
                                        DateTime.Now
                                    });
                                    if (!isComPortFlag)
                                    {
                                        var value = DateTime.Now.ToString("yyyyMMdd");
                                        WriteTextSafe(this.tbxRecvData, format + Environment.NewLine);
                                        SimpleLogger.Writeback("", cmc.StationName, value, format);
                                    }
                                    else
                                    {
                                        var value = DateTime.Now.ToString("yyyyMMdd");
                                        WriteTextSafe(this.tbxRecvData, format + Environment.NewLine);
                                    }
                                }

                                /**
                                  * BENCHMARKING
                                  * */
                                cmc.NamePanel.Invoke(new Action(() => { cmc.NamePanel.BackColor = System.Drawing.Color.FromArgb(118, 255, 3); }));
                                cmc.IsBusy = false;
                                cmc.EndExecute = Environment.TickCount;
                            }
                            else
                            {
                                long lTickCount = Environment.TickCount;
                                long eslapsed = lTickCount - cmc.BeginExecute;
                                if (eslapsed > 4444)
                                {

                                }
                            }
                        }));
                    }

                }
            }
        }
        private void WriteTextSafe(RichTextBox sender, string text)
        {
            if (sender.InvokeRequired)
            {
                sender.Invoke(new Action(() =>
                {
                    if (sender.Lines.Length > 1000)
                    {
                        sender.Clear();
                    }
                    sender.AppendText(text);
                    sender.ScrollToCaret();
                }));
            }
            else
            {
                if (sender.Lines.Length > 1000)
                {
                    sender.Clear();
                }
                sender.AppendText(text);
            }
        }

        private void StartSCADACenter()
        {
            HSCADAFLAG.G_ListPCName = HSCADAFLAG.G_PcName.Split(',');
            HSCADAFLAG.G_ListScadaID = new Dictionary<string, string>();
            if (HSCADAFLAG.G_ListPCName.Count() > 0)
            {
                this.trvCenter.Nodes[0].Nodes.Clear();
                foreach (var pcName in HSCADAFLAG.G_ListPCName)
                {

                    this.sSql = "select * from mes1.C_SCADA_NAME where SCADA_NAME='" + pcName + "' and SCADA_TYPE='0'";
                    this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, null);
                    bool flagg = this.m_DtTemp.Rows.Count > 0;
                    checked
                    {
                        if (flagg)
                        {
                            HSCADAFLAG.G_ListScadaID.Add(m_DtTemp.Rows[0]["SCADA_ID"].ToString(), pcName);
                            this.trvCenter.Nodes[0].Nodes.Add(m_DtTemp.Rows[0]["SCADA_ID"].ToString(), Conversions.ToString(this.m_DtTemp.Rows[0]["SCADA_NAME"]), "000_Pool_h32bit_16.png");
                            //this.tbxDebugNew.AppendText(Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("[I] SCADA_NAME: ", this.m_DtTemp.Rows[0]["SCADA_NAME"]), Environment.NewLine
                            #region InitializeFlag


                            this.sSql = "SELECT function_value1  From mes1.c_program_parameter  WHERE program_name = 'SCADA' and function_name='CHECK-ACTION+M+E' AND function_value1='Y' AND ROWNUM=1 ";
                            this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
                            bool flag2 = this.m_DtTemp.Rows.Count > 0;
                            if (flag2)
                            {
                                HSCADAFLAG.G_StopMachinFlag11 = true;
                            }
                            else
                            {
                                HSCADAFLAG.G_StopMachinFlag11 = false;
                            }
                            this.sSql = "SELECT * From mes1.c_program_parameter  WHERE program_name = 'SCADA' AND function_name = 'CHECK-TRSN-NOTONLINE-OVER-30-MIN'    AND function_object = 'CHECK-TRSN-NOTONLINE-OVER-30-MIN'    AND function_value1 = 'Y'    AND data1 = 'NORMAL' and rownum=1";
                            this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
                            bool flag3 = this.m_DtTemp.Rows.Count > 0;
                            if (flag3)
                            {
                                HSCADAFLAG.G_TrSnNotOnline32minsAlarm = true;
                                HSCADAFLAG.G_TrSnNotOnlineTime = (short)Conversions.ToInteger(this.m_DtTemp.Rows[0]["FUNCTION_VALUE2"]);
                            }
                            else
                            {
                                HSCADAFLAG.G_TrSnNotOnline32minsAlarm = false;
                                HSCADAFLAG.G_TrSnNotOnlineTime = 60;
                            }
                            this.sSql = "SELECT * From mes1.c_program_parameter  WHERE program_name = 'SCADA' AND function_name = 'OVER_TIME_STOP_MACHINE'    AND function_object = 'OVER_TIME_STOP_MACHINE'    AND function_value1 = 'Y'    AND data1 = 'NORMAL' and rownum=1";
                            this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
                            bool flag4 = this.m_DtTemp.Rows.Count > 0;
                            if (flag4)
                            {
                                HSCADAFLAG.G_IsStopMachineWhenOverTime = true;
                            }
                            else
                            {
                                HSCADAFLAG.G_IsStopMachineWhenOverTime = false;
                            }
                            this.sSql = "SELECT * From mes1.c_program_parameter  WHERE program_name = 'SCADA' AND function_name = 'OVER_TIME_STOP_MACHINE_FOR_MATERIAL_CONFIRM'    AND function_object = 'OVER_TIME_STOP_MACHINE_FOR_MATERIAL_CONFIRM'    AND function_value1 = 'Y'    AND data1 = 'NORMAL' and rownum=1";
                            this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
                            bool flag5 = this.m_DtTemp.Rows.Count > 0;
                            if (flag5)
                            {
                                HSCADAFLAG.G_StopMachineOvertimeMarialConfirm = true;
                            }
                            else
                            {
                                HSCADAFLAG.G_StopMachineOvertimeMarialConfirm = false;
                            }
                            this.sSql = "SELECT * From mes1.c_program_parameter  WHERE program_name = 'SCADA' AND function_name = 'OVER_MATERIAL_OFFLINE_STOP_MACHINE_OVER_5MINUTES'    AND function_object = 'OVER_MATERIAL_OFFLINE_STOP_MACHINE_OVER_5MINUTES'    AND function_value1 = 'Y'    AND data1 = 'NORMAL' and rownum=1";
                            this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
                            bool flag6 = this.m_DtTemp.Rows.Count > 0;
                            if (flag6)
                            {
                                HSCADAFLAG.G_StopMachineMaterialOfflineOver5mins = true;
                            }
                            else
                            {
                                HSCADAFLAG.G_StopMachineMaterialOfflineOver5mins = false;
                            }
                            this.sSql = "SELECT * From mes1.c_program_parameter  WHERE program_name = 'SCADA' AND function_name = 'stencil'    AND function_object = 'stencil_30Minutes_no_online'    AND function_value1 = 'Y'    AND data1 = 'NORMAL' and rownum=1";
                            this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
                            bool flag7 = this.m_DtTemp.Rows.Count > 0;
                            if (flag7)
                            {
                                HSCADAFLAG.G_StopMachineStencil30minsNoOnline = true;
                            }
                            else
                            {
                                HSCADAFLAG.G_StopMachineStencil30minsNoOnline = false;
                            }
                            this.sSql = "SELECT * From mes1.c_program_parameter  WHERE program_name = 'SCADA' AND function_name = 'stencil'    AND function_object = 'stencil_over4hour_offline'    AND function_value1 = 'Y'    AND data1 = 'NORMAL' and rownum=1";
                            this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
                            bool flag8 = this.m_DtTemp.Rows.Count > 0;
                            if (flag8)
                            {
                                HSCADAFLAG.G_StopMachineStencilOver4hNoOffline = true;
                            }
                            else
                            {
                                HSCADAFLAG.G_StopMachineStencilOver4hNoOffline = false;
                            }
                            this.sSql = "SELECT * From mes1.c_program_parameter  WHERE program_name = 'SCADA' AND function_name = 'stencil'    AND function_object = 'offline_stencil_need_online_in_10minutes'    AND function_value1 = 'Y'    AND data1 = 'NORMAL' and rownum=1";
                            this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
                            bool flag9 = this.m_DtTemp.Rows.Count > 0;
                            if (flag9)
                            {
                                HSCADAFLAG.G_StopMachineNeedOnlineStencilAfterOffline10Mins = true;
                            }
                            else
                            {
                                HSCADAFLAG.G_StopMachineNeedOnlineStencilAfterOffline10Mins = false;
                            }
                            this.sSql = "SELECT * From mes1.c_program_parameter  WHERE program_name = 'SCADA' AND function_name = 'stencil'    AND function_object = 'stencil_offline_30Minutes_no_wash'    AND function_value1 = 'Y'    AND data1 = 'NORMAL' and rownum=1";
                            this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
                            bool flag10 = this.m_DtTemp.Rows.Count > 0;
                            if (flag10)
                            {
                                HSCADAFLAG.G_StopMachineStencilOffline30minsNoWash = true;
                            }
                            else
                            {
                                HSCADAFLAG.G_StopMachineStencilOffline30minsNoWash = false;
                            }
                            this.sSql = "SELECT * From mes1.c_program_parameter  WHERE program_name = 'SCADA' AND function_name = 'SOLDER_24HOURS_OFFLINE_CONTROL'    AND function_object = 'SOLDER 24HOURS HAVE NOT OFFLINE TO STOP MACHINE'    AND function_value1 = 'Y'    AND data1 = 'NORMAL' and rownum=1";
                            this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
                            bool flag11 = this.m_DtTemp.Rows.Count > 0;
                            if (flag11)
                            {
                                HSCADAFLAG.G_StopMachineSolder24hNoOffline = true;
                            }
                            else
                            {
                                HSCADAFLAG.G_StopMachineSolder24hNoOffline = false;
                            }
                            this.sSql = "SELECT * From mes1.c_program_parameter  WHERE program_name = 'SCADA'  AND FUNCTION_NAME ='PCBA PASS AUTO SCANNER'     AND function_value1 = 'Y'    AND data1 = 'NORMAL' and rownum=1";
                            this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
                            bool flag12 = this.m_DtTemp.Rows.Count > 0;
                            if (flag12)
                            {
                                HSCADAFLAG.G_StopMachinePCBAMultiPassAutoScanner = true;
                            }
                            else
                            {
                                HSCADAFLAG.G_StopMachinePCBAMultiPassAutoScanner = false;
                            }
                            this.sSql = "SELECT * From mes1.c_program_parameter  WHERE program_name = 'SCADA' AND function_name = 'MATERAIL_NOTFULL_STOP_MACHINE'    AND function_object = 'MATERAIL_NOTFULL_STOP_MACHINE'    AND function_value1 = 'Y'    AND data1 = 'NORMAL' and rownum=1";
                            this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
                            bool flag13 = this.m_DtTemp.Rows.Count > 0;
                            if (flag13)
                            {
                                HSCADAFLAG.G_Material_NotFull_StopMachine_Flag = true;
                            }
                            else
                            {
                                HSCADAFLAG.G_Material_NotFull_StopMachine_Flag = false;
                            }
                            this.sSql = "SELECT *  From mes1.c_program_parameter   WHERE program_type = 'SP'   AND program_name = 'CHECK_PSN_SMT_MATERIAL'   AND function_name = 'CHECK_PSN_SMT_MATERIAL'   AND function_object = 'RECORD_STATION'   and function_value1='Y'   AND data1 = 'NORMAL' and rownum=1";
                            this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
                            bool flag14 = this.m_DtTemp.Rows.Count > 0;
                            if (flag14)
                            {
                                HSCADAFLAG.G_CheckMaterialRecordStation = true;
                            }
                            else
                            {
                                HSCADAFLAG.G_CheckMaterialRecordStation = false;
                            }
                            this.sSql = "SELECT *  From mes1.c_program_parameter WHERE program_type='SP'   AND program_name = 'SCADA'   AND function_name = 'DEDUCT_MATERIAL'   AND function_object = 'DEDUCT_MATERIAL'   AND function_value1 = 'Y'   AND data1 = 'NORMAL' and rownum=1";
                            this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
                            bool flag15 = this.m_DtTemp.Rows.Count > 0;
                            if (flag15)
                            {
                                HSCADAFLAG.G_DeductMaterialFlag = true;
                            }
                            else
                            {
                                HSCADAFLAG.G_DeductMaterialFlag = false;
                            }
                            this.sSql = "SELECT *  From mes1.c_program_parameter WHERE program_name = 'SCADA'   AND function_name = 'CHECK-CM-TRSN-NOTCONFIRM-OVER-10-MIN'   AND function_object = 'CHECK-CM-TRSN-NOTCONFIRM-OVER-10-MIN'   AND function_value1 = 'Y'   AND data1 = 'NORMAL' and rownum=1";
                            this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
                            bool flag16 = this.m_DtTemp.Rows.Count > 0;
                            if (flag16)
                            {
                                HSCADAFLAG.G_CMNotConfirmFlag = true;
                                HSCADAFLAG.G_CMNotConfirmTime = (short)Conversions.ToInteger(this.m_DtTemp.Rows[0]["FUNCTION_VALUE2"]);
                            }
                            else
                            {
                                HSCADAFLAG.G_CMNotConfirmFlag = false;
                                HSCADAFLAG.G_CMNotConfirmTime = 0;
                            }
                            this.sSql = "SELECT *  From mes1.c_program_parameter WHERE program_name = 'SCADA'   AND function_name = 'MACHINE-MATERIAL-CONFIRM-OVER-60-MIN'   AND function_object = 'MACHINE-MATERIAL-CONFIRM-OVER-60-MIN'   AND function_value1 = 'Y'   AND data1 = 'NORMAL' and rownum=1";
                            this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
                            bool flag17 = this.m_DtTemp.Rows.Count > 0;
                            if (flag17)
                            {
                                HSCADAFLAG.G_CMaterialconfirmFlag = true;
                                HSCADAFLAG.G_CMaterialconfirmTime = (short)Conversions.ToInteger(this.m_DtTemp.Rows[0]["FUNCTION_VALUE2"]);
                            }
                            else
                            {
                                HSCADAFLAG.G_CMaterialconfirmFlag = false;
                                HSCADAFLAG.G_CMaterialconfirmTime = 0;
                            }
                            this.sSql = "SELECT *  From mes1.c_program_parameter WHERE program_name = 'SCADA'   AND function_name = 'G_PCBA_ERROR_LINE_Flag'   AND function_object = 'LOCK_MACHINE_H1'   AND function_value1 = 'Y'   AND data1 = 'NORMAL' and rownum=1";
                            this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
                            bool flag18 = this.m_DtTemp.Rows.Count > 0;
                            if (flag18)
                            {
                                HSCADAFLAG.G_PCBA_ERROR_LINE_Flag = true;
                            }
                            else
                            {
                                HSCADAFLAG.G_PCBA_ERROR_LINE_Flag = false;
                            }
                            this.sSql = "SELECT * From mes1.c_program_parameter  WHERE program_name = 'SCADA' AND function_name = 'CHECK-MSD-TRSN-OVERTIME'    AND function_object = 'CHECK-MSD-TRSN-OVERTIME'    AND function_value1 = 'Y'    AND data1 = 'NORMAL' and rownum=1";
                            this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
                            bool flag19 = this.m_DtTemp.Rows.Count > 0;
                            if (flag19)
                            {
                                HSCADAFLAG.G_StopMachineCheckMSDOvertime = true;
                            }
                            else
                            {
                                HSCADAFLAG.G_StopMachineCheckMSDOvertime = false;
                            }
                            this.sSql = "SELECT * FROM mes1.c_program_parameter WHERE program_name = 'SCADA' AND function_object = 'CHECK-MATERIAL-OVER-10-MIN' AND function_value1 = 'Y' AND data1 = 'NORMAL'";
                            this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
                            bool flag20 = this.m_DtTemp.Rows.Count > 0;
                            if (flag20)
                            {
                                // this.CheckMaterialOver10Min.Enabled = true;
                            }
                            else
                            {
                                // this.CheckMaterialOver10Min.Enabled = false;

                            }
                            //<Added by: V0940534-IT at: 12/2/2019-14:26:39 on machine: V0940534-IT>
                            this.sSql = @"SELECT * FROM mes1.c_program_parameter WHERE program_name = 'SCADA'
                                          AND function_object = 'ALARM_PRG_IP' AND function_value1 = 'Y' AND data1 = 'NORMAL' AND DATA3='" + pcName + "'";
                            m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, sSql);

                            if (m_DtTemp.Rows.Count > 0)
                                HSCADAFLAG.G_AlarmPrgMEIp.Add(m_DtTemp.Rows[0]["FUNCTION_VALUE2"].ToString());
                            else
                                HSCADAFLAG.G_AlarmPrgMEIp.Add("NO_IP");
                            //</Added by: V0940534-IT at: 12/2/2019-14:26:39 on machine: V0940534-IT>
                            #endregion
                        }
                    }
                }

                foreach (var item in HSCADAFLAG.G_ListScadaID)
                {
                    this.InitializeTreeViewGW28(item.Key);
                }
            }

            //scada tasss
            foreach (var item in HSCADAFLAG.G_ListScadaID)
            {
                InitStationInfo(item);
            }

        }

        private void InitStationInfo(KeyValuePair<string, string> ScadaID)
        {
            // GET SCADA INFO
            string sSql = string.Empty;
            DataTable m_DtTemp = null;

            sSql = "select count(STATION_NO) co from  mes1.C_GW28_CONFIG where SCADA_ID='" + ScadaID.Key + "'";
            m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, sSql, null);
            bool flag = m_DtTemp.Rows.Count > 0;

            if (flag)
            {
                var temp = short.Parse(Conversions.ToString(m_DtTemp.Rows[0]["CO"]));
            }
            sSql = "select * from mes1.C_GW28_CONFIG where SCADA_ID='" + ScadaID.Key + "'";
            m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, sSql, new OracleParameter[0]);

            int idx = 0;
            foreach (DataRow row in m_DtTemp.Rows)
            {
                CMCControl cmcControl = new CMCControl()
                {
                    ID = idx,
                    ScadaId = ScadaID.Key,
                    StationNo = row["STATION_NO"].ToString(),
                    LineName = row["LINE_NAME"].ToString(),
                    StationName = row["station_name"].ToString(),
                    PointCode = row["point_code"].ToString(),
                    StoredProcedure = row["SP_NAME"].ToString(),
                    LanguageFlag = row["language_flag"].ToString(),
                    IP = row["ip"].ToString(),
                    IsConnected = false,
                    IsStart = false,
                    FailTimes = 0,
                    BeginExecute = Environment.TickCount
                };
                //l_gw28model.Stopwatch = new Stopwatch();
                if (Util.ValidateIPv4(cmcControl.IP))
                {
                    cmcControl.IpAddress = cmcControl.IP;
                    cmcControl.PortNumber = 55962;
                    cmcControl.ID = idx;
                    cmcControl.Name = ScadaID.Value;
                    cmcControl.NamePanel.Text = cmcControl.StationName;
                    cmcControl.Connect();

                    CMCManager.GetInstance().AddGW28(cmcControl);
                    this.flowLayoutPanel1.Controls.Add(cmcControl);
                }
                else
                {
                    CMCManager.GetInstance().AddGW28(cmcControl);
                    this.flowLayoutPanel1.Controls.Add(cmcControl);
                }
                idx++;
            }
        }

        private void InitializeTreeViewGW28(string scadaID)
        {
            this.sSql = "SELECT DISTINCT(LINE_NAME) FROM MES1.C_GW28_CONFIG WHERE SCADA_ID='" + scadaID + "' ORDER BY LINE_NAME";
            this.m_DtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
            try
            {
                foreach (object obj in this.m_DtTemp.Rows)
                {
                    DataRow row = (DataRow)obj;
                    TreeNode rootNode = this.trvCenter.Nodes[0].Nodes.Find(scadaID, false).FirstOrDefault<TreeNode>();
                    TreeNode secNode = rootNode.Nodes.Add(Conversions.ToString(Operators.ConcatenateObject("LINE_", row["LINE_NAME"])), Conversions.ToString(row["LINE_NAME"]), "000_Server_h32bit_16.png");
                    this.sSql = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("SELECT DISTINCT(STATION_NAME) FROM MES1.C_GW28_CONFIG  WHERE SCADA_ID='" + scadaID + "' AND LINE_NAME='", row["LINE_NAME"]), "'  ORDER BY STATION_NAME"));
                    DataTable l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);
                    try
                    {
                        foreach (object obj2 in l_dtTemp.Rows)
                        {
                            DataRow row2 = (DataRow)obj2;
                            TreeNode thirdNode = secNode.Nodes.Add(Conversions.ToString(Operators.ConcatenateObject("STATION_", row2["STATION_NAME"])), Conversions.ToString(row2["STATION_NAME"]), "000_VM_h32bit_16.png");
                            this.sSql = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("SELECT DISTINCT STATION_NO,POINT_CODE FROM MES1.C_GW28_CONFIG WHERE SCADA_ID='" + scadaID + "' AND LINE_NAME='", row["LINE_NAME"]), "' AND STATION_NAME='"), row2["STATION_NAME"]), "' ORDER BY POINT_CODE"));
                            DataTable l_dtTemp2 = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, this.sSql, new OracleParameter[0]);

                            foreach (object obj3 in l_dtTemp2.Rows)
                            {
                                DataRow row3 = (DataRow)obj3;
                                bool flag = !HSCADAFLAG.G_IsStart;
                                if (flag)
                                {
                                    thirdNode.Nodes.Add(Conversions.ToString(Operators.ConcatenateObject("STATION_NO_", row3["STATION_NO"])), Conversions.ToString(row3["STATION_NO"]), "000_StoppedVM_h32bit_16.png");
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            this.trvCenter.ExpandAll();
        }

        private void tmrCheckStatus_Tick(object sender, EventArgs e)
        {
            tmrCheckStatus.Enabled = false;
            var models = CMCManager.GetInstance().CMCs;
            foreach (var model in models)
            {
                if (model.Socket.State != OSWINSCK.StateConstants.sckConnected && model.Socket.State != OSWINSCK.StateConstants.sckConnecting)
                {
                    if (Util.ValidateIPv4(model.IP))
                    {
                        model.Connect();
                    }
                }
                if (model.Socket != null && model.Socket.State == OSWINSCK.StateConstants.sckConnected)
                {
                    var systemtick = Environment.TickCount;
                    if (Math.Abs(model.BeginExecute - systemtick) > 10000)
                    {
                        if (!model.IsBusy)
                        {
                            model.Connect();
                        }
                    }
                }
            }
            tmrCheckStatus.Enabled = true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!m_cancelToken.IsCancellationRequested) m_cancelToken.Cancel();
        }

        private void tmrCheckMaterialOver10Minds_Tick(object sender, EventArgs e)
        {
            this.tmrCheckMaterialOver10Minds.Enabled = false;
            Task.Run(() => { plugCheckMaterialOver10Min.OnExecute(); plugCheckLossMaterial.OnExecute(); }).Wait();
            this.tmrCheckMaterialOver10Minds.Enabled = true;
        }

        private void toolBtnStop_Click(object sender, EventArgs e)
        {
            toolBtnStop.Enabled = false;
            var models = CMCManager.GetInstance().CMCs;
            foreach (var model in models)
            {
                if (model.Socket != null)
                {
                    model.Disconnect("STOP");
                }

                model.Dispose();
                tmrCheckStatus.Enabled = false;
                tmrCheckMaterialOver10Minds.Enabled = false;
                tmrCheckLossMaterial.Enabled = false;
                trvCenter.Nodes[0].Nodes.Clear();
            }
            toolStripButton1.Enabled = true;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            var models = CMCManager.GetInstance().CMCs;
            foreach (var model in models)
            {

                if (model.Socket != null)
                {
                    byte[] resetCmd = new byte[] { 0x1b, 0x00 };
                    if (model.Socket.State == OSWINSCK.StateConstants.sckConnected)
                    {
                        model.Socket.SendData(resetCmd);
                    }
                }
            }
        }
    }
}
