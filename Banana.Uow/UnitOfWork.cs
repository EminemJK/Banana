using Banana.Uow.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Banana.Uow.Interface;

namespace Banana.Uow
{
    /// <summary>
    /// Coder：EminemJK 
    /// Date：2018-11-16
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

        public UnitOfWork()
        {
            context =  ConnectionBuilder.OpenConnection();
            transaction = context.BeginTransaction();
        }

        public UnitOfWork(IDbConnection context)
        {
            this.context = context;
            transaction = context.BeginTransaction();
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
        public IRepository<T> Repository<T>(T entity) where T : class, IEntity
        { 
            if (repositories == null)
            {
                repositories = new Dictionary<Type, object>();
            }
            var type = typeof(T);
            if (!repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), context);
                repositories.Add(type, repositoryInstance);
            } 
            return (Repository<T>)repositories[type];
        }

    }
}
