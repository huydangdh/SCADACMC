using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Threading;

/**
 * @author V0940534
 * 
 * */
namespace DBLib
{

    public sealed class OracleHelper
    {
        /// <summary>
        /// CTOR
        /// </summary>
        private OracleHelper()
        { }

        private static void PrepareCommand(OracleCommand cmd, OracleConnection connection, OracleTransaction oracleTransaction, CommandType cmdType, string cmdText, OracleParameter[] cmdParams)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            cmd.Connection = connection;
            cmd.CommandType = cmdType;
            cmd.CommandText = cmdText;
            if (oracleTransaction != null)
            {
                cmd.Transaction = oracleTransaction;
            }
            if (cmdParams != null)
            {
                AttachParameters(cmd, cmdParams);
            }
        }
        private static void AttachParameters(OracleCommand cmd, OracleParameter[] cmdParams)
        {
            foreach (OracleParameter param in cmdParams)
            {
                if (param.Direction == ParameterDirection.InputOutput & param.Value == null)
                {
                    param.Value = DBNull.Value;
                }

                cmd.Parameters.Add(param);
            }

        }
        public static DataTable ExecuteDataTable(OracleConnection connection, CommandType cmdType, string cmdText, params OracleParameter[] cmdParams)
        {
            using (OracleCommand cmd = new OracleCommand())
            {
                PrepareCommand(cmd, connection, (OracleTransaction)null, cmdType, cmdText, cmdParams);

                OracleDataAdapter oraDa = new OracleDataAdapter(cmd);
                DataTable dt = new DataTable();
                oraDa.Fill(dt);

                return dt;
            }
        }
        /// <summary>
        /// Truy van return DataTable
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParams"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string connectionString, CommandType cmdType, string cmdText, params OracleParameter[] cmdParams)
        {
            using (OracleConnection oraConn = new OracleConnection(connectionString))
            {
                return ExecuteDataTable(oraConn, cmdType, cmdText, cmdParams);
            }
        }
        public static object ExecuteScalar(OracleConnection connString, CommandType cmdType, string cmdText, params OracleParameter[] cmdParams)
        {
            using (OracleCommand cmd = new OracleCommand())
            {
                PrepareCommand(cmd, connString, null, cmdType, cmdText, cmdParams);
                try
                {
                    return cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    cmd.Dispose();
                    throw ex;
                }
            }
        }
        public static object ExecuteScalar(string connString, CommandType cmdType, string cmdText, params OracleParameter[] cmdParams)
        {
            using (OracleConnection oraConn = new OracleConnection(connString))
            {
                return ExecuteScalar(oraConn, cmdType, cmdText, cmdParams);
            }
        }
        public static int ExecuteNonQuery(OracleConnection connString, CommandType cmdType, string cmdText, params OracleParameter[] cmdParams)
        {
            using (OracleCommand cmd = new OracleCommand())
            {
                PrepareCommand(cmd, connString, null, cmdType, cmdText, cmdParams);
                return cmd.ExecuteNonQuery();
            }
        }
        public static int ExecuteNonQuery(string connString, CommandType cmdType, string cmdText, params OracleParameter[] cmdParams)
        {
            using (OracleConnection oraConn = new OracleConnection(connString))
            {
                return ExecuteNonQuery(oraConn, cmdType, cmdText, cmdParams);
            }
        }
        public static int ExecuteNonQuery(OracleConnection connString, OracleTransaction oraTrans, CommandType cmdType, string cmdText, params OracleParameter[] cmdParams)
        {
            using (OracleCommand cmd = new OracleCommand())
            {
                PrepareCommand(cmd, connString, oraTrans, cmdType, cmdText, cmdParams);
                return cmd.ExecuteNonQuery();
            }
        }
        public static int ExecuteNonQuery(OracleTransaction oraTrans, CommandType cmdType, string cmdText, params OracleParameter[] cmdParams)
        {
            return ExecuteNonQuery(oraTrans.Connection, oraTrans, cmdType, cmdText, cmdParams);
        }
        public static OracleDataReader ExecuteDataReader(OracleConnection connection, CommandType cmdType, string cmdText, params OracleParameter[] cmdParams)
        {
            using (OracleCommand cmd = new OracleCommand())
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, cmdParams);
                return cmd.ExecuteReader();
            }
        }
        public static OracleDataReader ExecuteDataReader(string connString, CommandType cmdType, string cmdText, params OracleParameter[] cmdParams)
        {
            OracleConnection oraConn = new OracleConnection(connString);
            return ExecuteDataReader(oraConn, cmdType, cmdText, cmdParams);

        }
    }
}
