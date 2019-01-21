/***********************************
 * Developer: Lio.Huang
 * Create Date：2018-11-16
 * 
 * Last Update：
 * 2019-01-07  1.Add this.context.State != ConnectionState.Connecting
 * 2019-01-21  1.Add UnitOfWork(string dbKey = "")
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
    /// 工作单元基类|
    /// Base UnitOfWork
    /// </summary>
    public class UnitOfWork : IUnitOfWork, IRepositoryFactory
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
        /// 工作单元基类|
        /// Base UnitOfWork
        /// </summary>
        public UnitOfWork(IDbConnection context)
        {
            this.context = context;
            if (this.context.State == ConnectionState.Closed && this.context.State != ConnectionState.Connecting)
            {
                this.context.Open();
            }
            transaction = this.context.BeginTransaction();
        }

        /// <summary>
        /// 工作单元基类|
        /// Base UnitOfWork
        /// </summary>
        public UnitOfWork(string dbKey = "")
        {
            this.context = ConnectionBuilder.CreateConnection(dbKey);
            this.context.Open();
            this.transaction = this.context.BeginTransaction();
        }

        /// <summary>
        /// 提交事务|
        /// transaction commit
        /// </summary>
        public void Commit()
        {
            this.transaction.Commit();
        }

        /// <summary>
        /// 事务回滚|
        /// transaction rollback
        /// </summary>
        public void Rollback()
        {
            this.transaction.Rollback();
        }

        /// <summary>
        /// 获取仓储|
        /// Get a repository
        /// </summary> 
        public IRepository<T> GetRepository<T>() where T : class, IEntity
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
