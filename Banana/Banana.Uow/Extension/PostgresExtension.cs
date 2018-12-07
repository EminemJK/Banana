/***********************************
 * Coder：EminemJK
 * Date：2018-12-07
 **********************************/

using Banana.Uow.Interface;
using Banana.Uow.Models;

namespace Banana.Uow.Extension
{
    /// <summary>
    /// Postgres分页扩展
    /// </summary>
    public class PostgresExtension : IAdapter
    {
        /// <summary>
        /// Postgres分页扩展
        /// </summary>
        public PostgresExtension() { }

        public SqlBuilder GetPageList<T>(IRepository<T> repository, int pageNum = 0, int pageSize = 0, string whereString = null, object param = null, object order = null, bool asc = false)
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
                sqlBuilder.Append($" limit {pageSize} offset {numMin}");
            }
            return sqlBuilder;
        }
    }
}
