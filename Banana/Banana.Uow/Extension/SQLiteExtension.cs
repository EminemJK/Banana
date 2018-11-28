using Banana.Uow.Interface;
using Banana.Uow.Models;
/***********************************
* Coder：EminemJK
* Date：2018-11-20
**********************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Uow.Extension
{
    /// <summary>
    /// SQLite 扩展
    /// </summary>
    public class SQLiteExtension : IAdapter
    {
        /// <summary>
        /// SQLite 扩展
        public SQLiteExtension() { } 


        public SqlBuilder GetPageList<T>(IRepository<T> repository, int pageNum, int pageSize, string whereString, object param, object order, bool asc)
            where T : class, IEntity
        {
            SqlBuilder sqlBuilder = new SqlBuilder();
            sqlBuilder.Select(repository.EntityType);
            sqlBuilder.From(repository.TableName);

            if (!string.IsNullOrEmpty(whereString))
            {
                sqlBuilder.Where(whereString, param);
            }

            if (order != null)
            {
                sqlBuilder.OrderBy(order);
                sqlBuilder.IsAse(asc);
            }

            if (pageNum >= 0 && pageSize > 0)
            {
                int numMin = (pageNum - 1) * pageSize;
                sqlBuilder.Append($" limit {numMin},{pageSize}");
            }
            return sqlBuilder;
        }
    }
}
