/***********************************
 * Coder：EminemJK
 * Date：2018-11-16
 * 
 * Last Update：2018-12-18
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
        SqlServer = 0,

        MySQL = 1,

        SQLite = 2,

        Postgres = 3,

        Oracle = 4
    }
}
