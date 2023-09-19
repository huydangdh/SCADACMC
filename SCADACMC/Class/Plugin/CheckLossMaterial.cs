using DBLib;
using SCADA.Class.Logger;
using SCADACMC.Class.Constant;
using SCADACMC.Control;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SCADACMC.Class.Plugin
{
    public class CheckLossMaterial
    {
        public string ScadaID;
        public string ScadaName;
        private object _objLock = new object();

        public void OnExecute()
        {
            lock (_objLock)
            {
                DataTable l_dtTmp;
                DataTable l_dtTmp1;
                string strsql = string.Empty;
                string ip_log = string.Empty;
                int tmp_time;
                int tmp_time1;
                try
                {
                    strsql = "select m.ip,n.* from mes1.c_gw28_config m,(";
                    strsql = strsql + "SELECT station, kp_no, slot_no ";
                    strsql = strsql + "FROM mes4.r_station_wip WHERE tr_sn IS NULL ";
                    strsql = strsql + "AND shareslot_flag = 0 AND station IN (SELECT station_name ";
                    strsql = strsql + "FROM mes1.c_gw28_config WHERE substr(station_name,6,2) not in('CT','AA','SB','AP') and scada_id = '" + this.ScadaID + "')  ";
                    strsql = strsql + "AND standard_qty > 0 UNION SELECT station, kp_no, slot_no FROM mes4.r_station_wip ";
                    strsql = strsql + " WHERE tr_sn IS NULL AND shareslot_flag = 1 AND station IN (SELECT station_name ";
                    strsql = strsql + "FROM mes1.c_gw28_config  WHERE substr(station_name,6,2) not in('CT','AA','SB','AP') and scada_id = '" + this.ScadaID + "') ";
                    strsql = strsql + "AND kp_no NOT IN (SELECT kp_no FROM mes4.r_station_wip  WHERE tr_sn IS NOT NULL ";
                    strsql = strsql + "AND shareslot_flag = 1 AND station IN (SELECT station_name ";
                    strsql = strsql + "FROM mes1.c_gw28_config WHERE substr(station_name,6,2) not in('CT','AA','SB','AP') and scada_id = '" + this.ScadaID + "') ) AND standard_qty > 0 ";
                    strsql = strsql + ") n where m.station_name=n.station";

                    bool result1 = HSCADAFLAG.G_Material_NotFull_StopMachine_Flag;
                    l_dtTmp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                    foreach (DataRow row in l_dtTmp.Rows)
                    {
                        ip_log = row.Field<string>("IP");
                        CMCControl gw28model = CMCManager.GetInstance().GetCMCByIP(ip_log);
                        // check xem slot lieu nao chua online
                        // dong thoi check xem co action-m-c hay action-m-h ko 
                        if (HSCADAFLAG.G_Material_NotFull_StopMachine_Flag == true)
                        {
                            if (gw28model.State == MyStateSocket.sckConnected)
                            {
                                string strsql1 = "SELECT function_value2,memo " +
                                " From mes1.c_program_parameter" +
                                " WHERE program_name = 'SCADA'" +
                                " AND function_name = 'MATERAIL_NOTFULL_STOP_MACHINE'" +
                                " AND function_object = 'MATERAIL_NOTFULL_STOP_MACHINE'" +
                                " AND function_value1 = 'Y'" +
                                " AND data1 = 'NORMAL' and rownum=1";
                                l_dtTmp1 = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql1);
                                tmp_time = int.Parse(l_dtTmp1.Rows[0].Field<string>("FUNCTION_VALUE2")); // M-C TIMER 
                                tmp_time1 = int.Parse(l_dtTmp1.Rows[0].Field<string>("Memo")); // M-H TIMER
                                                                                               // CHECK SCAN ACTION-M-C
                                strsql1 = "SELECT distinct(a.station_name) station_name, a.ip ip, b.time2 diff_time " +
                                            "FROM mes1.c_gw28_config a, " +
                                            "(SELECT STATION,KP_NO, CEIL ((SYSDATE - time1)*24*60) time2 " +
                                            "FROM (SELECT STATION,KP_NO, MAX (work_time) time1 " +
                                            "From mes4.r_tr_sn_null_confirm " +
                                            "WHERE STATION='" + row.Field<string>("station") + "' AND KP_NO='" + row.Field<string>("kp_no") +
                                            "' AND MEMO='ACTION-M-C' " +
                                            "GROUP BY STATION,KP_NO) m " +
                                            "WHERE (SYSDATE - time1)*24*60 > " + tmp_time + ") b," +
                                            "(select STATION from mes4.r_station_wip where STATION='" + row.Field<string>("station") +
                                            "' and kp_no='" + row.Field<string>("kp_no") + "' and tr_sn is null) c " +
                                            "WHERE a.ip='" + row.Field<string>("ip") +
                                            "' and a.station_name = b.station AND a.station_name=c.station and b.station =c.station order by station_name asc ";
                                l_dtTmp1 = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql1);
                                if (l_dtTmp1.Rows.Count > 0)
                                    gw28model.SendAlarm1(
                                        gw28model.ID,
                                        row.Field<string>("ip"),
                                        row.Field<string>("slot_no") + "||" + row.Field<string>("kp_no") + "->ACTION-M-C:QUA " + l_dtTmp1.Rows[0].Field<decimal>("diff_time") + " PHUT KHONG SAO LEN CHUYEN,MAY SE DUNG!",
                                        "",
                                        0);
                                else
                                {
                                    // ACTION-M-H
                                    strsql1 = "SELECT distinct(a.station_name) station_name, a.ip ip, b.time2 diff_time " +
                                              "FROM mes1.c_gw28_config a, " +
                                              "(SELECT STATION,KP_NO, CEIL ((SYSDATE - time1)*24*60) time2 " +
                                              "FROM (SELECT STATION,KP_NO, MAX (work_time) time1 " +
                                              "From mes4.r_tr_sn_null_confirm " +
                                              "WHERE STATION='" + row.Field<string>("station") + "' AND KP_NO='" + row.Field<string>("kp_no") +
                                              "' AND MEMO='ACTION-M-H' AND WORK_FLAG='0' " +
                                              "GROUP BY STATION,KP_NO) m " +
                                              "WHERE (SYSDATE - time1)*24*60 > " + tmp_time1 + ") b," +
                                              "(select STATION from mes4.r_station_wip where STATION='" + row.Field<string>("station") + "' and kp_no='" + row.Field<string>("kp_no") +
                                              "' and tr_sn is null) c " + "WHERE a.ip='" + row.Field<string>("ip") +
                                              "' and a.station_name = b.station AND a.station_name=c.station and b.station =c.station order by station_name asc ";
                                    l_dtTmp1 = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql1);
                                    if (l_dtTmp1.Rows.Count > 0)
                                        gw28model.SendAlarm1(gw28model.ID, row.Field<string>("ip"), row.Field<string>("slot_no") + "||" + row.Field<string>("kp_no") + "->ACTION-M-H:Overtime " + l_dtTmp1.Rows[0].Field<decimal>("diff_time") + " minutes no scan on line,machine will shutdown!", "", 0);
                                    else
                                    {
                                        // all material station
                                        strsql1 = "select * from mes4.r_station_wip where station='" + row.Field<string>("station") + "' and feeder_type<>'TRAY' and tr_sn is null";
                                        l_dtTmp1 = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql1);
                                        if (l_dtTmp1.Rows.Count > 0)
                                            gw28model.SendAlarm1(gw28model.ID, row.Field<string>("ip"), row.Field<string>("slot_no") + "||" + row.Field<string>("kp_no") + " no scan on line, machine will shutdown!", "", 0);
                                    }
                                }


                                strsql1 = "SELECT * FROM MES4.R_PROGRAM_LOG WHERE PRG_NAME='SCADA' AND FUN_NAME='MATERAIL_NOTFULL_STOP_MACHINE' AND ACTION_TYPE='" + row.Field<string>("station") +
                                            "' AND OLDSN='" + row.Field<string>("kp_no") +
                                            "' AND DATA1=0 AND DATA2='" + row.Field<string>("slot_no") + "'";
                                l_dtTmp1 = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql1);
                                if (l_dtTmp1.Rows.Count == 0)
                                {
                                    strsql = "Insert into MES4.R_PROGRAM_LOG(PRG_NAME, FUN_NAME, ACTION_TYPE, OLDSN,NEWSN,DATA1,DATA2,TIME) " +
                                             "Values('SCADA', 'MATERAIL_NOTFULL_STOP_MACHINE', '" + row.Field<string>("station") + "', '" + row.Field<string>("kp_no") + "','" + row.Field<string>("ip") + "','0','" + row.Field<string>("slot_no") + "', SYSDATE)";
                                    OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                }
                                else
                                {
                                    strsql = "UPDATE MES4.R_PROGRAM_LOG SET TIME=SYSDATE WHERE PRG_NAME='SCADA' AND FUN_NAME='MATERAIL_NOTFULL_STOP_MACHINE' AND ACTION_TYPE='" + row.Field<string>("station") +
                                             "' AND OLDSN='" + row.Field<string>("kp_no") + "' AND DATA1=0 AND DATA2='" + row.Field<string>("slot_no") + "'";
                                    OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                }
                                l_dtTmp1.Rows.Clear();
                                l_dtTmp1.Dispose();
                            }
                        }
                        else if (gw28model.IsConnected)
                            gw28model.SendAlarm1(gw28model.ID, gw28model.IP, row.Field<string>("slot_no") + "||" + row.Field<string>("kp_no") + "no scan on line,machine will shutdown!", "", 0);

                        string strMessage = DateTime.Now + "\n" + gw28model.StationName + "(" + gw28model.IP + ")-Receive CHECK_LOSSMATERIAL:" + row.Field<string>("slot_no") + "||" + row.Field<string>("kp_no") + " no scan on line,machine will shutdown!";
                        SimpleLogger.Writeback(this.ScadaName, gw28model.StationName, DateTime.Now.ToString("yyyyMMdd"), strMessage);
                    }
                    l_dtTmp.Rows.Clear();
                    l_dtTmp.Dispose();
                }
                catch (Exception ex2)
                {
                    string mzFolder2 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string strLogFile2 = Path.Combine(mzFolder2, "log_file.log");
                    string log = "--> DEBUG VARIABLE : " + strsql + Environment.NewLine + ip_log + Environment.NewLine;
                    File.AppendAllText(strLogFile2, string.Format("[E] {0}:{1} - {2} {3}", new object[]
                    {
                                        MethodBase.GetCurrentMethod(),
                                        ex2.ToString() + Environment.NewLine + log,
                                        DateTime.Now.ToString(),
                                        Environment.NewLine
                    }));

                }
            }
        }
    }
}
