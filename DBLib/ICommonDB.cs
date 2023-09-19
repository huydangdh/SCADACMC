using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;
/**
 * @author V0940534
 * */
namespace DBLib
{
    public interface ICommonDB
    {
        void OpenConnection();
        void CloseConnection();
        void BeginTransaction();
        void CommitTransaction();
        void RollBackTransaction();
        int ExecuteNonQuery(string strSQL);
        OracleDataReader ExecuteDataReader(string strSQL);
        int ExecuteCommitSQL(string strSQL);
        DataTable ExecuteDataTable(string strSQL);
        DataTable ExecuteDataTableWithParams(string sSql, CommandType eCmdType, OracleParameter[] arrOraParameter);
        OracleDataReader ExecuteProcedure(string sProc, CommandType commandType, OracleParameter[] oraParameters);
        void FillDataTable(string strSQL, ref DataTable dtTemp);
        object ExecuteScalar(string strSQL);

    }
}
