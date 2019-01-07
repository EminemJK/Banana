/***********************************
 * Developer: Lio.Huang
 * Date：2018-11-16
 * 
 * Last Update：2018-12-18
 * 2019-01-07  1.Add SqlServer2012 DBType
 **********************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Banana.Uow.Models
{
    /// <summary>
    /// 数据库类型|type of database
    /// </summary>
    public enum DBType
    {
        /// <summary>
        /// SqlServer 2005、2008 universal low versions
        /// </summary>
        SqlServer = 0,

        /// <summary>
        /// SqlServer 2012 High version
        /// </summary>
        SqlServer2012 = 1,

        /// <summary>
        /// MySQL
        /// </summary>
        MySQL = 2,

        /// <summary>
        /// SQLite
        /// </summary>
        SQLite = 3,

        /// <summary>
        /// PostgreSQL
        /// </summary>
        Postgres = 4,

        /// <summary>
        /// Oracle
        /// </summary>
        Oracle = 5
    }
}
