/***********************************
 * Coder：EminemJK
 * Date：2018-11-16
 **********************************/


using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 数据库链接实体
/// </summary>
namespace Banana.Uow.Models
{
    public class DBSetting
    {
        /// <summary>
        /// 数据库链接串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DBType DBType { get; set; }
    }
}
