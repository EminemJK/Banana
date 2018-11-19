using Banana.Uow.Interface;
using Banana.Uow.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Banana.Uow.Extension
{
    public class SQLServerExtension : IAdapter
    {
        public SQLServerExtension() { }

        public SqlBuilder GetPageSQL<T>(IRepository<T> repository, int pageNum, int pageSize, string whereString = null, object param = null, object order = null, bool asc = false)
            where T : class, IEntity
        {
            SqlBuilder sqlBuilder = new SqlBuilder();
            sqlBuilder.Select(repository.EntityType);
            sqlBuilder.Append("(SELECT ROW_NUMBER() OVER(ORDER BY ID ASC) AS rowid,* FROM infoTab) as t");

            int numMin = pageNum, numMax = pageSize + pageNum;
            if (!string.IsNullOrEmpty(whereString))
            {
                sqlBuilder.Where(whereString, param);
            }
            else
            {
                sqlBuilder.Where(" and t.rowid>@numMin and t.rowid<=@numMax", new { numMin, numMax });
            }
            if (order != null)
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
