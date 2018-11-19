using Banana.Uow.Interface;
using Banana.Uow.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Banana.Uow.Extension
{
    public class MySQLExtension : IAdapter
    {
        public MySQLExtension() { }

        public SqlBuilder GetPageSQL<T>(IRepository<T> repository, int pageNum, int pageSize, string whereString = null, object param = null, object order = null, bool asc = false) where T : class, IEntity
        {
            SqlBuilder sqlBuilder = new SqlBuilder();
            sqlBuilder.Select(repository.EntityType);
            sqlBuilder.From(repository.TableName);

            if (!string.IsNullOrEmpty(whereString))
            {
                sqlBuilder.Where(whereString, param);
            }
            sqlBuilder.Append($" limit {pageNum},{pageSize}");
            if (order!= null)
            {
                sqlBuilder.OrderBy(order);
                if (asc)
                {
                    sqlBuilder.Append("ase");
                }
                else
                {
                    sqlBuilder.Append("desc");
                }
            }

            return sqlBuilder;
        }
    }
}
