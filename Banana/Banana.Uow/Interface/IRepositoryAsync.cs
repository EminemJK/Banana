/***********************************
 * Coder：EminemJK
 * Date：2018-12-04
 **********************************/

using Banana.Uow.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Uow.Interface
{
    /// <summary>
    /// 异步仓储接口
    /// </summary>
    public interface IRepositoryAsync<T> where T : class, IEntity
    {
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
        Task<IEnumerable<T>> QueryListAsync(string whereString = null, object param = null);

        /// <summary>
        /// 查询对象集合
        /// </summary>
        Task<Paging<T>> QueryListAsync(int pageNum, int pageSize, string whereString = null, object param = null, string order = null, bool asc = false);

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        Task<int> ExecuteAsync(string sql, dynamic parms = null);

        /// <summary>
        /// 表名
        /// </summary>
        string TableName { get; }

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
