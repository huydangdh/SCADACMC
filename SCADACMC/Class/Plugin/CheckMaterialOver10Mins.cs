using DBLib;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using SCADA.Class.Logger;
using SCADACMC.Class.Constant;
using SCADACMC.Control;
using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace SCADACMC.Class.Plugin
{
    public class CheckMaterialOver10Min
    {
        string str_tr_sn;
        string str_station_name;
        string str_ip;
        string stop_reason;
        int strdiff_time;
        string strsql;
        string strsql1;
        int myid = 0;
        string strMessage;
        string c_kp_no;
        int limit_test;
        string stringsn;
        string msd_type;
        DataTable l_dtTemp = null/* TODO Change to default(_) if this is not a reference type */;
        string l_sTemp;
        DataTable l_dtTemp1 = null/* TODO Change to default(_) if this is not a reference type */;
        int limit;
        private object _objLock = new object();

        /// <summary>
        /// VARIALBE
        /// </summary>
        /// 
        public string ScadaID;
        public string ScadaName;

        private string check_machine_type(string station, string ip)
        {
            var res = string.Empty;
            var strsql = "SELECT DISTINCT EDIT_EMP MACHINE_TYPE FROM MES1.C_GW28_CONFIG WHERE STATION_NAME='" + station.Trim() + "' AND IP='" + ip.Trim() + "' AND ROWNUM=1";
            DataTable l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
            if (l_dtTemp.Rows.Count > 0)
            {
                res = l_dtTemp.Rows[0].Field<string>("MACHINE_TYPE");

            }
            else
            {
                res = "MACHINE";
                strMessage = DateTime.Now + "\n" + station.Trim() + " (" + ip.Trim() + ") - This machine and ip not exist in table MES1.C_GW28_CONFIG!";
                SimpleLogger.Writeback(this.ScadaName, station, DateTime.Now.ToString("yyyyMMdd"), strMessage);
            }
            l_dtTemp.Dispose();
            return res;
        }

        public void OnExecute()
        {
            try
            {
                lock (_objLock)
                {
                    // ---------------------UNLOCK PANASONIC MACHINE by ACTION-M-U - by THAN QUYEN------------------------------------------
                    strsql = @"SELECT A.STATION_NAME STATION, IP FROM MES1.C_LINE_STATION A, MES1.C_GW28_CONFIG B 
                       WHERE A.STATION_NAME=B.STATION_NAME AND DATA1='9' AND B.SCADA_ID='" + ScadaID + "'" +
                             @"Union 
                       SELECT STATION, IP FROM MES4.R_STOP_MACHINE A, MES1.C_GW28_CONFIG B 
                       WHERE A.STATION=B.STATION_NAME AND LOCK_FLAG='9' AND B.SCADA_ID='" + ScadaID + "'";
                    l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                    if (l_dtTemp.Rows.Count > 0)
                    {
                        foreach (DataRow row in l_dtTemp.Rows)
                        {
                            str_station_name = row.Field<string>("station");
                            str_ip = row.Field<string>("ip");
                            // unlock_machine_panasonic str_station_name, str_ip
                            strsql = "UPDATE MES1.C_LINE_STATION SET DATA1='0' WHERE STATION_NAME='" + str_station_name.Trim() + "'";
                            OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                            strsql = "UPDATE MES4.R_STOP_MACHINE SET LOCK_FLAG='N' WHERE STATION='" + str_station_name.Trim() + "'";
                            OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                        }
                    }
                    // ---------------------END UNLOCK PANASONIC MACHINE by ACTION-M-U - by THAN QUYEN------------------------------------------
                    // ---------------------CHECK STOP MACHINE IN TABLE MES4.R_STOP_MACHINE - by THAN QUYEN------------------------------------------
                    strsql = "SELECT DISTINCT A.ID, A.STATION, A.LOCK_FLAG, A.STOP_REASON, A.FROM_PROGRAM, B.IP " +
                            "FROM MES4.R_STOP_MACHINE A, MES1.C_GW28_CONFIG B, MES1.C_SMT_STOP_MACHINE C " +
                            "WHERE A.STATION=B.STATION_NAME AND A.LOCK_FLAG='Y' AND A.ID=C.ID AND C.FUNCTION_VALUE1='Y' AND B.SCADA_ID='" + ScadaID + "' " +
                            "AND SUBSTR(C.ID,1,3)=SUBSTR(B.STATION_NAME,1,3) ORDER BY STATION";
                    l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                    if (l_dtTemp.Rows.Count > 0)
                    {
                        foreach (DataRow row in l_dtTemp.Rows)
                        {

                            str_station_name = row.Field<string>("station");
                            stop_reason = row.Field<string>("stop_reason");
                            str_ip = row.Field<string>("ip");
                            CMCControl gw28model = CMCManager.GetInstance().GetCMCByIP(str_ip);
                            str_station_name = gw28model.StationName;

                            if (gw28model.State == MyStateSocket.sckConnected)
                            {
                                gw28model.SendAlarm1(
                                    myid,
                                    str_ip,
                                    " " + str_station_name + ":Will stop because " + stop_reason + ", Please Call IPQC UNLOCK!",
                                    "",
                                    0
                                    );
                                // If check_machine_type(str_station_name, str_ip) = "PANASONIC" Then
                                // lock_machine_panasonic str_station_name, str_ip
                                // End If
                                strMessage = DateTime.Now + Constants.vbCrLf + str_station_name + "(" + gw28model.IP + ")-Receive DATA18:will stop because:" + stop_reason + ", Please Call IPQC UNLOCK!.";
                                SimpleLogger.Writeback(this.ScadaName, gw28model.StationName, DateTime.Now.ToString("yyyyMMdd"), strMessage);
                            }
                        }
                    }
                    // ---------------------END CHECK STOP MACHINE IN TABLE MES4.R_STOP_MACHINE - by THAN QUYEN------------------------------------------
                    // ---------------------The machine SCAN ACTION+M+E , Then stop the machine 20130910 by marcus-------------------------------'
                    if (HSCADAFLAG.G_StopMachinFlag11 == true)
                    {
                        strsql = " SELECT aa.station_name station, aa.ip IP, scada_id " + "  FROM mes1.c_gw28_config aa, mes4.r_system_log bb " +
                                 " WHERE bb.action_desc = 'LOCK' AND aa.station_name = bb.prg_name AND aa.SCADA_ID='" + ScadaID + "'";
                        l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                        if (l_dtTemp.Rows.Count > 0)
                        {
                            foreach (DataRow row in l_dtTemp.Rows)
                            {
                                str_station_name = row.Field<string>("station");
                                str_ip = row.Field<string>("IP");
                                CMCControl gw28model = CMCManager.GetInstance().GetCMCByIP(str_ip);
                                str_station_name =gw28model.StationName;
                                if (gw28model.State == MyStateSocket.sckConnected)
                                {
                                    gw28model.SendAlarm1(myid,
                                        str_ip,
                                        " THE MACHINE:" + str_station_name + " STOP FOR ALL MATERIAL TAKEOFF(ACTION+M+E)", "", 0
                                        );

                                    strMessage = DateTime.Now + Constants.vbCrLf + gw28model.StationName + "-Receive DATA10:THE MACHINE " + str_station_name + " STOP FOR ALL MATERIAL OFF WITH";
                                    SimpleLogger.Writeback(this.ScadaName, gw28model.StationName, DateTime.Now.ToString("yyyyMMdd"), strMessage);

                                }
                            }
                        }
                    }
                    // -----------------------The machine ACTION+M+E scan , Then stop the machine 20130910---------------'
                    // ----------------------------------------------Lock machine then shutdown by Luongvd007 20140808------------------------------
                    if (HSCADAFLAG.G_CMaterialconfirmFlag == true)
                    {
                        strsql = "select distinct(a.station_name) station_name,data1,b.ip ip from mes1.c_line_station a , mes1.c_gw28_config b " +
                                 "WHERE (a.data1='1' or a.data1='MT') and a.station_name =b.station_name and b.SCADA_ID='" + this.ScadaID + "' and a.station_name not like '%CT%'";
                        l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                        if (l_dtTemp.Rows.Count > 0)
                        {
                            foreach (DataRow row in l_dtTemp.Rows)
                            {
                                str_station_name = row.Field<string>("station_name");
                                str_ip = row.Field<string>("ip");
                                l_sTemp = row.Field<string>("Data1");

                                CMCControl gw28model = CMCManager.GetInstance().GetCMCByIP(str_ip);
                                str_station_name = gw28model.StationName;
                                if (gw28model.State == MyStateSocket.sckConnected)
                                {
                                    if (Strings.Left(l_sTemp, 1) == "1")
                                    {
                                        gw28model.SendAlarm1(myid, str_ip, " " + str_station_name + ": The machine has LOCK, Please Call IPQC UNLOCK!", "", 0);
                                        if (check_machine_type(str_station_name, str_ip) == "PANASONIC")
                                            lock_machine_panasonic(str_station_name, str_ip);
                                        strMessage = DateTime.Now + Constants.vbCrLf + gw28model.StationName + "(" + gw28model.IP + ")-Receive DATA14: The machine has LOCK, Please Call IPQC UNLOCK!";
                                        SimpleLogger.Writeback(this.ScadaName, gw28model.StationName, DateTime.Now.ToString("yyyyMMdd"), strMessage);
                                    }
                                    else
                                    {
                                        gw28model.SendAlarm1(myid, str_ip, " " + str_station_name + ": The machine has LOCK, Please Call IPQC UNLOCK!", "", 0);

                                        if (check_machine_type(str_station_name, str_ip) == "PANASONIC")
                                        {
                                            lock_machine_panasonic(str_station_name, str_ip);
                                            strMessage = DateTime.Now + Constants.vbCrLf + gw28model.StationName + "(" + gw28model.IP + ")-Receive DATA14: The machine has LOCK by IPQC action M-T, Please Call IPQC UNLOCK!";
                                            SimpleLogger.Writeback(this.ScadaName, gw28model.StationName, DateTime.Now.ToString("yyyyMMdd"), strMessage);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // ----------------------------------------------Lock machine then shutdown by Luongvd007 20140808------------------------------
                    // -------------------------------------------Over 30 minutes TR_SN not online--------------------------------------------------
                    if (HSCADAFLAG.G_TrSnNotOnline32minsAlarm)
                    {
                        strsql = @"SELECT * FROM (SELECT DISTINCT m.tr_sn, CEIL ((SYSDATE - m.work_time) * 24 * 60) diff_time, n.station_name, n.ip 
                           FROM mes4.r_tr_sn_wip m, mes1.c_gw28_config n, mes4.r_station_wip l, mes4.r_tr_sn o, mes1.c_smt_ap_list k 
                           WHERE m.work_flag = '0' AND m.station_flag = '1' and n.SCADA_ID='" + this.ScadaID + "'" + "AND (SYSDATE - m.work_time) * 24 * 60 > " + HSCADAFLAG.G_TrSnNotOnlineTime +
                                 @"AND NOT EXISTS (SELECT 1 FROM MES4.r_tr_sn_wip WHERE M.STATION=STATION AND SUBSTR (m.station, 6, 2) ='CT') 
                           AND o.tr_sn = m.tr_sn AND m.station = n.station_name AND m.wo = l.wo AND o.data4 = l.process_flag AND m.station = l.station 
                           AND l.tr_sn is not null AND l.smt_code = k.smt_code AND m.kp_no = l.KP_NO AND m.kp_no IN k.kp_no AND l.smt_code IS NOT NULL 
                           AND NOT EXISTS(SELECT 1 FROM mes4.r_station_wip WHERE L.FEEDER_TYPE = FEEDER_TYPE AND FEEDER_TYPE = 'TRAY')) ORDER BY station_name, ip, tr_sn";
                        l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                        if (l_dtTemp.Rows.Count > 0)
                        {
                            foreach (DataRow row in l_dtTemp.Rows)
                            {
                                str_tr_sn = row.Field<string>("TR_SN");
                                str_ip = row.Field<string>("ip");
                                strdiff_time = int.Parse(row.Field<decimal>("diff_time").ToString());

                                CMCControl gw28model = CMCManager.GetInstance().GetCMCByIP(str_ip);
                                str_station_name =gw28model.StationName;
                                if (gw28model.State == MyStateSocket.sckConnected)
                                {

                                }
                            }
                        }
                    }
                    // -------------------------------------------END Over 30 minutes TR_SN not online----------------------------------------------

                    // -------------------------------------------Overtime 10 minutes not scan null disk confirm----------------------------------------------------------------------
                    if (HSCADAFLAG.G_CMNotConfirmFlag == true)
                    {
                        strsql = "SELECT   m.tr_sn, CEIL ((SYSDATE - m.work_time) * 24 * 60) AS diff_time," +
                            "         m.station AS station, n.ip AS ip                               " +
                            "    FROM mes4.r_tr_sn_null_confirm m,                                   " +
                            "         (SELECT y.wo, x.smt_code, x.kp_no, y.station, z.ip             " +
                            "            FROM mes1.c_smt_ap_list x,                                  " +
                            "                 mes4.r_station_wip y,                                  " +
                            "                 mes1.c_gw28_config z                                   " +
                            "   WHERE x.smt_code = y.smt_code and z.SCADA_ID='" + this.ScadaID + "'     " +
                            "             AND y.station = z.station_name                             " +
                            "             AND NOT EXISTS(SELECT 1  FROM mes4.r_station_wip           " +
                            "             WHERE y.FEEDER_TYPE = FEEDER_TYPE AND FEEDER_TYPE = 'TRAY') ";
                        strsql = strsql + "          UNION                                                         " +
                            "          SELECT b.wo, a.smt_code, a.kp_no, b.station, b.ip             " +
                            "            FROM mes1.c_replace_kp a,                                   " +
                            "                 (SELECT DISTINCT(y.wo), x.smt_code, x.slot_no, x.kp_no, " +
                            "                  y.station, z.ip                                        " +
                            "                  FROM mes1.c_smt_ap_list x,                             " +
                            "                  mes4.r_station_wip y,                                  " +
                            "                  mes1.c_gw28_config z                                   " +
                            "    WHERE x.smt_code = y.smt_code  and z.SCADA_ID='" + this.ScadaID + "'    " +
                            "                  AND x.replacekp_flag = '1'                             " +
                            "                  AND y.station = z.station_name                         " +
                            "             AND NOT EXISTS(SELECT 1  FROM mes4.r_station_wip            " +
                            "             WHERE y.FEEDER_TYPE = FEEDER_TYPE AND FEEDER_TYPE = 'TRAY'))b " +
                            "             WHERE a.smt_code = b.smt_code                                 " +
                            "             AND (a.kp_no = b.kp_no OR a.replace_kp_no = b.kp_no)          ";
                        strsql = strsql + "          UNION                                                         " +
                            "          SELECT b.wo, a.smt_code, a.replace_kp_no, b.station, b.ip     " +
                            "            FROM mes1.c_replace_kp a,                                   " +
                            "                 (SELECT DISTINCT(y.wo), x.smt_code, x.slot_no, x.kp_no, " +
                            "                                  y.station, z.ip                       " +
                            "                             FROM mes1.c_smt_ap_list x,                 " +
                            "                                  mes4.r_station_wip y,                 " +
                            "                                  mes1.c_gw28_config z                  " +
                            "      WHERE x.smt_code = y.smt_code and z.SCADA_ID='" + this.ScadaID + "'  " +
                            "                              AND x.replacekp_flag = '1'                " +
                            "                              AND y.station = z.station_name            " +
                            "             AND NOT EXISTS(SELECT 1  FROM mes4.r_station_wip            " +
                            "             WHERE y.FEEDER_TYPE = FEEDER_TYPE AND FEEDER_TYPE = 'TRAY'))b " +
                            "           WHERE a.smt_code = b.smt_code                                   " +
                            "             AND (a.kp_no = b.kp_no OR a.replace_kp_no = b.kp_no)) n       ";
                        strsql = strsql + "   WHERE (SYSDATE - work_time) * 24 * 60 > " + HSCADAFLAG.G_CMNotConfirmTime +
                            "     AND (m.work_flag = '0' OR work_flag = '2')                         " +
                            "     AND m.kp_no = n.kp_no                                              " +
                            "     AND m.wo = n.wo   and m.work_flag='0' AND m.station =n.station     " +
                            "     AND NOT EXISTS(SELECT 1 FROM mes4.r_tr_sn_null_confirm " +
                            "     WHERE m.station = station AND station = 'ERR-TRSN') and m.work_time > sysdate - 1 " +
                            "ORDER BY station           ";
                        l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                        if (l_dtTemp.Rows.Count > 0)
                        {
                            foreach (DataRow row in l_dtTemp.Rows)
                            {
                                str_tr_sn = row.Field<string>("TR_SN");
                                str_ip = row.Field<string>("ip");
                                strdiff_time = int.Parse(row.Field<decimal>("diff_time").ToString());

                                CMCControl gw28model = CMCManager.GetInstance().GetCMCByIP(str_ip);
                                str_station_name = gw28model.StationName;
                                if (gw28model.State == MyStateSocket.sckConnected)
                                {
                                    if (HSCADAFLAG.G_IsStopMachineWhenOverTime)
                                    {
                                        gw28model.SendAlarm1(myid, str_ip, str_station_name + " :" + " :" + str_tr_sn + Strings.Chr(58) + " qua 10p (" + strdiff_time + "p), Kitting chua sao xac nhan het lieu (M-J), May se bi dung.", "", 0);
                                        if (check_machine_type(str_station_name, str_ip) == "PANASONIC")
                                            lock_machine_panasonic(str_station_name, str_ip);
                                        strsql1 = "SELECT * FROM MES4.R_PROGRAM_LOG WHERE PRG_NAME='SCADA' AND FUN_NAME='CHECK-CM-TRSN-NOTCONFIRM-OVER-10-MIN' AND ACTION_TYPE='" + str_station_name + "' AND OLDSN='" + str_tr_sn + "' AND DATA1=0 AND DATA2<='" + strdiff_time + "'";
                                        l_dtTemp1 = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql1);
                                        if (l_dtTemp1.Rows.Count == 0)
                                        {
                                            strsql = "Insert into MES4.R_PROGRAM_LOG(PRG_NAME, FUN_NAME, ACTION_TYPE, OLDSN,NEWSN,DATA1,DATA2,DATA3,TIME) " + "Values('SCADA', 'CHECK-CM-TRSN-NOTCONFIRM-OVER-10-MIN', '" + str_station_name + "', '" + str_tr_sn + "','" + str_ip + "','0','" + strdiff_time + "','ACTION-M-J', SYSDATE)";
                                            OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                        }
                                        else
                                        {
                                            strsql = "UPDATE MES4.R_PROGRAM_LOG SET DATA2='" + strdiff_time + "' WHERE PRG_NAME='SCADA' AND FUN_NAME='CHECK-CM-TRSN-NOTCONFIRM-OVER-10-MIN' AND ACTION_TYPE='" + str_station_name + "' AND OLDSN='" + str_tr_sn + "' AND DATA1=0 AND DATA2<='" + strdiff_time + "'";
                                            OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                        }
                                        l_dtTemp1.Rows.Clear();
                                        l_dtTemp1.Dispose();
                                        strMessage = DateTime.Now + Constants.vbCrLf + str_station_name + "(" + gw28model.IP + ")-Receive DATA03:" + str_tr_sn + " qua 10p (" + strdiff_time + "p), Kitting chua sao xac nhan het lieu (M-J), May se bi dung.";
                                        SimpleLogger.Writeback(this.ScadaName, gw28model.StationName, DateTime.Now.ToString("yyyyMMdd"), strMessage);
                                    }
                                }

                            }
                        }

                    }

                    // -------------------------------------------END Overtime 10 minutes not scan null disk confirm----------------------------------------------------------------------
                    // -------------------------------------------60-100 Minutes machine have not confirm----------------------------------------------------------------------
                    if (HSCADAFLAG.G_CMaterialconfirmFlag == true)
                    {
                        strsql = "  select function_value2,Memo from mes1.c_program_parameter " + " WHERE program_name = 'SCADA' " + " AND function_name = 'OVER_TIME_STOP_MACHINE_FOR_MATERIAL_CONFIRM' " + " AND function_object = 'OVER_TIME_STOP_MACHINE_FOR_MATERIAL_CONFIRM' " + " AND function_value1 = 'Y'";
                        l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                        limit = int.Parse(l_dtTemp.Rows[0].Field<string>("FUNCTION_VALUE2"));
                        limit_test = int.Parse(l_dtTemp.Rows[0].Field<string>("Memo"));

                        strsql = " SELECT distinct(a.station_name) station_name, a.ip ip, b.time2 diff_time " +
                            "   FROM mes1.c_gw28_config a, " +
                            "     (SELECT machine, CEIL ((SYSDATE - time1)*24*60) time2 " +
                            "        FROM (SELECT   CATEGORY, machine, MAX (work_time) time1 " +
                            "          FROM mes4.r_job_record " +
                            "            WHERE CATEGORY = 'CHECK_WIP_KP' " +
                            "              GROUP BY CATEGORY, machine) m " +
                            "                WHERE (SYSDATE - time1)*24*60 > " + limit_test + ") b, " +
                            "                 mes4.r_station_wip c " +
                            "                  WHERE a.station_name = b.machine AND a.station_name=c.station AND a.SCADA_ID='" + this.ScadaID + "' order by station_name asc ";
                        l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                        foreach (DataRow row in l_dtTemp.Rows)
                        {
                            str_station_name = row.Field<string>("station_name");
                            str_ip = row.Field<string>("ip");
                            strdiff_time = int.Parse(row["diff_time"].ToString());

                            CMCControl gw28model = CMCManager.GetInstance().GetCMCByIP(str_ip);
                            str_station_name =gw28model.StationName;
                            if (gw28model.State == MyStateSocket.sckConnected)
                            {
                                if ((System.Convert.ToInt32(strdiff_time) >= limit_test && System.Convert.ToDouble(strdiff_time) <= limit))
                                {
                                    // gw28cls(myid).SendAlarm myid, str_ip, str_station_name & " :" & Chr(58) & " Over " & strdiff_time & " minutes,  please do machine material confirm,Machine will be Shutdown.", "", 0
                                    gw28model.SendAlarm(myid, str_ip, str_station_name + " :" + Strings.Chr(58) + " qua " + limit_test + "p (" + strdiff_time + "p) chua sao xac nhan lieu, Hay sao xac nhan lieu", "", 0);
                                }
                                if ((System.Convert.ToDouble(strdiff_time) > limit))
                                {
                                    if (HSCADAFLAG.G_StopMachineOvertimeMarialConfirm)
                                    {
                                        gw28model.SendAlarm1(myid, str_ip, str_station_name + " :" + Strings.Chr(58) + " qua " + limit + "p (" + strdiff_time + "p) chua sao xac nhan lieu, May se bi dung", "", 0);
                                        if (check_machine_type(str_station_name, str_ip) == "PANASONIC")
                                            lock_machine_panasonic(str_station_name, str_ip);
                                        strsql1 = "SELECT * FROM MES4.R_PROGRAM_LOG WHERE PRG_NAME='SCADA' AND FUN_NAME='OVER_TIME_STOP_MACHINE_FOR_MATERIAL_CONFIRM' AND ACTION_TYPE='" + str_station_name + "' AND DATA1=0 AND DATA2<='" + strdiff_time + "'";
                                        l_dtTemp1 = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql1);
                                        if (l_dtTemp1.Rows.Count == 0)
                                        {
                                            strsql1 = "SELECT * FROM MES4.R_PROGRAM_LOG WHERE PRG_NAME='SCADA' AND ACTION_TYPE = '" + str_station_name + "' AND FUN_NAME='OVER_TIME_STOP_MACHINE_FOR_MATERIAL_CONFIRM' " +
                                                        "AND TIME<=(SELECT MAX(WORK_TIME) FROM mes4.r_job_record WHERE CATEGORY = 'CHECK_WIP_KP' AND MACHINE='" + str_station_name + "' ) " +
                                                        "AND DATA1=0 AND TO_CHAR(TIME,'YYYYMMDD')=TO_CHAR(SYSDATE,'YYYYMMDD')";
                                            l_dtTemp1 = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                            if (l_dtTemp1.Rows.Count > 0)
                                            {
                                                strsql1 = "UPDATE MES4.R_PROGRAM_LOG SET DATA1=1,TIME=SYSDATE WHERE PRG_NAME='SCADA' AND ACTION_TYPE = '" + str_station_name + "' AND FUN_NAME='OVER_TIME_STOP_MACHINE_FOR_MATERIAL_CONFIRM' " + "AND TIME<=(SELECT MAX(WORK_TIME) FROM mes4.r_job_record WHERE CATEGORY = 'CHECK_WIP_KP' AND MACHINE='" + str_station_name + "' ) " + "AND DATA1=0 AND TO_CHAR(TIME,'YYYYMMDD')=TO_CHAR(SYSDATE,'YYYYMMDD')";
                                                OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql1);
                                            }
                                            else
                                            {
                                                strsql1 = "Insert into MES4.R_PROGRAM_LOG(PRG_NAME, FUN_NAME, ACTION_TYPE, OLDSN,NEWSN,DATA1,DATA2,DATA3,TIME) " + "Values('SCADA', 'OVER_TIME_STOP_MACHINE_FOR_MATERIAL_CONFIRM', '" + str_station_name + "', '','" + str_ip + "','0','" + strdiff_time + "','ACTION-M-K', SYSDATE)";
                                                OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql1);
                                            }
                                        }
                                        else
                                        {
                                            strsql = "UPDATE MES4.R_PROGRAM_LOG SET DATA2='" + strdiff_time + "' WHERE PRG_NAME='SCADA' AND FUN_NAME='OVER_TIME_STOP_MACHINE_FOR_MATERIAL_CONFIRM' AND ACTION_TYPE='" + str_station_name + "' AND DATA1=0 AND DATA2<='" + strdiff_time + "'";
                                            OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                        }
                                        l_dtTemp1.Rows.Clear();
                                        l_dtTemp1.Dispose();
                                        strMessage = DateTime.Now + Constants.vbCrLf + str_station_name + "(" + gw28model.IP + ")-Receive DATA04:" + str_station_name + " qua " + limit + "p (" + strdiff_time + "p) chua sao xac nhan lieu, May se bi dung";
                                        SimpleLogger.Writeback(this.ScadaName, gw28model.StationName, DateTime.Now.ToString("yyyyMMdd"), strMessage);
                                    }
                                }

                            }
                        }
                    }

                    // -------------------------------------------end 60-100 Minutes machine have not confirm----------------------------------------------------------------------

                    // -------------------------------------------Material no scan online by James mo 20121222-----------------------------------------------------------------------
                    if (HSCADAFLAG.G_StopMachineMaterialOfflineOver5mins)
                    {
                        strsql = " SELECT MM.station_name station_name, MM.ip ip, NN.KP_NO,NN.diff_time " + "   FROM mes1.c_gw28_config MM, " +
                                 "   (select STATION,KP_NO,diff_time from ( " +
                                 "   SELECT   a.station, a.kp_no, CEIL((SYSDATE - MAX (a.work_time))  *24 * 60)  diff_time " +
                                 "     FROM mes4.r_tr_sn_null_confirm a, mes4.r_station_wip b,mes4.r_tr_sn_wip c " +
                                 "          WHERE a.station = b.station AND a.station = c.station AND a.kp_no = b.kp_no AND c.work_flag =1 AND a.wo = b.wo AND SUBSTR (b.emp_no, 0, 1) <> '+' " +
                                 "                AND b.tr_sn IS NULL AND b.station LIKE '%A%' AND NOT EXISTS(SELECT 1 FROM mes4.r_station_wip " +
                                 "                WHERE b.FEEDER_TYPE = FEEDER_TYPE AND FEEDER_TYPE = 'TRAY') " +
                                 "                GROUP BY a.station, a.kp_no " +
                                 "                   ORDER BY station, a.kp_no ) TB1 WHERE  diff_time > 3 ) NN " +
                                 "                    WHERE MM.station_name= NN.station AND MM.SCADA_ID='" + this.ScadaName + "'";
                        l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                        foreach (DataRow row in l_dtTemp.Rows)
                        {
                            str_ip = row.Field<string>("ip");
                            strdiff_time = int.Parse(row["diff_time"].ToString());
                            c_kp_no = row.Field<string>("kp_no");

                            CMCControl gw28model = CMCManager.GetInstance().GetCMCByIP(str_ip);
                            str_station_name =gw28model.StationName;
                            if (gw28model.State == MyStateSocket.sckConnected)
                            {
                                gw28model.SendAlarm1(myid, str_ip, str_station_name + " : " + c_kp_no + Strings.Chr(58) + " qua 5p (" + strdiff_time + "p) tiep lieu chua hoan thanh. May se bi dung", "", 0);
                                if (check_machine_type(str_station_name, str_ip) == "PANASONIC")
                                    lock_machine_panasonic(str_station_name, str_ip);
                                strsql = "SELECT * FROM MES4.R_PROGRAM_LOG WHERE PRG_NAME='SCADA' AND FUN_NAME='CHECK-MATERIAL-OVER-10-MIN' AND ACTION_TYPE='" + str_station_name + "' AND OLDSN='" + c_kp_no + "' AND DATA1=0 AND DATA2<='" + strdiff_time + "'";
                                l_dtTemp1 = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                if (l_dtTemp1.Rows.Count == 0)
                                {
                                    strsql = "Insert into MES4.R_PROGRAM_LOG(PRG_NAME, FUN_NAME, ACTION_TYPE, OLDSN,NEWSN,DATA1,DATA2,DATA3,TIME) " + "Values('SCADA', 'CHECK-MATERIAL-OVER-10-MIN', '" + str_station_name + "', '" + c_kp_no + "','" + str_ip + "','0','" + strdiff_time + "','StopMachineMaterialOfflineOver5mins', SYSDATE)";
                                    OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                }
                                else
                                {
                                    strsql = "UPDATE MES4.R_PROGRAM_LOG SET DATA2='" + strdiff_time + "' WHERE PRG_NAME='SCADA' AND FUN_NAME='CHECK-MATERIAL-OVER-10-MIN' AND ACTION_TYPE='" + str_station_name + "' AND OLDSN='" + c_kp_no + "' AND DATA1=0 AND DATA2<='" + strdiff_time + "'";
                                    OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                }
                                l_dtTemp1.Rows.Clear();
                                l_dtTemp1.Dispose();
                                strMessage = DateTime.Now + Constants.vbCrLf + str_station_name + "(" + gw28model.IP + ")-Receive StopMachineMaterialOfflineOver5mins:" + c_kp_no + " qua 5p (" + strdiff_time + "p) tiep lieu chua hoan thanh. May se bi dung";
                                SimpleLogger.Writeback(this.ScadaName, gw28model.StationName, DateTime.Now.ToString("yyyyMMdd"), strMessage);
                            }
                        }
                    }
                    // -------------------------------------------END Material no scan online by James mo 20121222-------------------------------------------------------------------
                    // -------------------------------------------stencil over 30 p have no online:alarm&pause----------------------------------------------------------------------

                    if (HSCADAFLAG.G_CMaterialconfirmFlag == true)
                    {
                        strsql = " SELECT a.station_name station_name, a.ip ip, b.TIME1 diff_time,b.stencil_sn from " +
                                 " mes1.c_gw28_config a, " +
                                 " (SELECT stencil_sn, line_name||'AH1' station_name, wo, CEIL((SYSDATE - loan_time) * 24 * 60) TIME1 " +
                                 " FROM (SELECT a.*, (SELECT MAX (end_time) " +
                                 "                     FROM mes4.r_stencil_detail " +
                                 "                    WHERE stencil_sn = a.stencil_sn) detail_time, " +
                                 "             (SELECT MAX (start_time) " +
                                 "                FROM mes4.r_stencil_wash " +
                                 "              WHERE stencil_sn = a.stencil_sn) wash_time " +
                                 "       FROM mes4.r_stencil_whs a " +
                                 "      WHERE return_time IS NULL " +
                                 "         AND EXISTS (SELECT 1 " +
                                 "                        FROM mes1.c_stencil_base " +
                                 "                       WHERE stencil_sn = a.stencil_sn AND work_flag = '2' AND PROCESS <> 'C'))" +
                                 " WHERE (    loan_time > detail_time " +
                                 "     AND loan_time > wash_time " +
                                 "    AND loan_time < SYSDATE - 1 / 48 " + "    ) " +
                                 "   ) b where a.station_name =b.station_name AND a.SCADA_ID='" + this.ScadaID + "'";
                        l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                        foreach (DataRow row in l_dtTemp.Rows)
                        {
                            str_station_name = row.Field<string>("station_name");
                            str_ip = row.Field<string>("ip");
                            strdiff_time = int.Parse(row["diff_time"].ToString());

                            CMCControl gw28model = CMCManager.GetInstance().GetCMCByIP(str_ip);
                            str_station_name =gw28model.StationName;

                            string stencil_location = Getstencillocation(row.Field<string>("stencil_sn"));
                            if (gw28model.State == MyStateSocket.sckConnected)
                            {
                                gw28model.SendAlarm(myid, str_ip, "Khuon thiec/dao " + row.Field<string>("stencil_sn") + "-" + stencil_location + " qua 30p (" + strdiff_time + "p) chua online. May se bi dung", "", 0);
                                if (HSCADAFLAG.G_StopMachineStencil30minsNoOnline)
                                {
                                    gw28model.SendAlarm1(myid, str_ip, "Khuon thiec/dao " + row.Field<string>("stencil_sn") + "-" + stencil_location + " qua 30p (" + strdiff_time + "p) chua online. May se bi dung", "", 0);
                                    if (check_machine_type(str_station_name, str_ip) == "PANASONIC")
                                        lock_machine_panasonic(str_station_name, str_ip);
                                    strsql = "SELECT * FROM MES4.R_PROGRAM_LOG WHERE PRG_NAME='SCADA' AND FUN_NAME='stencil_30Minutes_no_online' AND ACTION_TYPE='" + str_station_name + "' AND OLDSN='" + row.Field<string>("stencil_sn") + "' AND DATA1=0 AND DATA2<='" + strdiff_time + "'";
                                    l_dtTemp1 = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                    if (l_dtTemp1.Rows.Count == 0)
                                    {
                                        strsql = "Insert into MES4.R_PROGRAM_LOG(PRG_NAME, FUN_NAME, ACTION_TYPE, OLDSN,NEWSN,DATA1,DATA2,TIME) " + "Values('SCADA', 'stencil_30Minutes_no_online', '" + str_station_name + "', '" + row.Field<string>("stencil_sn") + "','" + str_ip + "','0','" + strdiff_time + "', SYSDATE)";
                                        OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                    }
                                    else
                                    {
                                        strsql = "UPDATE MES4.R_PROGRAM_LOG SET DATA2='" + strdiff_time + "' WHERE PRG_NAME='SCADA' AND FUN_NAME='stencil_30Minutes_no_online' AND ACTION_TYPE='" + str_station_name + "' AND OLDSN='" + row.Field<string>("stencil_sn") + "' AND DATA1=0 AND DATA2<='" + strdiff_time + "'";
                                        OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                    }
                                    l_dtTemp1.Rows.Clear();
                                    l_dtTemp1.Dispose();

                                    strMessage = DateTime.Now + Constants.vbCrLf + gw28model.StationName + "-Receive DATA06: Khuon thiec/dao " + row.Field<string>("stencil_sn") + "-" + stencil_location + " qua 30p (" + strdiff_time + "p) chua online. May se bi dung";
                                    SimpleLogger.Writeback(this.ScadaName, gw28model.StationName, DateTime.Now.ToString("yyyyMMdd"), strMessage);

                                }
                            }
                        }
                    }
                    // -------------------------------------------END stencil over 30 have no online:alarm&pause------------------------------------------------------------------
                    // -------------------------------------------stencil of online must offline after using 4 hours:alarm&pause----------------------------------------------------------------------
                    if (HSCADAFLAG.G_CMaterialconfirmFlag == true)
                    {
                        strsql = @"SELECT aa.station_name,
                               aa.ip,
                               stencil_sn,
                               CEIL ( (SYSDATE - bb.start_time) * 24 * 60) diff_time
                          FROM mes1.c_gw28_config aa,
                               (SELECT cc.line_name || 'AH1' station_name,
                                       cc.stencil_sn,
                                       cc.start_time
                                  FROM mes4.r_stencil_wip cc, mes1.c_stencil_base dd
                                 WHERE     (SYSDATE - cc.start_time) * 24 * 60 > dd.LIMIT_WASH_TIME
                                       AND cc.stencil_sn = dd.stencil_sn
                                       AND dd.process NOT IN ('C')) bb
                         WHERE aa.station_name = bb.station_name AND aa.SCADA_ID = '" + this.ScadaID + "' ";

                        l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                        foreach (DataRow row in l_dtTemp.Rows)
                        {
                            str_station_name = row.Field<string>("station_name");
                            str_ip = row.Field<string>("ip");
                            strdiff_time = int.Parse(row["diff_time"].ToString());

                            CMCControl gw28model = CMCManager.GetInstance().GetCMCByIP(str_ip);
                            str_station_name =gw28model.StationName;


                            string stencil_location = Getstencillocation(row.Field<string>("stencil_sn"));
                            if (gw28model.State == MyStateSocket.sckConnected)
                            {
                                // SendCMCAlarm.SendAlarm(myid, str_ip, "Khuon thiec/dao " & row.Field<string>("stencil_sn") & " da online qua 240p (" & strdiff_time & "p). May se bi dung", "", 0)
                                gw28model.SendAlarm(myid, str_ip, "Khuon thiec/dao " + row.Field<string>("stencil_sn") + " da qua thoi gian online (" + strdiff_time + "p). May se bi dung", "", 0);

                                strsql1 = "SELECT * FROM MES4.R_PROGRAM_LOG WHERE PRG_NAME='SCADA' AND FUN_NAME='stencil_over4hour_offline' AND ACTION_TYPE='" + str_station_name + "' AND OLDSN='" + row.Field<string>("stencil_sn") + "' AND DATA1=0 AND DATA2<='" + strdiff_time + "'";

                                l_dtTemp1 = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql1);
                                if (l_dtTemp1.Rows.Count == 0)
                                {
                                    strsql = "Insert into MES4.R_PROGRAM_LOG(PRG_NAME, FUN_NAME, ACTION_TYPE, OLDSN,NEWSN,DATA1,DATA2,TIME) " + "Values('SCADA', 'stencil_over4hour_offline', '" + str_station_name + "', '" + row.Field<string>("stencil_sn") + "','" + str_ip + "','0','" + strdiff_time + "', SYSDATE)";
                                    OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                }
                                else
                                {
                                    strsql = "UPDATE MES4.R_PROGRAM_LOG SET DATA2='" + strdiff_time + "' WHERE PRG_NAME='SCADA' AND FUN_NAME='stencil_over4hour_offline' AND ACTION_TYPE='" + str_station_name + "' AND OLDSN='" + row.Field<string>("stencil_sn") + "' AND DATA1=0 AND DATA2<='" + strdiff_time + "'";
                                    OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                }

                                if (HSCADAFLAG.G_StopMachineStencilOver4hNoOffline)
                                {
                                    // SendCMCAlarm.SendAlarm1(myid, str_ip, "Khuon thiec " & row.Field<string>("stencil_sn") & " da online qua 240p (" & strdiff_time & "p). May se bi dung", "", 0)
                                    gw28model.SendAlarm1(myid, str_ip, "Khuon thiec/dao " + row.Field<string>("stencil_sn") + " da qua thoi gian online (" + strdiff_time + "p). May se bi dung", "", 0);

                                    if (check_machine_type(str_station_name, str_ip) == "PANASONIC")
                                        lock_machine_panasonic(str_station_name, str_ip);
                                    // strMessage = Now & vbCrLf & m_gw28obj.Gw28Model[myid].StationName & "-Receive DATA07: Khuon thiec " & row.Field<string>("stencil_sn") & " da online qua 240p (" & strdiff_time & "p). May se bi dung"
                                    strMessage = DateTime.Now + Constants.vbCrLf + gw28model.StationName + "-Receive DATA07: Khuon thiec/dao " + row.Field<string>("stencil_sn") + " da qua thoi gian online (" + strdiff_time + "p). May se bi dung";

                                    SimpleLogger.Writeback(this.ScadaName, gw28model.StationName, DateTime.Now.ToString("yyyyMMdd"), strMessage);
                                }

                                l_dtTemp1.Rows.Clear();
                                l_dtTemp1.Dispose();
                            }
                        }
                    }
                    // -------------------------------------------stencil of online must offline after using limit time hours:alarm&pause-------------------------------------------------------------
                    // -------------------------------------------END stencil of online must offline after using 4 hours:alarm&pause----------------------------------------------------------------------
                    // -------------------------------------------stencil have no wash after offline 30 minutes :alarm&pause----------------------------------------------------------------------
                    if (HSCADAFLAG.G_CMaterialconfirmFlag == true)
                    {
                        strsql = " SELECT a.station_name station_name, a.ip ip, b.TIME1 diff_time,b.stencil_sn from " +
                            "  mes1.c_gw28_config a," +
                            " (SELECT stencil_sn, line_name||'AH1' station_name, wo, CEIL((SYSDATE - detail_time) * 24 * 60) TIME1 " +
                            "   FROM (SELECT a.*, (SELECT MAX (end_time)" + "                        FROM mes4.r_stencil_detail" +
                            "                       WHERE stencil_sn = a.stencil_sn) detail_time," +
                            "                (SELECT MAX (start_time)" +
                            "                   FROM mes4.r_stencil_wash" +
                            "                  WHERE stencil_sn = a.stencil_sn) wash_time" +
                            "           FROM mes4.r_stencil_whs a" +
                            "          WHERE return_time IS NULL" +
                            "            AND EXISTS (SELECT 1" +
                            "                          FROM mes1.c_stencil_base" +
                            "                         WHERE stencil_sn = a.stencil_sn AND work_flag = '2' and process <> 'C'))" +
                            "  WHERE (    loan_time < detail_time" +
                            "         AND wash_time < detail_time" +
                            "         AND detail_time < SYSDATE - 1/48" +
                            "        )" +
                            "        ) b where a.station_name = b.station_name and a.SCADA_ID='" + this.ScadaID + "'";

                        l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                        foreach (DataRow row in l_dtTemp.Rows)
                        {
                            str_station_name = row.Field<string>("station_name");
                            str_ip = row.Field<string>("ip");
                            strdiff_time = int.Parse(row["diff_time"].ToString());

                            CMCControl gw28model = CMCManager.GetInstance().GetCMCByIP(str_ip);
                            str_station_name =gw28model.StationName;

                            string stencil_location = Getstencillocation(row.Field<string>("stencil_sn"));

                            if (gw28model.State == MyStateSocket.sckConnected)
                            {
                                gw28model.SendAlarm(myid, str_ip, "Khuon thiec/dao " + row.Field<string>("stencil_sn") + "-" + stencil_location + " da offline qua 30p (" + strdiff_time + "p)  ma chua rua khuon. May se bi dung", "", 0);
                                strsql1 = "SELECT * FROM MES4.R_PROGRAM_LOG WHERE PRG_NAME='SCADA' AND FUN_NAME='stencil_offline_30Minutes_no_wash' AND ACTION_TYPE='" + str_station_name + "' AND OLDSN='" + row.Field<string>("stencil_sn") + "' AND DATA1=0 AND DATA2<='" + strdiff_time + "'";

                                l_dtTemp1 = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                if (l_dtTemp1.Rows.Count == 0)
                                {
                                    strsql = "Insert into MES4.R_PROGRAM_LOG(PRG_NAME, FUN_NAME, ACTION_TYPE, OLDSN,NEWSN,DATA1,DATA2,TIME) " + "Values('SCADA', 'stencil_offline_30Minutes_no_wash', '" + str_station_name + "', '" + row.Field<string>("stencil_sn") + "','" + str_ip + "','0','" + strdiff_time + "', SYSDATE)";
                                    OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                }
                                else
                                {
                                    strsql = "UPDATE MES4.R_PROGRAM_LOG SET DATA2='" + strdiff_time + "' WHERE PRG_NAME='SCADA' AND FUN_NAME='stencil_offline_30Minutes_no_wash' AND ACTION_TYPE='" + str_station_name + "' AND OLDSN='" + row.Field<string>("stencil_sn") + "' AND DATA1=0 AND DATA2<='" + strdiff_time + "'";
                                    OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                }
                                l_dtTemp1.Rows.Clear();
                                l_dtTemp1.Dispose();

                                if (HSCADAFLAG.G_StopMachineStencilOffline30minsNoWash)
                                {
                                    gw28model.SendAlarm1(myid, str_ip, "Khuon thiec/dao " + row.Field<string>("stencil_sn") + "-" + stencil_location + " da offline qua 30p (" + strdiff_time + "p)  ma chua rua khuon. May se bi dung", "", 0);
                                    if (check_machine_type(str_station_name, str_ip) == "PANASONIC")
                                        lock_machine_panasonic(str_station_name, str_ip);
                                    strMessage = DateTime.Now + Constants.vbCrLf + gw28model.StationName + "-Receive DATA08: Khuon thiec/dao " + row.Field<string>("stencil_sn") + "-" + stencil_location + " da offline qua 30p (" + strdiff_time + "p)  ma chua rua khuon. May se bi dung";
                                    SimpleLogger.Writeback(this.ScadaName, gw28model.StationName, DateTime.Now.ToString("yyyyMMdd"), strMessage);
                                }
                            }
                        }
                    }
                    // -------------------------------------------end stencil have no wash after offline 30 minutes :alarm&pause----------------------------------------------------------------------
                    // -------------------------------------------the other stencil need on_line in 10 minutes after one stencil off-line :alarm&pause----------------------------------------------------------------------
                    if (HSCADAFLAG.G_CMaterialconfirmFlag == true)
                    {
                        strsql = " SELECT DISTINCT bb.STENCIL_SN, aa.station_name station_name, aa.ip ip, round(bb.strdiff_time,2) diff_time " +
                                 "  FROM mes1.c_gw28_config aa, " +
                                 "   ( SELECT   line_name || 'AH1' station_name,CEIL((sysdate - MAX (end_time))*60*24) strdiff_time,STENCIL_SN " +
                                 "           FROM mes4.r_stencil_detail" +
                                 "            WHERE end_time > SYSDATE - 4/24 " +
                                 "              AND station_name NOT IN (SELECT station_name FROM mes4.r_stencil_wip)" +
                                 "                AND (SYSDATE - end_time) * 24 * 60 > 10 GROUP BY line_name,STENCIL_SN   ) bb " +
                                 "          WHERE aa.station_name = bb.station_name and aa.SCADA_ID='" + this.ScadaID + "'";

                        l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                        foreach (DataRow row in l_dtTemp.Rows)
                        {
                            str_station_name = row.Field<string>("station_name");
                            str_ip = row.Field<string>("ip");
                            strdiff_time = int.Parse(row["diff_time"].ToString());

                            CMCControl gw28model = CMCManager.GetInstance().GetCMCByIP(str_ip);
                            str_station_name =gw28model.StationName;

                            string stencil_location = Getstencillocation(row.Field<string>("stencil_sn"));

                            if (gw28model.State == MyStateSocket.sckConnected)
                            {
                                gw28model.SendAlarm(myid, str_ip, "Khuon thiec/dao tren may: " + str_station_name + "  da offline qua 10p (" + strdiff_time + "p) ma chua online khuon khac thay the. May se bi dung", "", 0);
                                strsql1 = "SELECT * FROM MES4.R_PROGRAM_LOG WHERE PRG_NAME='SCADA' AND FUN_NAME='stencil_Wash_30Minutes_no_online' AND ACTION_TYPE='" + str_station_name + "' AND OLDSN='" + row.Field<string>("stencil_sn") + "' AND DATA1=0 AND DATA2<='" + strdiff_time + "'";

                                l_dtTemp1 = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                if (l_dtTemp1.Rows.Count == 0)
                                {
                                    strsql = "Insert into MES4.R_PROGRAM_LOG(PRG_NAME, FUN_NAME, ACTION_TYPE, OLDSN,NEWSN,DATA1,DATA2,TIME) " + "Values('SCADA', 'stencil_Wash_30Minutes_no_online', '" + str_station_name + "', '" + row.Field<string>("stencil_sn") + "','" + str_ip + "','0','" + strdiff_time + "', SYSDATE)";
                                    OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                }
                                else
                                {
                                    strsql = "UPDATE MES4.R_PROGRAM_LOG SET DATA2='" + strdiff_time + "' WHERE PRG_NAME='SCADA' AND FUN_NAME='stencil_Wash_30Minutes_no_online' AND ACTION_TYPE='" + str_station_name + "' AND OLDSN='" + row.Field<string>("stencil_sn") + "' AND DATA1=0 AND DATA2<='" + strdiff_time + "'";
                                    OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                }
                                l_dtTemp1.Rows.Clear();
                                l_dtTemp1.Dispose();

                                if (HSCADAFLAG.G_StopMachineNeedOnlineStencilAfterOffline10Mins)
                                {
                                    gw28model.SendAlarm1(myid, str_ip, "Khuon thiec/dao tren may: " + str_station_name + "  da offline qua 10p (" + strdiff_time + "p) ma chua online khuon khac thay the. May se bi dung", "", 0);
                                    if (check_machine_type(str_station_name, str_ip) == "PANASONIC")
                                        lock_machine_panasonic(str_station_name, str_ip);
                                    strMessage = DateTime.Now + Constants.vbCrLf + gw28model.StationName + "-Receive DATA09: Khuon thiec/dao tren may: " + str_station_name + "  da offline qua 10p (" + strdiff_time + "p) ma chua online khuon khac thay the. May se bi dung";
                                    SimpleLogger.Writeback(this.ScadaName, gw28model.StationName, DateTime.Now.ToString("yyyyMMdd"), strMessage);
                                }
                            }
                        }
                    }
                    // -------------------------------------------END the other stencil need on_line in 10 minutes after one stencil off-line :alarm&pause----------------------------------------------------------------------
                    // -------------------------------------------solder have 24 hour have no offline STOP MACHINE modify by Marcus 20131011----------------------------------------------------------------------------
                    if (HSCADAFLAG.G_CMaterialconfirmFlag == true)
                    {
                        strsql = " SELECT aa.station_name, aa.ip, bb.tr_sn,ROUND(bb.timelong * 24 * 60,2) diff_time,bb.work_time " +
                                "  FROM mes1.c_gw28_config aa, " +
                                " (SELECT c.*, d.work_time,(sysdate- d.work_time) timelong FROM mes4.r_tr_sn_wip d, " +
                                "  (SELECT a.station, a.tr_sn, a.kp_no FROM mes4.r_station_wip a, mes1.c_solder_base b " +
                                "           WHERE a.kp_no = b.kp_no AND tr_sn IS NOT NULL) c " +
                                "            WHERE c.tr_sn = d.tr_sn " +
                                "              AND d.work_time IS NOT NULL " +
                                "                AND (SYSDATE - d.work_time) > 20/24 ) bb " +
                                "          WHERE aa.station_name = bb.station and aa.SCADA_ID='" + this.ScadaID + "'";

                        l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                        foreach (DataRow row in l_dtTemp.Rows)
                        {
                            str_station_name = row.Field<string>("station_name");
                            str_ip = row.Field<string>("ip");
                            strdiff_time = (int)Math.Round(row.Field<decimal>("diff_time"), 0);


                            CMCControl gw28model = CMCManager.GetInstance().GetCMCByIP(str_ip);
                            str_station_name =gw28model.StationName;

                            if (gw28model.State == MyStateSocket.sckConnected)
                            {
                                gw28model.SendAlarm(myid, str_ip, "Kem thiec:" + row.Field<string>("TR_SN") + " len chuyen da qua 20h (" + strdiff_time + "p) chua xuong chuyen bao phe. May se bi dung", "", 0);
                                if (HSCADAFLAG.G_StopMachineSolder24hNoOffline)
                                {
                                    gw28model.SendAlarm(myid, str_ip, "Kem thiec:" + row.Field<string>("TR_SN") + " len chuyen da qua 20h (" + strdiff_time + "p) chua xuong chuyen bao phe. May se bi dung", "", 0);
                                    if (check_machine_type(str_station_name, str_ip) == "PANASONIC")
                                        lock_machine_panasonic(str_station_name, str_ip);

                                    strsql1 = "SELECT * FROM MES4.R_PROGRAM_LOG WHERE PRG_NAME='SCADA' AND FUN_NAME='SOLDER 24HOURS HAVE NOT OFFLINE TO STOP MACHINE' AND ACTION_TYPE='" + str_station_name + "' AND OLDSN='" + row.Field<string>("TR_SN") + "' AND DATA1=0 AND DATA2<='" + strdiff_time + "'";
                                    l_dtTemp1 = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                    if (l_dtTemp1.Rows.Count == 0)
                                    {
                                        strsql = "Insert into MES4.R_PROGRAM_LOG(PRG_NAME, FUN_NAME, ACTION_TYPE, OLDSN,NEWSN,DATA1,DATA2,TIME) " + "Values('SCADA', 'SOLDER 24HOURS HAVE NOT OFFLINE TO STOP MACHINE', '" + str_station_name + "', '" + row.Field<string>("TR_SN") + "','" + str_ip + "','0','" + strdiff_time + "', SYSDATE)";
                                        OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                    }
                                    else
                                    {
                                        strsql = "UPDATE MES4.R_PROGRAM_LOG SET DATA2='" + strdiff_time + "' WHERE PRG_NAME='SCADA' AND FUN_NAME='SOLDER 24HOURS HAVE NOT OFFLINE TO STOP MACHINE' AND ACTION_TYPE='" + str_station_name + "' AND OLDSN='" + row.Field<string>("TR_SN") + "' AND DATA1=0 AND DATA2<='" + strdiff_time + "'";
                                        OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                    }
                                    l_dtTemp1.Rows.Clear();
                                    l_dtTemp1.Dispose();

                                    strMessage = DateTime.Now + Constants.vbCrLf + gw28model.StationName + "-Receive SolderCheck: Kem thiec:" + row.Field<string>("TR_SN") + " len truyen da qua 20h (" + strdiff_time + "p) chua xuong truyen bao phe. May se bi dung";
                                    SimpleLogger.Writeback(this.ScadaName, gw28model.StationName, DateTime.Now.ToString("yyyyMMdd"), strMessage);
                                }
                            }
                        }
                    }
                    // -------------------------------------------END solder have 24 hour have no offline STOP MACHINE modify by Marcus 20131011----------------------------------------------------------------------------
                    // -------------------------------------------PCB PASS AUTOSCANNER DUP SCAN OR PCB PNO ERROR modify by Marcus 20141318----------------------------------------------------------------------------
                    if (HSCADAFLAG.G_CMaterialconfirmFlag == true)
                    {
                        strsql = " select  a.station_name station_name,data1,B.IP ip from mes1.c_line_station a , mes1.c_gw28_config b  WHERE substr(a.data1,1,1) in ('2','3') and  a.station_name =b.station_name and b.SCADA_ID='" + this.ScadaID + "'";
                        l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                        foreach (DataRow row in l_dtTemp.Rows)
                        {
                            str_station_name = row.Field<string>("station_name");
                            str_ip = row.Field<string>("ip");
                            var strdiff_time = row.Field<string>("Data1");

                            CMCControl gw28model = CMCManager.GetInstance().GetCMCByIP(str_ip);
                            str_station_name =gw28model.StationName;

                            if (gw28model.State == MyStateSocket.sckConnected)
                            {
                                gw28model.SendAlarm(myid, str_ip, "SN: " + Strings.Right(strdiff_time, 7) + " sao Autoscan qua nhieu lan. May se bi dung", "", 0);
                                if (HSCADAFLAG.G_StopMachinePCBAMultiPassAutoScanner)
                                {
                                    if (Strings.Left(strdiff_time, 1) == "2")
                                    {
                                        gw28model.SendAlarm1(myid, str_ip, "SN: " + Strings.Right(strdiff_time, 7) + " sao Autoscan qua nhieu lan. May se bi dung", "", 0);
                                        if (check_machine_type(str_station_name, str_ip) == "PANASONIC")
                                            lock_machine_panasonic(str_station_name, str_ip);
                                        strMessage = DateTime.Now + Constants.vbCrLf + gw28model.StationName + "-Receive DATA12: SN: " + Strings.Right(strdiff_time, 7) + " sao Autoscan qua nhieu lan. May se bi dung";
                                        SimpleLogger.Writeback(this.ScadaName, gw28model.StationName, DateTime.Now.ToString("yyyyMMdd"), strMessage);

                                    }
                                    else if (Strings.Left(strdiff_time, 1) == "3")
                                    {
                                        gw28model.SendAlarm1(myid, str_ip, "SN: " + strdiff_time + " Ban mach su dung sai. may se bi dung", "", 0);
                                        if (check_machine_type(str_station_name, str_ip) == "PANASONIC")
                                            lock_machine_panasonic(str_station_name, str_ip);
                                        strMessage = DateTime.Now + Constants.vbCrLf + gw28model.StationName + "-Receive PcbAutoScanCheck: SN: " + strdiff_time + " Ban mach su dung sai. may se bi dung";
                                        SimpleLogger.Writeback(this.ScadaName, gw28model.StationName, DateTime.Now.ToString("yyyyMMdd"), strMessage);
                                    }
                                }
                            }
                        }
                    }
                    // -------------------------------------------END PCB PASS AUTOSCANNER DUP SCAN OR PCB PNO ERROR modify by Marcus 20141318----------------------------------------------------------------------------
                    // -------------------------------------------PCB INPUT THE ERROR LINE THEN LOCK MACHINE H1,modify by Marcus 20140817----------------------------------------------------------------------------
                    if (HSCADAFLAG.G_PCBA_ERROR_LINE_Flag == true)
                    {
                        strsql = " SELECT distinct a.EMP_NO station_name, b.ip ip,b.station_name,a.action_desc " + " FROM mes4.r_system_log a, mes1.c_gw28_config b Where A.EMP_NO = b.line_name and b.SCADA_ID='" + this.ScadaID + "' " + " AND A.prg_name = 'SP:MES1.CHECK_PSN_MATERIAL:ERROR_PROCESS_FLAG' " + " AND b.station_name = a.emp_no||'AH1' ";
                        l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                        foreach (DataRow row in l_dtTemp.Rows)
                        {
                            str_station_name = row.Field<string>("station_name");
                            str_ip = row.Field<string>("ip");
                            stringsn = row.Field<string>("action_desc");

                            CMCControl gw28model = CMCManager.GetInstance().GetCMCByIP(str_ip);
                            str_station_name =gw28model.StationName;

                            if (gw28model.State == MyStateSocket.sckConnected)
                            {
                                gw28model.SendAlarm(myid, str_ip, "SN:" + Strings.Right(stringsn, 7) + " sao sai mat. May se bi dung", "", 0);
                                if (HSCADAFLAG.G_PCBA_ERROR_LINE_Flag)
                                {
                                    gw28model.SendAlarm1(myid, str_ip, "SN:" + Strings.Right(stringsn, 7) + " sao sai mat. May se bi dung", "", 0);
                                    if (check_machine_type(str_station_name, str_ip) == "PANASONIC")
                                        lock_machine_panasonic(str_station_name, str_ip);
                                    strMessage = DateTime.Now + Constants.vbCrLf + gw28model.StationName + "-Receive DATA12: SN:" + Strings.Right(stringsn, 7) + " sao sai mat. May se bi dung";
                                    SimpleLogger.Writeback(this.ScadaName, gw28model.StationName, DateTime.Now.ToString("yyyyMMdd"), strMessage);
                                    strsql1 = " delete from mes4.r_system_log WHERE prg_name = 'SP:MES1.CHECK_PSN_MATERIAL:ERROR_PROCESS_FLAG' and action_desc ='" + stringsn + "' ";
                                    OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql1);
                                }
                            }
                        }
                    }

                    // -------------------------------------------END PCB INPUT THE ERROR LINE THEN LOCK MACHINE H1,modify by Marcus 20140817----------------------------------------------------------------------------
                    // -------------------------------------------CHECK LEVER CODE OVERTIME HAS ONLINE BY LUONGVD007------------------------------
                    if (HSCADAFLAG.G_StopMachineCheckMSDOvertime)
                    {
                        strsql = "SELECT TR_SN,kp_no,STATION_NAME,msd_desc,msd_type,MAX(DIFF_TIME) diff_time,IP FROM(SELECT   *" +
                                " FROM (SELECT DISTINCT (m.tr_sn),m.kp_no,q.msd_desc," +
                                " CEIL ((SYSDATE - m.work_time) * 24  + R.TOTAL_TIME) diff_time,q.msd_type," +
                                " n.station_name , n.ip " +
                                " FROM mes4.r_tr_sn_wip m, " +
                                " mes1.c_gw28_config n, " +
                                " mes4.r_station_wip l, " +
                                " mes4.r_tr_sn o, " +
                                " mes1.c_smt_ap_list k, " +
                                " mes1.c_msd_kp_config q, " +
                                " mes4.r_msd_kp_detail r " +
                                " Where n.SCADA_ID='" + this.ScadaID +
                                "'and m.station_flag = '1' AND m.work_flag=1 AND (CEIL ((SYSDATE - m.work_time) * 24)+ R.TOTAL_TIME) >= q.msd_desc-4 " +
                                " AND NOT EXISTS (SELECT 1 FROM MES4.r_tr_sn_wip WHERE M.STATION=STATION AND SUBSTR (m.station, 6, 2) ='CT') " +
                                " AND o.tr_sn = m.tr_sn AND l.tr_sn=m.tr_sn AND m.station = n.station_name " +
                                " AND m.wo = l.wo AND m.station = l.station and r.location_flag='2'" +
                                " AND l.smt_code = k.smt_code AND m.kp_no = l.KP_NO " +
                                " AND m.kp_no = k.kp_no AND q.msd_kp_no=m.kp_no and r.tr_sn=m.tr_sn " +
                                " AND q.msd_type>='2' AND l.smt_code IS NOT NULL and Q.MSD_DESC is not null " +
                                " AND NOT EXISTS(SELECT 1 FROM mes4.r_station_wip WHERE L.FEEDER_TYPE = FEEDER_TYPE AND FEEDER_TYPE = 'TRAY')) " +
                                " ORDER BY station_name, ip, tr_sn) group by TR_SN,kp_no,STATION_NAME,msd_desc,msd_type,IP";

                        l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                        foreach (DataRow row in l_dtTemp.Rows)
                        {
                            str_tr_sn = row.Field<string>("station_name");
                            str_ip = row.Field<string>("ip");
                            strdiff_time = int.Parse(row["diff_time"].ToString());
                            msd_type = row.Field<string>("msd_type");


                            CMCControl gw28model = CMCManager.GetInstance().GetCMCByIP(str_ip);
                            str_station_name =gw28model.StationName;

                            if (gw28model.State == MyStateSocket.sckConnected)
                            {
                                gw28model.SendAlarm1(myid, str_ip, str_station_name + ": " + str_tr_sn + Strings.Chr(58) + " voi quan che MSD " + msd_type + ", Da qua thoi gian " + row.Field<string>("msd_desc") + "p (" + strdiff_time + "p). May se bi dung", "", 0);
                                if (check_machine_type(str_station_name, str_ip) == "PANASONIC")
                                    lock_machine_panasonic(str_station_name, str_ip);
                                strsql = "select * from mes4.r_station_wip where tr_sn ='" + str_tr_sn + "' and station='" + str_station_name + "'";
                                l_dtTemp1 = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                if (l_dtTemp1.Rows.Count == 0)
                                {
                                    strsql = "update mes4.r_tr_Sn set data5='MSD OVERTIIME' where tr_sn ='" + str_tr_sn + "'";
                                    OracleHelper.ExecuteNonQuery(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                                }
                                // Call Sendmail("SMT", "TR_SN:" & str_tr_sn & " Lever " & msd_type & " Has online station  " & str_station_name & " Overtime  " & strdiff_time & " ,Machine will Shutdown!")
                                strMessage = DateTime.Now + Constants.vbCrLf + str_station_name + "(" + gw28model.IP + ")-Receive MsdOvertime:" + str_station_name + ": " + str_tr_sn + Strings.Chr(58) + " voi quan che MSD " + msd_type + ", Da qua thoi gian " + row.Field<string>("msd_desc") + "p (" + strdiff_time + "p). May se bi dung";
                                SimpleLogger.Writeback(this.ScadaName, gw28model.StationName, DateTime.Now.ToString("yyyyMMdd"), strMessage);
                                l_dtTemp1.Rows.Clear();
                                l_dtTemp1.Dispose();
                            }
                        }
                    }
                    // -------------------------------------------END CHECK LEVER CODE OVERTIME HAS ONLINE BY LUONGVD007------------------------------

                    if (l_dtTemp != null)
                        l_dtTemp.Dispose();
                    if (l_dtTemp1 != null)
                        l_dtTemp1.Dispose();
                }
            }
            catch (Exception ex2)
            {
                string mzFolder2 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string strLogFile2 = Path.Combine(mzFolder2, "log_file.log");
                string log = "--> DEBUG VARIABLE : " + strsql + Environment.NewLine;
                File.AppendAllText(strLogFile2, string.Format("[E] {0}:{1} - {2} {3}", new object[]
                {
                                        MethodBase.GetCurrentMethod(),
                                        ex2.ToString() + log,
                                        DateTime.Now.ToString(),
                                        Environment.NewLine
                }));
            }
        }

        private string Getstencillocation(string stencilsn)
        {
            string strsql = "SELECT location from mes1.c_stencil_base WHERE stencil_sn ='" + stencilsn + "'";
            DataTable l_orard = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
            if (l_orard.Rows.Count > 0)
                return l_orard.Rows[0]["location"].ToString();
            else return "";
        }

        private void lock_machine_panasonic(string station, string ip)
        {
            string strsql, file_stt, copy_from, copy_to, strMessage;
            DataTable l_dtTemp;
            strsql = "SELECT DISTINCT ICP_DO FILE_STT FROM MES1.C_GW28_CONFIG WHERE STATION_NAME='" + Strings.Trim(Conversions.ToString(station)) + "' AND IP='" + Strings.Trim(Conversions.ToString(ip)) + "' AND EDIT_EMP='PANASONIC' AND ROWNUM=1";
            l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
            if (l_dtTemp.Rows.Count > 0)
            {
                file_stt = Strings.Trim(l_dtTemp.Rows[0].Field<string>("file_stt"));
                copy_from = Application.StartupPath + @"\Release\NPM0" + file_stt + ".LK";
                if (!string.IsNullOrEmpty(FileSystem.Dir(copy_from, Constants.vbDirectory)))
                {
                    strsql = "SELECT DISTINCT ICP_IP PATH FROM MES1.C_SCADA_ICP WHERE ICP_ID='" + Strings.Left(Strings.Trim(Conversions.ToString(station)), 5) + "' AND ROWNUM=1";
                    l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                    if (l_dtTemp.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(FileSystem.Dir(Strings.Trim(l_dtTemp.Rows[0].Field<string>("Path")), Constants.vbDirectory)))
                        {
                            copy_to = Strings.Trim(l_dtTemp.Rows[0].Field<string>("Path")) + "NPM0" + file_stt + ".LK";
                            if (string.IsNullOrEmpty(FileSystem.Dir(copy_to, Constants.vbDirectory)))
                            {
                                Microsoft.VisualBasic.FileIO.FileSystem.CopyFile(copy_from, copy_to);
                            }
                        }
                        else
                        {
                            strMessage = DateAndTime.Now + Constants.vbCrLf + Strings.Trim(Conversions.ToString(station)) + " (" + Strings.Trim(Conversions.ToString(ip)) + ") - This path: " + Strings.Trim(l_dtTemp.Rows[0].Field<string>("Path")) + " not exist. Can't unlock machine!";
                            SimpleLogger.Writeback(this.ScadaName, station, DateAndTime.Now.ToString("yyyyMMdd"), strMessage);
                        }
                    }
                    else
                    {
                        strMessage = DateAndTime.Now + Constants.vbCrLf + Strings.Trim(Conversions.ToString(station)) + " (" + Strings.Trim(Conversions.ToString(ip)) + ") - This station not exist in table MES1.C_SCADA_ICP. Can't unlock machine!";
                        SimpleLogger.Writeback(this.ScadaName, station, DateAndTime.Now.ToString("yyyyMMdd"), strMessage);
                    }
                }
                else
                {
                    strMessage = DateAndTime.Now + Constants.vbCrLf + Strings.Trim(Conversions.ToString(station)) + " (" + Strings.Trim(Conversions.ToString(ip)) + ") - File NPM0" + file_stt + ".LR not exist in path " + Application.StartupPath + @"\Release. Can't unlock machine!";
                    SimpleLogger.Writeback(this.ScadaName, station, DateAndTime.Now.ToString("yyyyMMdd"), strMessage);
                }
            }
            else
            {
                strMessage = DateAndTime.Now + Constants.vbCrLf + Strings.Trim(Conversions.ToString(station)) + " (" + Strings.Trim(Conversions.ToString(ip)) + ") - This station not exist in table MES1.C_GW28_CONFIG. Can't unlock machine!";
                SimpleLogger.Writeback(this.ScadaName, station, DateAndTime.Now.ToString("yyyyMMdd"), strMessage);
            }

            l_dtTemp.Rows.Clear();
            l_dtTemp.Dispose();

            return;
        }

        private void unlock_machine_panasonic(string station, string ip)
        {
            string strsql, file_stt, copy_from, copy_to, strMessage;
            DataTable l_dtTemp;
            strsql = "SELECT DISTINCT ICP_DO FILE_STT FROM MES1.C_GW28_CONFIG WHERE STATION_NAME='" + Strings.Trim(Conversions.ToString(station)) + "' AND IP='" + Strings.Trim(Conversions.ToString(ip)) + "' AND EDIT_EMP='PANASONIC' AND ROWNUM=1";
            l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
            if (l_dtTemp.Rows.Count > 0)
            {
                file_stt = Strings.Trim(l_dtTemp.Rows[0].Field<string>("file_stt"));
                copy_from = Application.StartupPath + @"\Release\NPM0" + file_stt + ".LR";
                if (!string.IsNullOrEmpty(FileSystem.Dir(copy_from, Constants.vbDirectory)))
                {
                    strsql = "SELECT DISTINCT ICP_IP PATH FROM MES1.C_SCADA_ICP WHERE ICP_ID='" + Strings.Left(Strings.Trim(Conversions.ToString(station)), 5) + "' AND ROWNUM=1";
                    l_dtTemp = OracleHelper.ExecuteDataTable(HCONSTANT.ORACLE_CONNECTION_STRING_DB, CommandType.Text, strsql);
                    if (l_dtTemp.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(FileSystem.Dir(Strings.Trim(l_dtTemp.Rows[0].Field<string>("Path")), Constants.vbDirectory)))
                        {
                            copy_to = Strings.Trim(l_dtTemp.Rows[0].Field<string>("Path")) + "NPM0" + file_stt + ".LR";
                            if (string.IsNullOrEmpty(FileSystem.Dir(copy_to, Constants.vbDirectory)))
                            {
                                Microsoft.VisualBasic.FileIO.FileSystem.CopyFile(copy_from, copy_to);
                            }
                        }
                        else
                        {
                            strMessage = DateAndTime.Now + Constants.vbCrLf + Strings.Trim(Conversions.ToString(station)) + " (" + Strings.Trim(Conversions.ToString(ip)) + ") - This path: " + Strings.Trim(l_dtTemp.Rows[0].Field<string>("Path")) + " not exist. Can't unlock machine!";
                            SimpleLogger.Writeback(this.ScadaName, station, DateAndTime.Now.ToString("yyyyMMdd"), strMessage);
                        }
                    }
                    else
                    {
                        strMessage = DateAndTime.Now + Constants.vbCrLf + Strings.Trim(Conversions.ToString(station)) + " (" + Strings.Trim(Conversions.ToString(ip)) + ") - This station not exist in table MES1.C_SCADA_ICP. Can't unlock machine!";
                        SimpleLogger.Writeback(this.ScadaName, station, DateAndTime.Now.ToString("yyyyMMdd"), strMessage);
                    }
                }
                else
                {
                    strMessage = DateAndTime.Now + Constants.vbCrLf + Strings.Trim(Conversions.ToString(station)) + " (" + Strings.Trim(Conversions.ToString(ip)) + ") - File NPM0" + file_stt + ".LR not exist in path " + Application.StartupPath + @"\Release. Can't unlock machine!";
                    SimpleLogger.Writeback(this.ScadaName, station, DateAndTime.Now.ToString("yyyyMMdd"), strMessage);
                }
            }
            else
            {
                strMessage = DateAndTime.Now + Constants.vbCrLf + Strings.Trim(Conversions.ToString(station)) + " (" + Strings.Trim(Conversions.ToString(ip)) + ") - This station not exist in table MES1.C_GW28_CONFIG. Can't unlock machine!";
                SimpleLogger.Writeback(this.ScadaName, station, DateAndTime.Now.ToString("yyyyMMdd"), strMessage);
            }

            l_dtTemp.Rows.Clear();
            l_dtTemp.Dispose();

            return;
        }

    }
}
