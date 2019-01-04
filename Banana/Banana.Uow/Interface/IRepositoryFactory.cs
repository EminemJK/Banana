/***********************************
 * Developer: Lio.Huang
 * Date：2018-12-06
 * 
 * Last Update：2018-12-18
 **********************************/

using Banana.Uow.Models;

namespace Banana.Uow.Interface
{
    /// <summary>
    /// 定义IRepository接口|The interface for repository's factory  
    /// </summary>
    public interface IRepositoryFactory
    {
        /// <summary>
        /// Get Repository
        /// </summary>
        IRepository<T> GetRepository<T>() where T : class, IEntity;
    }
}
