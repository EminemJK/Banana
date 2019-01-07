/***********************************
 * Developer: Lio.Huang
 * Date：2018-12-06
 * 
 * Last Update：2018-12-18
 **********************************/

using System.Collections.Generic;

namespace Banana.Uow.Models
{
    /// <summary>
    /// 分页数据接口|
    /// The interface for paging
    /// </summary>
    public interface IPage<T>
    {
        /// <summary>
        /// All data rows
        /// </summary>
        int dataCount { get; set; }

        /// <summary>
        ///pageCount
        /// </summary>
        int pageCount { get; }

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
