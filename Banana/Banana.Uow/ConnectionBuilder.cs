/***********************************
 * Developer: Lio.Huang
 * Create Date：2018-11-16
 * 
 * Last Update：
 * 2019-01-07  1. GetAdapter(connection)
 * 2019-01-21  1.增加同时多数据库支持
 * 2019-02-11  1.rename dbkey 
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
using System.Collections.Concurrent;

namespace Banana.Uow
{
    /// <summary>
    /// 创建基础链接| Creating database links
    /// </summary>
    public class ConnectionBuilder
    {
        private static ConcurrentDictionary<string, DBSetting> DBSettingDic;
        /// <summary>
        /// Default key Name
        /// </summary>
        public const string DefaultAliase = "Banana-ORM";

        /// <summary>
        /// 注册链接|Register database links
        /// </summary>
        /// <param name="strConn">connection string</param>
        /// <param name="dBType">type of database</param>
        /// <param name="dbAliase">Multiple databases can be injected depending on the key</param>
        public static void ConfigRegist(string strConn, DBType dBType = DBType.SqlServer, string dbAliase = DefaultAliase)
        {
           var dbSetting = new DBSetting() { ConnectionString = strConn, DBType = dBType };
            ConfigRegist(dbSetting, dbAliase);
        }

        /// <summary>
        /// 注册链接|Register database links
        /// </summary>
        /// <param name="db">connection model</param>
        /// <param name="dbAliase">Multiple databases can be injected depending on the key</param>
        public static void ConfigRegist(DBSetting db, string dbAliase = DefaultAliase)
        {
            if (DBSettingDic == null)
            {
                DBSettingDic = new ConcurrentDictionary<string, DBSetting>();
            }
            if (string.IsNullOrEmpty(dbAliase))
            {
                dbAliase = DefaultAliase;
            }
            if (DBSettingDic.ContainsKey(dbAliase))
            {
                throw new Exception("The same key already exists:" + dbAliase);
            }
            DBSettingDic[dbAliase] = db;
        }

        /// <summary>
        /// 创建连接串|Create database connection
        /// </summary>
        public static IDbConnection CreateConnection(string dbAliase = DefaultAliase)
        {
            try
            {
                if (string.IsNullOrEmpty(dbAliase))
                {
                    dbAliase = DefaultAliase;
                }
                DBSetting dBSetting;
                if (!DBSettingDic.TryGetValue(dbAliase, out dBSetting))
                {
                    throw new Exception("The key doesn't exist:" + dbAliase);
                }
                var conn = dBSetting.ConnectionString;
                switch (dBSetting.DBType)
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

                throw new Exception("Unregistered database link, please register by \"ConnectionBuilder.ConfigRegist\"");
            }
            catch (Exception ex)
            {
                throw new Exception("Unregistered database link, please register by \"ConnectionBuilder.ConfigRegist\"");
            }
        }

        /// <summary>
        /// Get DB Option
        /// </summary>
        /// <param name="dbAliase">DB alias</param>
        /// <returns></returns>
        public static DBSetting GetDBSetting(string dbAliase = DefaultAliase)
        {
            if (string.IsNullOrEmpty(dbAliase))
                dbAliase = DefaultAliase;
            DBSetting dBSetting;
            if (!DBSettingDic.TryGetValue(dbAliase, out dBSetting))
            {
                throw new Exception("The key doesn't exist:" + dbAliase);
            }
            return dBSetting;
        }
        /// <summary>
        /// The interface for all Dapper database operations 
        /// </summary>
        /// <returns></returns>
        internal static ISqlAdapter GetAdapter(IDbConnection connection)
        {
            return GetFormatter(connection);
        }
    }
}
