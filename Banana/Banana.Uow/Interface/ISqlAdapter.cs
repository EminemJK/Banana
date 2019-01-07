/***********************************
 * Developer: Lio.Huang
 * Date：2018-11-16
 * 
 * Last Update：
 * 2019-01-03  1.更新AppendColumnName、AppendColumnNameEqualsValue,增加别名
 **********************************/

using Banana.Uow.Extension;
using Banana.Uow.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Uow.Interface
{
    /// <summary>
    /// The interface for all Dapper database operations
    /// Implementing this is each provider's model.
    /// </summary>
    public partial interface ISqlAdapter
    {
        /// <summary>
        /// Inserts <paramref name="entityToInsert"/> into the database, returning the Id of the row created.
        /// </summary>
        /// <param name="connection">The connection to use.</param>
        /// <param name="transaction">The transaction to use.</param>
        /// <param name="commandTimeout">The command timeout to use.</param>
        /// <param name="tableName">The table to insert into.</param>
        /// <param name="columnList">The columns to set with this insert.</param>
        /// <param name="parameterList">The parameters to set for this insert.</param>
        /// <param name="keyProperties">The key columns in this table.</param>
        /// <param name="entityToInsert">The entity to insert.</param>
        /// <returns>The Id of the row created.</returns>
        int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert, bool isList);

        /// <summary>
        /// Inserts <paramref name="entityToInsert"/> into the database, returning the Id of the row created.
        /// </summary>
        /// <param name="connection">The connection to use.</param>
        /// <param name="transaction">The transaction to use.</param>
        /// <param name="commandTimeout">The command timeout to use.</param>
        /// <param name="tableName">The table to insert into.</param>
        /// <param name="columnList">The columns to set with this insert.</param>
        /// <param name="parameterList">The parameters to set for this insert.</param>
        /// <param name="keyProperties">The key columns in this table.</param>
        /// <param name="entityToInsert">The entity to insert.</param>
        /// <returns>The Id of the row created.</returns>
        Task<int> InsertAsync(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert, bool isList);

        /// <summary>
        /// Adds the name of a column.
        /// </summary>
        /// <param name="sb">The string builder  to append to.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="columnAlias">The column alias.</param>
        void AppendColumnName(StringBuilder sb, string columnName, string columnAlias);

        /// <summary>
        /// Adds a column equality to a parameter.
        /// </summary>
        /// <param name="sb">The string builder  to append to.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="columnAlias">The column alias.</param>
        void AppendColumnNameEqualsValue(StringBuilder sb, string columnName, string columnAlias);

        /// <summary>
        /// Adds the parametr to sql
        /// </summary>
        /// <param name="sbParameterList"></param>
        /// <param name="paramName"></param>
        void AppendParametr(StringBuilder sbParameterList, string paramName);

        /// <summary>
        /// 分页查询|
        /// Executes a query, returning the paging data typed as T.
        /// </summary>
        /// <param name="repository">repository</param>
        /// <param name="pageNum">页码|page number</param>
        /// <param name="pageSize">页大小|page size</param>
        /// <param name="whereString">parameterized sql of "where",(example:whereString:name like @name)</param>
        /// <param name="param">whereString's param，(example:new { name = "google%" })</param>
        /// <param name="order">order param,(example:order:"createTime")</param>
        /// <param name="asc">Is ascending</param>
        /// <returns>返回分页数据|returning the paging data typed as T</returns>
        ISqlBuilder GetPageList<T>(IRepository<T> repository, int pageNum = 0, int pageSize = 0, string whereString = null, object param = null, object order = null, bool asc = false)
           where T : class, IEntity;
    }
}
