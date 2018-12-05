/***********************************
 * Coder：EminemJK
 * Date：2018-11-16
 **********************************/

using Banana.Uow.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Banana.Uow.Interface;

namespace Banana.Uow
{
    /// <summary>
    /// 工作单元基类
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private Dictionary<Type, object> repositories;
        private readonly IDbConnection context;
        private readonly IDbTransaction transaction;

        #region Disposed
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //clear repo
                    if (repositories != null)
                    {
                        repositories.Clear();
                    }
                    if (context.State != ConnectionState.Closed)
                        context.Close();
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// 工作单元基类
        /// </summary>
        /// <param name="context"></param>
        public UnitOfWork(IDbConnection context = null)
        {
            this.context = context;
            if (this.context == null)
            {
                this.context = ConnectionBuilder.CreateConnection();
            }
            if (this.context.State == ConnectionState.Closed)
            {
                this.context.Open();
            }
            transaction = this.context.BeginTransaction();
        }

        /// <summary>
        /// 提交
        /// </summary>
        public void Commit()
        {
            this.transaction.Commit();
        }

        /// <summary>
        /// 回滚
        /// </summary>
        public void Rollback()
        {
            this.transaction.Rollback();
        }

        /// <summary>
        /// 获取仓储
        /// </summary> 
        public IRepository<T> Repository<T>() where T : class, IEntity
        { 
            if (repositories == null)
            {
                repositories = new Dictionary<Type, object>();
            }
            var type = typeof(T);
            if (!repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), context, transaction);
                repositories.Add(type, repositoryInstance);
            } 
            return (Repository<T>)repositories[type];
        }

    }
}
