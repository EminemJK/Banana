/***********************************
* Developer: Lio.Huang
* Date：2018-11-20
*
* UpdateDate:
* 2018-12-28  1.更新GetPageList中的Select *  => Select {ColumnList}
 * 2019-01-03  1.更新GetPageList中的property.Name => SqlMapperExtensions.GetColumnAlias(property)
 *             2.更新AppendColumnName、AppendColumnNameEqualsValue 新增别名
**********************************/

using Banana.Uow.Extension;
using Banana.Uow.Interface;
using Banana.Uow.Models;
using Banana.Uow.SQLBuilder;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Uow.Adapter
{
    /// <summary>
    /// SQLite 扩展
    /// </summary>
    internal partial class SQLiteAdapter : SqlAdapterBase, ISqlAdapter
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
        public async Task<int> InsertAsync(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert, bool isList)
        {
            string cmd = "";
            if (isList)
            {
                cmd = $"insert into {tableName} ({columnList}) values ({parameterList})";
                return await connection.ExecuteAsync(cmd, entityToInsert, transaction, commandTimeout);
            }
            cmd = $"INSERT INTO {tableName} ({columnList}) VALUES ({parameterList}); SELECT last_insert_rowid() id";
            var multi = await connection.QueryMultipleAsync(cmd, entityToInsert, transaction, commandTimeout).ConfigureAwait(false);

            var id = (int)multi.Read().First().id;
            var pi = keyProperties as PropertyInfo[] ?? keyProperties.ToArray();
            if (pi.Length == 0) return id;

            var idp = pi[0];
            idp.SetValue(entityToInsert, Convert.ChangeType(id, idp.PropertyType), null);

            return id;
        }

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
        public int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert, bool isList)
        {
            string cmd = "";
            if (isList)
            {
                cmd = $"insert into {tableName} ({columnList}) values ({parameterList})";
                return connection.Execute(cmd, entityToInsert, transaction, commandTimeout);
            }
            cmd = $"INSERT INTO {tableName} ({columnList}) VALUES ({parameterList}); SELECT last_insert_rowid() id";
            var multi = connection.QueryMultiple(cmd, entityToInsert, transaction, commandTimeout);

            var id = (int)multi.Read().First().id;
            var propertyInfos = keyProperties as PropertyInfo[] ?? keyProperties.ToArray();
            if (propertyInfos.Length == 0) return id;

            var idProperty = propertyInfos[0];
            idProperty.SetValue(entityToInsert, Convert.ChangeType(id, idProperty.PropertyType), null);

            return id;
        }

        /// <summary>
        /// Adds the name of a column.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="columnAlias">The column alias.</param>
        /// <param name="tableName"></param>
        public string AppendColumnName(string columnName, string columnAlias, string tableName)
        {
            string tmp = "";
            if (string.IsNullOrEmpty(columnAlias) || columnName.Equals(columnAlias))
                tmp = string.Format("\"{0}\"", columnName);
            else
                tmp = string.Format("\"{0}\" as {1}", columnName, columnAlias);

            if (!string.IsNullOrEmpty(tableName))
                return string.Format("\"{0}\".{1}", tableName, tmp);
            return tmp;
        }

        /// <summary>
        /// Adds a column equality to a parameter.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="columnAlias">The column alias.</param>
        public string AppendColumnNameEqualsValue(string columnName, string columnAlias)
        {
            if (string.IsNullOrEmpty(columnAlias) || columnName.Equals(columnAlias))
                return string.Format("\"{0}\" = @{1}", columnName, columnName);
            else
                return string.Format("\"{0}\" = @{1}", columnName, columnAlias);
        }

        /// <summary>
        /// Adds the parametr to sql.
        /// </summary>
        /// <param name="sb">The string builder  to append to.</param>
        /// <param name="paramName">The column name.</param>
        public string AppendParametr(string paramName)
        {
            return string.Format("@{0}", paramName);
        }

        /// <summary>
        /// SQLite 扩展
        public SQLiteAdapter() { } 


        public ISqlBuilder GetPageList<T>(IRepository<T> repository, int pageNum, int pageSize, string whereString, object param, object order, bool asc)
            where T : class, IEntity
        {
            SqlBuilder sqlBuilder = new SqlBuilder();
            var sbColumnList = new StringBuilder(null);
            var allProperties = SqlMapperExtensions.TypePropertiesCache(typeof(T));
            for (var i = 0; i < allProperties.Count; i++)
            {
                var property = allProperties[i];
                sbColumnList.Append(AppendColumnName(SqlMapperExtensions.GetColumnName(property), property.Name, ""));
                if (i < allProperties.Count - 1)
                    sbColumnList.Append(", ");
            }

            sqlBuilder.Select(args: sbColumnList.ToString());
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

        public string GetPageList(string selection, string source, string conditions, string order, int pageSize, int? pageNumber = null)
        {
            throw new NotImplementedException();
        }
    }
}
