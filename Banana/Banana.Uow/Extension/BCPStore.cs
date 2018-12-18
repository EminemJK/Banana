/***********************************
 * Coder：EminemJK
 * Date：2018-12-17
 **********************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Banana.Uow.Interface;
using Banana.Uow.Models;
using Dapper;

namespace Banana.Uow.Extension
{
    /// <summary>
    /// SQL Server SqlBulkCopy
    /// </summary>
    public class BCPStore
    {
        private string tableName = "";
        private DataTable myTable = new DataTable();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tablename"></param>
        public void Init(SqlConnection connection, string tablename)
        { 
            this.tableName = tablename;
            init(connection);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="repository"></param>
        public void Init<T>(IRepository<T> repository) where T : class, IEntity
        {
            this.tableName = repository.TableName;
            if (repository.DBConnection is SqlConnection)
            {
                init(repository.DBConnection as SqlConnection);
            }
            else
            {
                throw new ArgumentException("BCPStore仅Sql Server数据库中使用", "repository.DBConnection");
            }
        }

        private void init(SqlConnection connection)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand cmd = new SqlCommand("select top 0 * from " + tableName, connection);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            myTable = ds.Tables[0];
            myTable.Rows.Clear();
        }

        /// <summary>
        /// 数据
        /// </summary>
        public void AddData(object[] objRow)
        {
            myTable.Rows.Add(objRow);
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="truncateFirst"></param>
        /// <param name="bulkCopyTimeout"></param>
        public void Flush(SqlConnection connection, bool truncateFirst, int bulkCopyTimeout = 36000)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            try
            {
                if (truncateFirst)
                {
                    SqlCommand cmd = new SqlCommand("truncate table " + tableName, connection);
                    cmd.ExecuteNonQuery();
                }
                using (SqlBulkCopy bcp = new SqlBulkCopy(connection))
                {
                    bcp.DestinationTableName = tableName;
                    bcp.BulkCopyTimeout = bulkCopyTimeout;
                    bcp.WriteToServer(myTable);
                }
                myTable.Rows.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
