/***********************************
 * Developer: Lio.Huang
 * Create Date：2018-11-16
 * 
 * Last Update：
 * 2019-01-07  1. GetAdapter(connection)
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
    /// 创建基础链接|
    /// Creating database links
    /// </summary>
    public class ConnectionBuilder
    {
        internal static DBSetting DBSetting;

        /// <summary>
        /// 注册链接|
        /// Register database links
        /// </summary>
        /// <param name="strConn">connection string</param>
        /// <param name="dBType">type of database</param>
        public static void ConfigRegist(string strConn, DBType dBType = DBType.SqlServer)
        {
            DBSetting = new DBSetting() { ConnectionString = strConn, DBType = dBType };
        }

        /// <summary>
        /// 注册链接|
        /// Register database links
        /// </summary>
        /// <param name="db">connection model</param>
        public static void ConfigRegist(DBSetting db)
        {
            DBSetting = new DBSetting() { ConnectionString = db.ConnectionString, DBType = db.DBType };
        }

        /// <summary>
        /// 创建连接串|
        /// create database connection
        /// </summary>
        public static IDbConnection CreateConnection()
        {
            try
            {
                var conn = DBSetting.ConnectionString;
                switch (DBSetting.DBType)
                {
                    case DBType.SqlServer:
                    case DBType.SqlServer2012:
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

        /// <summary>
        /// The interface for all Dapper.Contrib database operations 
        /// </summary>
        /// <returns></returns>
        internal static ISqlAdapter GetAdapter(IDbConnection connection)
        {
            return GetFormatter(connection);
        }
    }
}
