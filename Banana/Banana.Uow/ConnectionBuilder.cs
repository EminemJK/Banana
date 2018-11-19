/***********************************
 * Coder：EminemJK
 * Date：2018-11-16
 **********************************/

using Banana.Uow.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace Banana.Uow
{
    /// <summary>
    /// 创建基础链接
    /// </summary>
    public class ConnectionBuilder
    {
        internal static DBSetting dBSetting;
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
        public static IDbConnection OpenConnection()
        {
            try
            {
                switch (dBSetting.DBType)
                {
                    case DBType.SqlServer:
                        return new SqlConnection(dBSetting.ConnectionString);
                    case DBType.MySQL:
                        return new MySqlConnection(dBSetting.ConnectionString);
                    case DBType.SQLite:
                        return new SQLiteConnection(dBSetting.ConnectionString);
                }
                throw new Exception("未注册数据库链接，请调用ConnectionBuilder.ConfigRegist");
            }
            catch
            {
                throw new Exception("未注册数据库链接，请调用ConnectionBuilder.ConfigRegist");
            } 
        }


    }
}
