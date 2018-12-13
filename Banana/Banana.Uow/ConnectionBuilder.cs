/***********************************
 * Coder：EminemJK
 * Date：2018-11-16
 **********************************/
 
using Banana.Uow.Interface;
using Banana.Uow.Models;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using static Banana.Uow.Extension.SqlMapperExtensions;


namespace Banana.Uow
{
    /// <summary>
    /// 创建基础链接
    /// </summary>
    public class ConnectionBuilder
    {
        private static DBSetting dBSetting;

        /// <summary>
        /// 注册链接
        /// </summary>
        /// <param name="strConn">链接串</param>
        /// <param name="dBType">数据库类型</param>
        public static void ConfigRegist(string strConn, DBType dBType = DBType.SqlServer)
        {
            dBSetting = new DBSetting() { ConnectionString = strConn, DBType = dBType };
        }

        /// <summary>
        /// 注册链接
        /// </summary>
        public static void ConfigRegist(DBSetting dBSetting)
        {
            dBSetting = new DBSetting() { ConnectionString = dBSetting.ConnectionString, DBType = dBSetting.DBType };
        }

        /// <summary>
        /// 创建连接串
        /// </summary>
        public static IDbConnection CreateConnection()
        {
            try
            {
                var conn = dBSetting.ConnectionString;
                switch (dBSetting.DBType)
                {
                    case DBType.SqlServer:
                        return new SqlConnection(conn);
                    case DBType.MySQL:
                        return new MySqlConnection(conn);
                    case DBType.SQLite:
                        return new SQLiteConnection(conn);
                    case DBType.Postgres:
                        return new NpgsqlConnection(conn);
                    case DBType.Oracle:
                        return new OracleConnection(conn);
                }
                throw new Exception("未注册数据库链接，请调用ConnectionBuilder.ConfigRegist");
            }
            catch(Exception ex)
            {
                throw new Exception("未注册数据库链接，请调用ConnectionBuilder.ConfigRegist");
            }
        } 

        internal static ISqlAdapter GetAdapter()
        {
            return GetFormatter(CreateConnection());
        }
    }
}
