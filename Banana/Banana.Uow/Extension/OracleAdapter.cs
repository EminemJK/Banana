/***********************************
 * Coder：EminemJK
 * Date：2018-12-12
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
    /// The Oracle adapter.
    /// </summary>
    internal partial class OracleAdapter : ISqlAdapter
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
        public async Task<int> InsertAsync(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
        {
            var cmd = $"insert into {tableName} ({columnList}) values ({parameterList})";
            await connection.ExecuteAsync(cmd, entityToInsert, transaction, commandTimeout).ConfigureAwait(false);

            var propertyInfos = keyProperties as PropertyInfo[] ?? keyProperties.ToArray();
            string oracleSequence =""; 
            for(int i = 0; i < propertyInfos.Length; i++)
            {
                var sequencePropertyDescriptor = propertyInfos[i].GetCustomAttribute<KeyAttribute>();
                if (sequencePropertyDescriptor != null)
                {
                    oracleSequence = sequencePropertyDescriptor.OracleSequence;
                    break;
                }
            }
            if (string.IsNullOrEmpty(oracleSequence))
                throw new Exception("TableAttribute's OracleSequence is Null");


            var r =  connection.Query($"SELECT \"{oracleSequence}\".\"NEXTVAL\" oracleSequence FROM \"DUAL\"", transaction: transaction, commandTimeout: commandTimeout);

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
        public int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
        {
            var cmd = $"insert into {tableName} ({columnList}) values ({parameterList})";
            connection.Execute(cmd, entityToInsert, transaction, commandTimeout);

            var propertyInfos = keyProperties as PropertyInfo[] ?? keyProperties.ToArray();
            string oracleSequence = "";
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                var sequencePropertyDescriptor = propertyInfos[i].GetCustomAttribute<KeyAttribute>();
                if (sequencePropertyDescriptor != null)
                {
                    oracleSequence = sequencePropertyDescriptor.OracleSequence;
                    break;
                }
            }
            if (string.IsNullOrEmpty(oracleSequence))
                throw new Exception("TableAttribute's OracleSequence is Null");


            var r = connection.Query($"SELECT \"{oracleSequence}\".\"NEXTVAL\" oracleSequence FROM \"DUAL\"", transaction: transaction, commandTimeout: commandTimeout);

            var id = r.First().ID;
            if (id == null)
                return 0;
            if (propertyInfos.Length == 0)
                return Convert.ToInt32(id);

            var idp = propertyInfos[0];
            idp.SetValue(entityToInsert, Convert.ChangeType(id, idp.PropertyType), null);

            return Convert.ToInt32(id);
        }

        /// <summary>
        /// Adds the name of a column.
        /// </summary>
        /// <param name="sb">The string builder  to append to.</param>
        /// <param name="columnName">The column name.</param>
        public void AppendColumnName(StringBuilder sb, string columnName)
        {
            sb.AppendFormat("{0}", columnName);
        }

        /// <summary>
        /// Adds a column equality to a parameter.
        /// </summary>
        /// <param name="sb">The string builder  to append to.</param>
        /// <param name="columnName">The column name.</param>
        public void AppendColumnNameEqualsValue(StringBuilder sb, string columnName)
        {
            sb.AppendFormat("{0} = :{1}", columnName, columnName);
        }

        /// <summary>
        /// Adds parameter.
        /// </summary>
        /// <param name="sb">The string builder  to append to.</param>
        /// <param name="paramName">The parametr name.</param>
        public void AppendParametr(StringBuilder sb, string paramName)
        {
            sb.AppendFormat(":{0}", paramName);
        }

        public SqlBuilder GetPageList<T>(IRepository<T> repository, int pageNum = 0, int pageSize = 0, string whereString = null, object param = null, object order = null, bool asc = false)
          where T : class, IEntity
        {
            SqlBuilder sqlBuilder = new SqlBuilder();
            sqlBuilder.Select(args: "*");
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
                    orderSql = SqlBuilder.GetArgsString("ORDER BY", prefix: "t.", args: order);
                }

                sqlBuilderRows.Select(args: "SELECT ROW_NUMBER() OVER(ORDER BY " + orderSql + " " + ascSql + ") AS row_id,t.*");
                sqlBuilderRows.From(repository.TableName);
                if (!string.IsNullOrEmpty(whereString))
                {
                    sqlBuilderRows.Where(whereString, param);
                }
                sqlBuilder.Append($"From ({sqlBuilderRows.SQL}) TT", sqlBuilderRows.Arguments);

                if (pageNum <= 0)
                    pageNum = 1;
                int numMin = (pageNum - 1) * pageSize + 1, 
                    numMax = pageNum * pageSize;
                sqlBuilder.Where("TT.row_id between {0} and {1}", new { numMin, numMax });
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
