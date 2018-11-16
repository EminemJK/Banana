using Banana.Uow.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Banana.Uow.Interface
{
    /// <summary>
    /// Coder：EminemJK 
    /// Date：2018-11-16
    /// 仓储接口
    public interface IRepository<T> where T : class, IEntity
    {
        /// <summary>
        /// 插入对象
        /// </summary>
        /// <param name="entity"></param>
        long Insert(T entity, IDbTransaction dbTransaction = null);

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <param name="entity"></param>
        bool Update(T entity, IDbTransaction dbTransaction = null);

        /// <summary>
        /// 删除对象
        /// </summary>
        bool Delete(T entity, IDbTransaction dbTransaction = null);

        /// <summary>
        /// 查询对象集合
        /// </summary>
        List<T> QueryAll();

        /// <summary>
        /// 查询对象集合
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="pagesize"></param>
        /// <param name="order"></param>
        /// <param name="asc"></param>
        /// <param name="express"></param>
        /// <returns></returns>
        List<T> QueryAll(int pageNum, int pagesize, string order = null, bool asc = false);

        /// <summary>
        /// 查询对象
        /// </summary>
        T Query(int id);

        string TableName { get; }

        IDbTransaction OpenTransaction { get; }
    }
}
