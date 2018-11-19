using Banana.Uow.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Banana.Uow.Interface
{
    /// <summary>
    /// Coder：EminemJK 
    /// Date：2018-11-16
    /// 工作单元接口
    /// </summary>
    public interface IUnitOfWork : IDisposable 
    {
        void Commit();

        void Rollback();

        IRepository<T> Repository<T>() where T : class, IEntity;
    }
}
