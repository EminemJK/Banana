/***********************************
 * Developer: Lio.Huang
 * Create Date：2018-11-16
 * 
 * Last Update：2018-12-18
 * 2019-01-04  1.更新Query和QueryAsync 新增stringId
 * 2019-01-07  1.Add && _dbConnection.State!= ConnectionState.Connecting
 *             2.GetAdapter(_dbConnection)
 * 2019-01-11  1.Query(int) => Query(obj)
 * 2019-01-21  1.Current DB Setting
 **********************************/

using System;
using System.Collections.Generic;
using Dapper;
using System.Data;
using System.Linq;
using Banana.Uow.Models;
using Banana.Uow.Interface;
using System.Threading.Tasks;
using Banana.Uow.Extension;

namespace Banana.Uow
{
    /// <summary>
    /// 仓储基类| Base Repository
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        /// <summary>
        /// 仓储基类| Base Repository
        /// </summary>
        public Repository(string dbAliase ="")
        {
            this.dbAliase = dbAliase;
        }


        /// <summary>
        /// 仓储基类| Base Repository
        /// </summary>
        public Repository(IDbConnection dbConnection, IDbTransaction dbTransaction = null)
        {
            this._dbConnection = dbConnection;
            this._dbTransaction = dbTransaction;
        }

        #region Field & method
        private string dbAliase;

        /// <summary>
        /// CurrentDB Setting
        /// </summary>
        public DBSetting CurrentDBSetting => ConnectionBuilder.GetDBSetting(dbAliase);

        private IDbConnection _dbConnection;
        /// <summary>
        /// IDbConnection
        /// </summary>
        public IDbConnection DBConnection
        {
            get
            {
                if (_dbConnection == null)
                {
                    _dbConnection = ConnectionBuilder.CreateConnection(dbAliase);
                }
                if (_dbConnection.State == ConnectionState.Closed && _dbConnection.State!= ConnectionState.Connecting)
                {
                    _dbConnection.Open();
                } 
                return _dbConnection;
            }
            private set { this._dbConnection = value; }
        }

        /// <summary>
        /// 表名|
        /// To get the name of the table
        /// </summary>
        public string TableName
        {
            get
            { 
                return SqlMapperExtensions.GetTableName(typeof(T));
            }
        }

        private IDbTransaction _dbTransaction;
        /// <summary>
        /// 开启事务|
        /// Open transaction
        /// </summary>
        public IDbTransaction OpenTransaction()
        {
            _dbTransaction = DBConnection.BeginTransaction();
            TrancationState = ETrancationState.Opened;
            return _dbTransaction;
        }

        /// <summary>
        /// 事务状态|
        /// transaction's state
        /// </summary>
        public ETrancationState TrancationState { get; private set; } = ETrancationState.Closed;

        /// <summary>
        /// 对象类型|
        /// type of entity
        /// </summary>
        public Type EntityType => typeof(T);
        #endregion

        #region Sync
        /// <summary>
        /// 删除实体|
        /// Delete entity in table "Ts".
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns>true if deleted, false if not found</returns>
        public bool Delete(T entity)
        {
            return DBConnection.Delete(entity, _dbTransaction);
        }

        /// <summary>
        /// 插入实体|
        /// Inserts an entity into table "Ts" and returns identity id or number of inserted rows if inserting a list.
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns>返回自增Id|Identity of inserted entity.</returns>
        public long Insert(T entity)
        {
            return DBConnection.Insert(entity, _dbTransaction);
        }

        /// <summary>
        /// 插入实体列表
        /// |Inserts an entity into table "Ts" and returns identity id or number of inserted rows if inserting a list.
        /// </summary>
        /// <param name="entityList">entity list</param>
        /// <returns>返回受影响行数|number of inserted rows if inserting a list.</returns>
        public long Insert(IEnumerable<T> entityList)
        {
            return DBConnection.Insert(entityList, _dbTransaction);
        }

        /// <summary>
        /// 查询单个实体|
        /// Returns a single entity by a single id from table "Ts".  
        /// Id must be marked with [Key]/[ExplicitKey] attribute.
        /// Entities created from interfaces are tracked/intercepted for changes and used by the Update() extension
        /// for optimal performance. 
        /// </summary>
        /// <param name="id">Id of the entity to get, must be marked with [Key]/[ExplicitKey] attribute</param>
        /// <returns>Entity of T</returns>
        public T Query(object id)
        {
            return DBConnection.Get<T>(id);
        }

        /// <summary>
        /// 查询总数|
        /// Returns the number of rows
        /// </summary>
        /// <param name="whereString">where sql</param>
        /// <param name="param">param</param>
        /// <returns>number of rows</returns>
        public int QueryCount(string whereString = null, object param = null)
        {
            SqlBuilder sb = new SqlBuilder();
            sb.Select(args: "Count(*)");
            sb.From(TableName);
            if (!string.IsNullOrEmpty(whereString))
            {
                sb.Where(whereString, param);
            }
            return DBConnection.QueryFirst<int>(sb.SQL, sb.Arguments);
        }

        /// <summary>
        /// 查询列表|
        /// Executes a query, returning the data typed as T.
        /// </summary>
        /// <param name="whereString">whereString,(example:whereString:name like @name)</param>
        /// <param name="param">whereString's param，(example:new { name = "google%" })</param> 
        /// <param name="order">order param,(example:order:"createTime")</param>
        /// <param name="asc">Is ascending</param>
        /// <returns>returning the data list typed as T.</returns>
        public List<T> QueryList(string whereString = null, object param = null, string order = null, bool asc = false)
        {
            if (string.IsNullOrEmpty(whereString) && string.IsNullOrEmpty(order))
            {
                return DBConnection.GetAll<T>().ToList();
            }
            else
            {
                ISqlAdapter adapter = ConnectionBuilder.GetAdapter(this.DBConnection);
                var sqlbuilder = adapter.GetPageList(this, whereString: whereString, param: param, order: order, asc: asc);
                return DBConnection.Query<T>(sqlbuilder.SQL, sqlbuilder.Arguments).ToList();
            }
        }

        /// <summary>
        /// 分页查询|
        /// Executes a query, returning the paging data typed as T.
        /// </summary>
        /// <param name="pageNum">页码|page number</param>
        /// <param name="pageSize">页大小|page size</param>
        /// <param name="whereString">parameterized sql of "where",(example:whereString:name like @name)</param>
        /// <param name="param">whereString's param，(example:new { name = "google%" })</param>
        /// <param name="order">order param,(example:order:"createTime")</param>
        /// <param name="asc">Is ascending</param>
        /// <returns>返回分页数据|returning the paging data typed as T</returns>
        public IPage<T> QueryList(int pageNum, int pageSize, string whereString = null, object param = null, string order = null, bool asc = false)
        {
            IPage<T> paging = new Paging<T>(pageNum, pageSize);
            ISqlAdapter adapter = ConnectionBuilder.GetAdapter(this.DBConnection);
            var sqlbuilder = adapter.GetPageList(this, pageNum, pageSize, whereString, param, order, asc);
            paging.data = DBConnection.Query<T>(sqlbuilder.SQL, sqlbuilder.Arguments).ToList();
            paging.dataCount = QueryCount(whereString, param);
            return paging;
        }

        /// <summary>
        /// 更新|
        /// Updates entity in table "Ts", checks if the entity is modified if the entity is tracked by the Get() extension.
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        public bool Update(T entity)
        {
            return DBConnection.Update<T>(entity, _dbTransaction);
        }

        /// <summary>
        /// 执行单条语句
        /// |Execute parameterized SQL.
        /// </summary>
        /// <param name="sql">parameterized SQL</param>
        /// <param name="parms">The parameters to use for this query.</param>
        /// <returns>受影响的行数|The number of rows affected.</returns>
        public int Execute(string sql, dynamic parms)
        {
            return DBConnection.Execute(sql, (object)parms, transaction: _dbTransaction);
        }

        /// <summary>
        /// 批量插入数据|
        /// Execute insert SQL.
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="entities">entityList</param>
        /// <returns>受影响的行数|The number of rows affected.</returns>
        public virtual bool InsertBatch(string sql, IEnumerable<T> entities)
        {
            using (IDbTransaction trans = OpenTransaction())
            {
                try
                {
                    int res = Execute(sql, entities);
                    TrancationState = ETrancationState.Closed;
                    if (res > 0)
                    {
                        trans.Commit();
                        return true;
                    }
                    else
                    {
                        trans.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    TrancationState = ETrancationState.Closed;
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 删除|
        /// Delete data in table "Ts".
        /// </summary>
        /// <param name="whereString">parameterized sql of "where",(example:whereString:name like @name)</param>
        /// <param name="param">whereString's param，(example:new { name = "google%" })</param>
        /// <returns>受影响的行数|The number of rows affected.</returns>
        public bool Delete(string whereString, object param)
        {
            SqlBuilder sb = new SqlBuilder();
            sb.Append("DELETE FROM " + TableName);
            sb.Where(whereString, param);
            return Execute(sb.SQL, sb.Arguments) > 0;
        }
        #endregion

        #region Async
        /// <summary>
        /// 插入|
        /// Inserts an entity into table "Ts" asynchronously using .NET 4.5 Task and returns identity id.
        /// </summary>
        /// <param name="entity">Entity to insert</param>
        /// <returns>返回自增Id|Identity of inserted entity.</returns>
        public async Task<int> InsertAsync(T entity)
        {
            return await DBConnection.InsertAsync(entity, _dbTransaction);
        }

        /// <summary>
        /// 插入实体列表|
        ///  Inserts an entity list into table "Ts" asynchronously using .NET 4.5 Task and returns identity id.
        /// </summary>
        /// <param name="entityList">Entity list to insert</param>
        /// <returns>返回受影响行数|number of inserted rows if inserting a list.</returns>
        public async Task<int> InsertAsync(IEnumerable<T> entityList)
        {
            return await DBConnection.InsertAsync(entityList, _dbTransaction);
        }

        /// <summary>
        /// 删除|
        /// Delete entity in table "Ts" asynchronously using .NET 4.5 Task.
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        /// <returns>是否成功|true if deleted, false if not found</returns>
        public async Task<bool> DeleteAsync(T entity)
        {
            return await DBConnection.DeleteAsync(entity, _dbTransaction);
        }

        /// <summary>
        /// 更新|
        /// Updates entity in table "Ts" asynchronously using .NET 4.5 Task, checks if the entity is modified if the entity is tracked by the Get() extension.
        /// </summary>
        /// <param name="entity">Entity to be updated</param>
        /// <returns>是否更新成功|true if updated, false if not found or not modified (tracked entities)</returns>
        public async Task<bool> UpdateAsync(T entity)
        {
            return await DBConnection.UpdateAsync(entity, _dbTransaction);
        }

        /// <summary>
        /// 查询|
        /// Returns a single entity by a single id from table "Ts" asynchronously using .NET 4.5 Task. T must be of interface type. 
        /// Id must be marked with [Key] attribute.
        /// Created entity is tracked/intercepted for changes and used by the Update() extension. 
        /// </summary>
        /// <param name="id">Id of the entity to get, must be marked with [Key] attribute</param>
        /// <returns>返回实体|Entity of T</returns>
        public async Task<T> QueryAsync(object id)
        {
            return await DBConnection.GetAsync<T>(id);
        }

        /// <summary>
        /// 异步查询总数|
        ///  Returns the number of rows
        /// </summary>
        /// <param name="whereString">parameterized sql of "where",(example:whereString:name like @name)</param>
        /// <param name="param">whereString's param，(example:new { name = "google%" })</param>
        /// <returns>总数|Returns the number of rows</returns>
        public async Task<int> QueryCountAsync(string whereString = null, object param = null)
        {
            SqlBuilder sb = new SqlBuilder();
            sb.Select(args: "Count(*)");
            sb.From(TableName);
            if (!string.IsNullOrEmpty(whereString))
            {
                sb.Where(whereString, param);
            }
            return await DBConnection.QueryFirstAsync<int>(sb.SQL, sb.Arguments);
        }

        /// <summary>
        /// 查询列表|
        /// Executes a query, returning the data typed as T.
        /// </summary>
        /// <param name="whereString">whereString,(example:whereString:name like @name)</param>
        /// <param name="param">whereString's param，(example:new { name = "google%" })</param> 
        /// <param name="order">order param,(example:order:"createTime")</param>
        /// <param name="asc">Is ascending</param>
        /// <returns>returning the data list typed as T.</returns>
        public async Task<IEnumerable<T>> QueryListAsync(string whereString = null, object param = null, string order = null, bool asc = false)
        {
            if (string.IsNullOrEmpty(whereString) && string.IsNullOrEmpty(order))
            {
                return await DBConnection.GetAllAsync<T>();
            }
            else
            {
                ISqlAdapter adapter = ConnectionBuilder.GetAdapter(this.DBConnection);
                var sqlbuilder = adapter.GetPageList(this, whereString: whereString, param: param, order: order, asc: asc);
                return await DBConnection.QueryAsync<T>(sqlbuilder.SQL, sqlbuilder.Arguments);
            }
        }

        /// <summary>
        /// 分页查询|
        /// Executes a query, returning the paging data typed as T.
        /// </summary>
        /// <param name="pageNum">页码|page number</param>
        /// <param name="pageSize">页大小|page size</param>
        /// <param name="whereString">parameterized sql of "where",(example:whereString:name like @name)</param>
        /// <param name="param">whereString's param，(example:new { name = "google%" })</param>
        /// <param name="order">order param,(example:order:"createTime")</param>
        /// <param name="asc">Is ascending</param>
        /// <returns>返回分页数据|returning the paging data typed as T</returns>
        public async Task<IPage<T>> QueryListAsync(int pageNum, int pageSize, string whereString = null, object param = null, string order = null, bool asc = false)
        {
            return await Task.Run(() =>
            {
                IPage<T> paging = new Paging<T>(pageNum, pageSize);
                ISqlAdapter adapter = ConnectionBuilder.GetAdapter(this.DBConnection);
                var sqlbuilder = adapter.GetPageList(this, pageNum, pageSize, whereString, param, order, asc);
                paging.data = DBConnection.Query<T>(sqlbuilder.SQL, sqlbuilder.Arguments).ToList();
                paging.dataCount = QueryCount(whereString, param);
                return paging;
            }); ;
        }

        /// <summary>
        /// 执行单条语句
        /// |Execute parameterized SQL.
        /// </summary>
        /// <param name="sql">parameterized SQL</param>
        /// <param name="parms">The parameters to use for this query.</param>
        /// <returns>受影响的行数|The number of rows affected.</returns>
        public async Task<int> ExecuteAsync(string sql, dynamic parms)
        {
            return await DBConnection.ExecuteAsync(sql, (object)parms, transaction: _dbTransaction);
        }

        /// <summary>
        /// 删除全部|
        /// Delete all data
        /// </summary>
        public bool DeleteAll()
        {
            return DBConnection.DeleteAll<T>(_dbTransaction);
        }

        /// <summary>
        /// 删除全部|
        /// Delete all data
        /// </summary>
        public async Task<bool> DeleteAllAsync()
        {
            return await DBConnection.DeleteAllAsync<T>(_dbTransaction);
        }

        /// <summary>
        /// 删除|
        /// Delete data in table "Ts".
        /// </summary>
        /// <param name="whereString">parameterized sql of "where",(example:whereString:name like @name)</param>
        /// <param name="param">whereString's param，(example:new { name = "google%" })</param>
        /// <returns>受影响的行数|The number of rows affected.</returns>
        public async Task<bool> DeleteAsync(string whereString, object param)
        {
            SqlBuilder sb = new SqlBuilder();
            sb.Append("DELETE FROM " + TableName);
            sb.Where(whereString, param);
            return await ExecuteAsync(sb.SQL, sb.Arguments) > 0;
        }
        #endregion
    }
}
