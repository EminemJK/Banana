/***********************************
 * Coder：EminemJK
 * Create Date：2018-12-06
 * 
 * Last Update：2018-12-18
 **********************************/

using System.Collections.Generic;

namespace Banana.Uow.Models
{
    /// <summary>
    /// 分页数据|
    /// paging data
    /// </summary>
    public class Paging<T>: IPage<T>
    {
        /// <summary>
        /// 总页数|pageCount
        /// </summary>
        public int pageCount { get; set; }

        /// <summary>
        /// 当前页码|page number
        /// </summary>
        public int pageNo { get; set; }

        /// <summary>
        /// 每页显示记录数|page size
        /// </summary>
        public int pageSize { get; set; }

        /// <summary>
        /// 当前分页数据|data list
        /// </summary>
        public List<T> data { get; set; }

        /// <summary>
        /// 分页数据|
        /// paging data
        /// </summary>
        public Paging()
        {
            data = new List<T>();
        }

        /// <summary>
        /// 分页数据|
        /// paging data
        /// </summary>
        /// <param name="pageNo">page number</param>
        /// <param name="pageSize">page size</param>
        public Paging(int pageNo, int pageSize) : this()
        {
            this.pageNo = pageNo;
            this.pageSize = pageSize;
        }
    }
}
