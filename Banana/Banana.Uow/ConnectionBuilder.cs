/***********************************
 * Developer: Lio.Huang
 * Create Date：2018-11-16
 * 
 * Last Update：
 * 2019-01-07  1. GetAdapter(connection)
 * 2019-01-21  1.增加同时多数据库支持
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
    /// 创建基础链接|
    /// Creating database links
    /// </summary>
    public class ConnectionBuilder
    {
        private static ConcurrentDictionary<string, DBSetting> DBSettingDic;
        /// <summary>
        /// Default key Name
        /// </summary>
        public const string DefaultKeyName = "Banana's read&wirte db connection key";

        /// <summary>
        /// 注册链接|
        /// Register database links
        /// </summary>
        /// <param name="strConn">connection string</param>
        /// <param name="dBType">type of database</param>
        /// <param name="dbKey">Multiple databases can be injected depending on the key</param>
        public static void ConfigRegist(string strConn, DBType dBType = DBType.SqlServer, string dbKey = DefaultKeyName)
        {
           var dbSetting = new DBSetting() { ConnectionString = strConn, DBType = dBType };
            ConfigRegist(dbSetting, dbKey);
        }

        /// <summary>
        /// 注册链接|
        /// Register database links
        /// </summary>
        /// <param name="db">connection model</param>
        /// <param name="dbKey">Multiple databases can be injected depending on the key</param>
        public static void ConfigRegist(DBSetting db, string dbKey = DefaultKeyName)
        {
            if (DBSettingDic == null)
            {
                DBSettingDic = new ConcurrentDictionary<string, DBSetting>();
            }
            if (string.IsNullOrEmpty(dbKey))
            {
                dbKey = DefaultKeyName;
            }
            if (DBSettingDic.ContainsKey(dbKey))
            {
                throw new Exception("The same key already exists:" + dbKey);
            }
            DBSettingDic[dbKey] = db;
        }

        /// <summary>
        /// 创建连接串|
        /// create database connection
        /// </summary>
        public static IDbConnection CreateConnection(string dbKey = DefaultKeyName)
        {
            try
            {
                if (string.IsNullOrEmpty(dbKey))
                {
                    dbKey = DefaultKeyName;
                }
                DBSetting dBSetting;
                if (!DBSettingDic.TryGetValue(dbKey, out dBSetting))
                {
                    throw new Exception("The key doesn't exist:" + dbKey);
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

        public static DBSetting GetDBSetting(string dbKey = DefaultKeyName)
        {
            if (string.IsNullOrEmpty(dbKey))
                dbKey = DefaultKeyName;
            DBSetting dBSetting;
            if (!DBSettingDic.TryGetValue(dbKey, out dBSetting))
            {
                throw new Exception("The key doesn't exist:" + dbKey);
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
