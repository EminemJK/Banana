/***********************************
 * Coder：EminemJK
 * Date：2018-12-12
 * 
 * UpdateDate:
 * 2019-01-03  1.更新AppendColumnName、AppendColumnNameEqualsValue 新增别名
 **********************************/

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

namespace Banana.Uow.Extension
{
    /// <summary>
    /// The Firebase SQL adapeter.
    /// </summary>
    internal partial class FbAdapter : ISqlAdapter
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
            cmd = $"insert into {tableName} ({columnList}) values ({parameterList})";
            await connection.ExecuteAsync(cmd, entityToInsert, transaction, commandTimeout).ConfigureAwait(false);

            var propertyInfos = keyProperties as PropertyInfo[] ?? keyProperties.ToArray();
            var keyName = propertyInfos[0].Name;
            var r = await connection.QueryAsync($"SELECT FIRST 1 {keyName} ID FROM {tableName} ORDER BY {keyName} DESC", transaction: transaction, commandTimeout: commandTimeout).ConfigureAwait(false);

            var id = r.First().ID;
            if (id == null) return 0;
            if (propertyInfos.Length == 0) return Convert.ToInt32(id);

            var idp = propertyInfos[0];
            idp.SetValue(entityToInsert, Convert.ChangeType(id, idp.PropertyType), null);

            return Convert.ToInt32(id);
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
            cmd = $"insert into {tableName} ({columnList}) values ({parameterList})";
            connection.Execute(cmd, entityToInsert, transaction, commandTimeout);

            var propertyInfos = keyProperties as PropertyInfo[] ?? keyProperties.ToArray();
            var keyName = propertyInfos[0].Name;
            var r = connection.Query($"SELECT FIRST 1 {keyName} ID FROM {tableName} ORDER BY {keyName} DESC", transaction: transaction, commandTimeout: commandTimeout);

            var id = r.First().ID;
            if (id == null) return 0;
            if (propertyInfos.Length == 0) return Convert.ToInt32(id);

            var idp = propertyInfos[0];
            idp.SetValue(entityToInsert, Convert.ChangeType(id, idp.PropertyType), null);

            return Convert.ToInt32(id);
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
                sb.AppendFormat("{0}", columnName);
            else
                sb.AppendFormat("{0} as {1}", columnName, columnAlias);
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
                sb.AppendFormat("{0} = @{1}", columnName, columnName);
            else
                sb.AppendFormat("{0} = @{1}", columnName, columnAlias);
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

        public ISqlBuilder GetPageList<T>(IRepository<T> repository, int pageNum = 0, int pageSize = 0, string whereString = null, object param = null, object order = null, bool asc = false)
           where T : class, IEntity
        {
            throw new Exception("Sorry, it is not supported for the time being");
        }
    }
}

