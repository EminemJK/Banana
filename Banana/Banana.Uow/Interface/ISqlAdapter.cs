/***********************************
 * Developer: Lio.Huang
 * Date：2018-11-16
 * 
 * Last Update：
 * 2019-01-03  1.更新AppendColumnName、AppendColumnNameEqualsValue,增加别名
 **********************************/

using Banana.Uow.Extension;
using Banana.Uow.Models;
using Banana.Uow.SQLBuilder;
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
        /// <param name="columnName">The column name.</param>
        /// <param name="columnAlias">The column alias.</param>
        /// <param name="tableName">tableName</param>
        string AppendColumnName(string columnName, string columnAlias, string tableName);

        /// <summary>
        /// Adds a column equality to a parameter.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="columnAlias">The column alias.</param>
        string AppendColumnNameEqualsValue(string columnName, string columnAlias);

        /// <summary>
        /// Adds the parametr to sql
        /// </summary>
        /// <param name="paramName"></param>
        string AppendParametr(string paramName);

        /// <summary>
        /// 分页查询|
        /// Executes a query, returning the paging data typed as T.
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="source"></param>
        /// <param name="conditions"></param>
        /// <param name="pageSize">页大小|page size</param>
        /// <param name="pageNumber"></param>
        /// <param name="order">order param,(example:order:"createTime")</param>
        /// <returns>返回分页数据|returning the paging data typed as T</returns>
        string GetPageList(string selection, string source, string conditions, string order, int pageSize, int? pageNumber = null);

        /// <summary>
        /// Base query
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="source"></param>
        /// <param name="conditions"></param>
        /// <param name="order"></param>
        /// <param name="grouping"></param>
        /// <param name="having"></param>
        /// <returns></returns>
        string QueryString(string selection, string source, string conditions, string order, string grouping, string having);

        ISqlBuilder GetPageList<T>(IRepository<T> repository, int pageNum = 0, int pageSize = 0, string whereString = null, object param = null, object order = null, bool asc = false)
          where T : class, IEntity;
    }
}
