/***********************************
 * Coder：EminemJK
 * Create Date：2018-11-16
 * 
 * Last Update：2018-12-18
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
        private static DBSetting dbSetting;

        /// <summary>
        /// 注册链接|
        /// Register database links
        /// </summary>
        /// <param name="strConn">connection string</param>
        /// <param name="dBType">type of database</param>
        public static void ConfigRegist(string strConn, DBType dBType = DBType.SqlServer)
        {
            dbSetting = new DBSetting() { ConnectionString = strConn, DBType = dBType };
        }

        /// <summary>
        /// 注册链接|
        /// Register database links
        /// </summary>
        /// <param name="db">connection model</param>
        public static void ConfigRegist(DBSetting db)
        {
            dbSetting = new DBSetting() { ConnectionString = db.ConnectionString, DBType = db.DBType };
        }

        /// <summary>
        /// 创建连接串|
        /// create database connection
        /// </summary>
        public static IDbConnection CreateConnection()
        {
            try
            {
                var conn = dbSetting.ConnectionString;
                switch (dbSetting.DBType)
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

        /// <summary>
        /// The interface for all Dapper.Contrib database operations 
        /// </summary>
        /// <returns></returns>
        internal static ISqlAdapter GetAdapter()
        {
            return GetFormatter(CreateConnection());
        }
    }
}
