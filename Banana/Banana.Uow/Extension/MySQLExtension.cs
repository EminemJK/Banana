/***********************************
 * Coder：EminemJK
 * Date：2018-11-20
 **********************************/

using Banana.Uow.Interface;
using Banana.Uow.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Banana.Uow.Extension
{
    /// <summary>
    /// MySQL 扩展
    /// </summary>
    public class MySQLExtension : IAdapter
    {
        public MySQLExtension() { }

        public SqlBuilder GetPageList<T>(IRepository<T> repository, int pageNum = 0, int pageSize = 0, string whereString = null, object param = null, object order = null, bool asc = false) where T : class, IEntity
        {
            SqlBuilder sqlBuilder = new SqlBuilder();
            sqlBuilder.Select(repository.EntityType);
            sqlBuilder.From(repository.TableName);

            if (!string.IsNullOrEmpty(whereString))
            {
                sqlBuilder.Where(whereString, param);
            }
            if (pageNum > 0 && pageSize > 0)
            {
                sqlBuilder.Append($" limit {pageNum},{pageSize}");
            } 
            if (order != null)
            {
                sqlBuilder.OrderBy(order);
                sqlBuilder.IsAse(asc);
            }

            return sqlBuilder;
        }
    }
}
