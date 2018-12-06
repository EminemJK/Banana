/***********************************
 * Coder：EminemJK
 * Date：2018-12-06
 **********************************/

using System.Collections.Generic;

namespace Banana.Uow.Models
{
    /// <summary>
    /// 分页数据接口
    /// </summary>
    public interface IPage<T>
    {
        /// <summary>
        ///pageCount
        /// </summary>
        int pageCount { get; set; }

        /// <summary>
        /// pageNo
        /// </summary>
        int pageNo { get; set; }

        /// <summary>
        /// pageSize
        /// </summary>
        int pageSize { get; set; }

        /// <summary>
        /// data
        /// </summary>
        List<T> data { get; set; }
    }

}
