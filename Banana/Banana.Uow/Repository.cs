/***********************************
 * Coder：EminemJK
 * Date：2018-11-16
 **********************************/

using System;
using System.Collections.Generic;
using Dapper;
using System.Data;
using System.Linq;
using System.Reflection;
using Banana.Uow.Models;
using Banana.Uow.Interface;
using System.Threading.Tasks;
using Banana.Uow.Extension;

namespace Banana.Uow
{
    /// <summary>
    /// 仓储基类
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class, IEntity
    { 
        /// <summary>
        /// 仓储
        /// </summary>
        public Repository()
        {
            _dbConnection = ConnectionBuilder.CreateConnection(); 
        }


        /// <summary>
        /// 仓储
        /// </summary>
        public Repository(IDbConnection dbConnection, IDbTransaction dbTransaction = null)
        {
            this._dbConnection = dbConnection;
            this._dbTransaction = dbTransaction;
        }

        #region Method Properties
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
                    _dbConnection = ConnectionBuilder.CreateConnection();
                }
                if (_dbConnection.State == ConnectionState.Closed)
                {
                    _dbConnection.Open();
                }
                return _dbConnection;
            }
            private set { this._dbConnection = value; }
        }
         
        /// <summary>
        /// 表名
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
        /// 开启事务
        /// </summary>
        public IDbTransaction OpenTransaction()
        {
            _dbTransaction = DBConnection.BeginTransaction();
            TrancationState = ETrancationState.Opened;
            return _dbTransaction;
        }

        /// <summary>
        /// 事务状态
        /// </summary>
        public ETrancationState TrancationState { get; private set; } = ETrancationState.Closed;

        /// <summary>
        /// 对象类型
        /// </summary>
        public Type EntityType => typeof(T); 
        #endregion

        #region Sync
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体</param>
        public bool Delete(T entity)
        {
            return DBConnection.Delete(entity, _dbTransaction);
        }

        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity">实体</param>
        public long Insert(T entity)
        {
            return DBConnection.Insert(entity, _dbTransaction);
        }

        /// <summary>
        /// 查询单个实体
        /// </summary>
        /// <param name="id">Id</param>
        public T Query(int id)
        {
            return DBConnection.Get<T>(id);
        }

        /// <summary>
        /// 查询总数
        /// </summary>
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
        /// 查询列表
        /// </summary>
        /// <param name="whereString">where 语句如： name like @name (使用参数化)</param>
        /// <param name="param">参数，语句如：new { name = "%李%" }</param> 
        public List<T> QueryList(string whereString = null, object param = null, string order = null, bool asc = false)
        {
            if (string.IsNullOrEmpty(whereString) && string.IsNullOrEmpty(order))
            {
                return DBConnection.GetAll<T>().ToList();
            }
            else
            {
                ISqlAdapter adapter = ConnectionBuilder.GetAdapter();
                var sqlbuilder = adapter.GetPageList(this, whereString: whereString, param: param, order: order, asc: asc);
                return DBConnection.Query<T>(sqlbuilder.SQL, sqlbuilder.Arguments).ToList();
            }
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageNum">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="whereString">where语句，无效携带where关键字</param>
        /// <param name="param">参数</param>
        /// <param name="order">排序</param>
        /// <param name="asc">是否</param>
        /// <returns></returns>
        public IPage<T> QueryList(int pageNum, int pageSize, string whereString = null, object param = null, string order = null, bool asc = false)
        {
            IPage<T> paging = new Paging<T>(pageNum, pageSize);
            ISqlAdapter adapter = ConnectionBuilder.GetAdapter();
            var sqlbuilder = adapter.GetPageList(this, pageNum, pageSize, whereString, param, order, asc);
            paging.data = DBConnection.Query<T>(sqlbuilder.SQL, sqlbuilder.Arguments).ToList();
            paging.pageCount = QueryCount(whereString, param);
            return paging;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        public bool Update(T entity)
        {
            return DBConnection.Update<T>(entity, _dbTransaction);
        }

        /// <summary>
        /// 执行单条语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parms"></param>
        public int Execute(string sql, dynamic parms)
        {
            return DBConnection.Execute(sql, (object)parms, transaction: _dbTransaction);
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="entities">entityList</param>
        /// <returns></returns>
        public virtual bool InsertBatch(string sql, IEnumerable<T> entities)
        {
            using (IDbTransaction trans = OpenTransaction())
            {
                try
                {
                    int res = Execute(sql, entities);
                    if (res > 0)
                    {
                        trans.Commit();
                        TrancationState = ETrancationState.Closed;
                        return true;
                    }
                    else
                    {
                        trans.Rollback();
                        TrancationState = ETrancationState.Closed;
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    TrancationState = ETrancationState.Closed;
                    return false;
                }
            }
        }


        /// <summary>
        /// 删除
        /// </summary>
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
        /// 插入
        /// </summary>
        public async Task<int> InsertAsync(T entity)
        {
            return await DBConnection.InsertAsync(entity, _dbTransaction);
        }

        /// <summary>
        /// 删除
        /// </summary>
        public async Task<bool> DeleteAsync(T entity)
        {
            return await DBConnection.DeleteAsync(entity, _dbTransaction);
        }

        /// <summary>
        /// 更新
        /// </summary>
        public async Task<bool> UpdateAsync(T entity)
        {
            return await DBConnection.UpdateAsync(entity, _dbTransaction);
        }

        /// <summary>
        /// 查询
        /// </summary>
        public async Task<T> QueryAsync(int id)
        {
            return await DBConnection.GetAsync<T>(id);
        }

        /// <summary>
        /// 异步总数
        /// </summary>
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
        /// 异步获取列表
        /// </summary>
        public async Task<IEnumerable<T>> QueryListAsync(string whereString = null, object param = null, string order = null, bool asc = false)
        {
            if (string.IsNullOrEmpty(whereString) && string.IsNullOrEmpty(order))
            {
                return await DBConnection.GetAllAsync<T>();
            }
            else
            {
                ISqlAdapter adapter = ConnectionBuilder.GetAdapter();
                var sqlbuilder = adapter.GetPageList(this, whereString: whereString, param: param, order: order, asc: asc);
                return await DBConnection.QueryAsync<T>(sqlbuilder.SQL, sqlbuilder.Arguments);
            }
        }

        /// <summary>
        /// 异步分页获取
        /// </summary>
        public async Task<IPage<T>> QueryListAsync(int pageNum, int pageSize, string whereString = null, object param = null, string order = null, bool asc = false)
        {
            return await Task.Run(() =>
            {
                IPage<T> paging = new Paging<T>(pageNum, pageSize);
                ISqlAdapter adapter = ConnectionBuilder.GetAdapter();
                var sqlbuilder = adapter.GetPageList(this, pageNum, pageSize, whereString, param, order, asc);
                paging.data = DBConnection.Query<T>(sqlbuilder.SQL, sqlbuilder.Arguments).ToList();
                paging.pageCount = QueryCount(whereString, param);
                return paging;
            }); ;
        }

        /// <summary>
        /// 执行单条语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parms"></param>
        public async Task<int> ExecuteAsync(string sql, dynamic parms)
        {
            return await DBConnection.ExecuteAsync(sql, (object)parms, transaction: _dbTransaction);
        }

        /// <summary>
        /// 删除全部
        /// </summary>
        /// <returns></returns>
        public bool DeleteAll()
        {
            return DBConnection.DeleteAll<T>(_dbTransaction);
        }

        /// <summary>
        /// 删除全部
        /// </summary>
        public async Task<bool> DeleteAllAsync()
        {
            return await DBConnection.DeleteAllAsync<T>(_dbTransaction);
        }

        /// <summary>
        /// 删除
        /// </summary>
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
