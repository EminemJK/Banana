/***********************************
 * Coder：EminemJK
 * Date：2018-11-16
 **********************************/

using Banana.Uow.Extension;
using Banana.Uow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Uow.Interface
{
    /// <summary>
    /// 数据库各数据的适配扩展接口
    /// </summary>
    public interface IAdapter
    {
        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repository"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <param name="whereString"></param>
        /// <param name="param"></param>
        /// <param name="order"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        SqlBuilder GetPageList<T>(IRepository<T> repository, int pageNum = 0, int pageSize = 0, string whereString = null, object param = null, object order = null, bool asc = false)
           where T : class, IEntity;
    }
}
