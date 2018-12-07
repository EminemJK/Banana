/***********************************
 * Coder：EminemJK
 * Date：2018-11-16
 **********************************/

using Banana.Uow.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Uow.Interface
{
    /// <summary>
    /// 仓储接口
    public interface IRepository<T> where T : class, IEntity
    {
        #region Sync
        /// <summary>
        /// 插入对象
        /// </summary>
        long Insert(T entity);

        /// <summary>
        /// 更新对象
        /// </summary>
        bool Update(T entity);

        /// <summary>
        /// 删除对象
        /// </summary>
        bool Delete(T entity);

        /// <summary>
        /// 删除对象
        /// </summary>
        bool Delete(string whereString, object param);

        /// <summary>
        /// 删除全部
        /// </summary>
        bool DeleteAll();

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        int Execute(string sql, dynamic parms = null);

        /// <summary>
        /// 查询对象
        /// </summary>
        T Query(int id);

        /// <summary>
        /// 查询总数
        /// </summary>
        int QueryCount(string whereString = null, object param = null);

        /// <summary>
        /// 查询对象集合
        /// </summary>
        List<T> QueryList(string whereString = null, object param = null, string order = null, bool asc = false);

        /// <summary>
        /// 查询对象集合
        /// </summary>
        IPage<T> QueryList(int pageNum, int pageSize, string whereString = null, object param = null, string order = null, bool asc = false);

        #endregion

        #region Async
        /// <summary>
        /// 插入实体
        /// </summary>
        Task<int> InsertAsync(T entity);

        /// <summary>
        /// 删除对象
        /// </summary>
        Task<bool> DeleteAsync(T entity);

        /// <summary>
        /// 删除对象
        /// </summary>
        Task<bool> DeleteAsync(string whereString, object param);

        /// <summary>
        /// 删除全部
        /// </summary>
        Task<bool> DeleteAllAsync();

        /// <summary>
        /// 更新对象
        /// </summary>
        Task<bool> UpdateAsync(T entity);

        /// <summary>
        /// 查询
        /// </summary>
        Task<T> QueryAsync(int id);

        /// <summary>
        /// 查询总数
        /// </summary>
        Task<int> QueryCountAsync(string whereString = null, object param = null);

        /// <summary>
        /// 查询对象集合
        /// </summary>
        Task<IEnumerable<T>> QueryListAsync(string whereString = null, object param = null, string order = null, bool asc = false);

        /// <summary>
        /// 查询对象集合
        /// </summary>
        Task<IPage<T>> QueryListAsync(int pageNum, int pageSize, string whereString = null, object param = null, string order = null, bool asc = false);

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        Task<int> ExecuteAsync(string sql, dynamic parms = null);
        #endregion

        /// <summary>
        /// 表名
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// DBConnection
        /// </summary>
        IDbConnection DBConnection { get; }

        /// <summary>
        /// 开启事务
        /// </summary>
        IDbTransaction OpenTransaction();

        /// <summary>
        /// 事务状态
        /// </summary>
        ETrancationState TrancationState { get; }

        /// <summary>
        /// 对象类型
        /// </summary>
        Type EntityType { get; }
    }
}
