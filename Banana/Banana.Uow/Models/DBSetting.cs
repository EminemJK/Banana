using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Coder：EminemJK 
/// Date：2018-11-16
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

        public DBType DBType { get; set; }
    }
}
