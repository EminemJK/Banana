/***********************************
 * Coder：EminemJK
 * Date：2018-11-16
 **********************************/

using Banana.Uow.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Banana.Uow.Interface
{
    /// <summary>
    /// 工作单元接口
    /// </summary>
    public interface IUnitOfWork : IDisposable 
    {
        /// <summary>
        /// Commit
        /// </summary>
        void Commit();

        /// <summary>
        /// Rollback
        /// </summary>
        void Rollback();

        /// <summary>
        /// Get Repository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IRepository<T> GetRepository<T>() where T : class, IEntity;
    }
}
