/***********************************
 * Coder：EminemJK
 * Create Date：2019-01-03
 **********************************/

using System;

namespace Banana.Uow.Models
{
    /// <summary>
    /// 别名|Specifies that this is a column name
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// 别名|column name
        /// </summary>
        /// <param name="columnName"></param>
        public ColumnAttribute(string columnName)
        {
            this.ColumnName = columnName;
        }
        /// <summary>
        /// ColumnName
        /// </summary>
        public string ColumnName { get; set; }
    }
}
