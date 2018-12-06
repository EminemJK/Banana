/***********************************
 * Coder：EminemJK
 * Date：2018-12-06
 **********************************/

using Banana.Uow.Models;

namespace Banana.Uow.Interface
{
    /// <summary>
    /// 定义IRepository接口
    /// </summary>
    public interface IRepositoryFactory
    {
        /// <summary>
        /// Get Repository
        /// </summary>
        IRepository<T> GetRepository<T>() where T : class, IEntity;
    }
}
