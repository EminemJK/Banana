/***********************************
 * Developer: Lio.Huang
 * Date：2018-12-12
 * 
 * UpdateDate:
 * 2018-12-28  1.更新GetPageList中的Select *  => Select {ColumnList}
 * 2019-01-03  1.更新GetPageList中的property.Name => SqlMapperExtensions.GetColumnAlias(property)
 *             2.更新AppendColumnName、AppendColumnNameEqualsValue 新增别名
 * 2019-01-09  1.Fix bug => sqlBuilder.Where(whereString, param);
 **********************************/

using Banana.Uow.Extension;
using Banana.Uow.Interface;
using Banana.Uow.Models;
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
    /// The SQL Server database adapter.
    /// </summary>
    internal partial class SqlServerAdapter : ISqlAdapter
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
            cmd = $"INSERT INTO {tableName} ({columnList}) values ({parameterList}); SELECT SCOPE_IDENTITY() id";
            var multi = await connection.QueryMultipleAsync(cmd, entityToInsert, transaction, commandTimeout).ConfigureAwait(false);

            var first = multi.Read().FirstOrDefault();
            if (first == null || first.id == null) return 0;

            var id = (int)first.id;
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
            cmd = $"insert into {tableName} ({columnList}) values ({parameterList});select SCOPE_IDENTITY() id";
            var multi = connection.QueryMultiple(cmd, entityToInsert, transaction, commandTimeout);

            var first = multi.Read().FirstOrDefault();
            if (first == null || first.id == null) return 0;

            var id = (int)first.id;
            var propertyInfos = keyProperties as PropertyInfo[] ?? keyProperties.ToArray();
            if (propertyInfos.Length == 0) return id;

            var idProperty = propertyInfos[0];
            idProperty.SetValue(entityToInsert, Convert.ChangeType(id, idProperty.PropertyType), null);

            return id;
        }

        /// <summary>
        /// Adds the name of a column.
        /// </summary>
        /// <param name="sb">The string builder  to append to.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="columnAlias">The column alias.</param>
        public void AppendColumnName(StringBuilder sb, string columnName, string columnAlias)
        {
            if (string.IsNullOrEmpty(columnAlias) || columnName.Equals(columnAlias))
                sb.AppendFormat("[{0}]", columnName);
            else
                sb.AppendFormat("[{0}] as {1}", columnName, columnAlias);
        }

        /// <summary>
        /// Adds a column equality to a parameter.
        /// </summary>
        /// <param name="sb">The string builder  to append to.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="columnAlias">The column alias.</param>
        public void AppendColumnNameEqualsValue(StringBuilder sb, string columnName, string columnAlias)
        {
            if (string.IsNullOrEmpty(columnAlias) || columnName.Equals(columnAlias))
                sb.AppendFormat("[{0}] = @{1}", columnName, columnName);
            else
                sb.AppendFormat("[{0}] = @{1}", columnName, columnAlias);
        }

        /// <summary>
        /// Adds the parametr to sql.
        /// </summary>
        /// <param name="sb">The string builder  to append to.</param>
        /// <param name="paramName">The column name.</param>
        public void AppendParametr(StringBuilder sb, string paramName)
        {
            sb.AppendFormat("@{0}", paramName);
        }

        /// <summary>
        /// SQL Server 扩展
        /// </summary>
        public SqlServerAdapter() { }

        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repository"></param>
        /// <param name="pageNum">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="whereString">where语句，不需要携带where</param>
        /// <param name="param">where 参数</param>
        /// <param name="order"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        public ISqlBuilder GetPageList<T>(IRepository<T> repository, int pageNum = 0, int pageSize = 0, string whereString = null, object param = null, object order = null, bool asc = false)
            where T : class, IEntity
        {
            SqlBuilder sqlBuilder = new SqlBuilder();
            var sbColumnList = new StringBuilder(null);
            var allProperties = SqlMapperExtensions.TypePropertiesCache(typeof(T));
            for (var i = 0; i < allProperties.Count; i++)
            {
                var property = allProperties[i];
                AppendColumnName(sbColumnList, SqlMapperExtensions.GetColumnName(property), property.Name);
                if (i < allProperties.Count - 1)
                    sbColumnList.Append(", ");
            }

            sqlBuilder.Select(args: sbColumnList.ToString());
            if (pageSize > 0)
            {
                SqlBuilder sqlBuilderRows = new SqlBuilder();
                string ascSql = " asc";
                if (!asc)
                {
                    ascSql = " desc";
                }
                string orderSql = "ID";
                if (order != null)
                {
                    orderSql = SqlBuilder.GetArgsString("ORDER BY", args: order);
                } 

                if (repository.CurrentDBSetting.DBType == DBType.SqlServer2012)
                {
                    sqlBuilder.From(repository.TableName);
                    if (!string.IsNullOrEmpty(whereString))
                    {
                        sqlBuilder.Where(whereString, param);
                    }
                    sqlBuilder.OrderBy(orderSql);
                    int numMin = (pageNum - 1) * pageSize;
                    sqlBuilder.Append($"offset {numMin} rows fetch next {pageSize} rows only");
                }
                else
                {
                    sqlBuilderRows.Select(args: "SELECT ROW_NUMBER() OVER(ORDER BY " + orderSql + " " + ascSql + ") AS row_id,*");
                    sqlBuilderRows.From(repository.TableName);
                    if (!string.IsNullOrEmpty(whereString))
                    {
                        sqlBuilderRows.Where(whereString, param);
                    }
                    sqlBuilder.Append($"From ({sqlBuilderRows.SQL}) as t", sqlBuilderRows.Arguments);
                    if (pageNum <= 0)
                        pageNum = 1;
                    int numMin = (pageNum - 1) * pageSize + 1,
                        numMax = pageNum * pageSize;
                    sqlBuilder.Where("t.row_id>=@numMin and t.row_id<=@numMax", new { numMin, numMax });
                }
               
            }
            else
            {
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
            }
            return sqlBuilder;
        }
    }
}
