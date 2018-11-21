/***********************************
 * Coder：EminemJK
 * Date：2018-11-21
 **********************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Utility.Common
{
    public class PagingUtil<T> : List<T>
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public int dataCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int pageCount { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int pageNo { get; set; }

        /// <summary>
        /// 每页显示记录数
        /// </summary>
        public int pageSize { get; set; }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPreviousPage
        {
            get { return pageNo > 1; }
        }

        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNextPage
        {
            get { return pageNo < this.pageCount; }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNo"></param>
        public PagingUtil(List<T> dataList, int pageSize, int pageNo)
        {
            this.pageSize = pageSize;
            this.pageNo = pageNo;
            this.dataCount = dataList.Count;
            this.pageCount = (int)Math.Ceiling((decimal)this.dataCount / pageSize);
            this.AddRange(dataList.Skip((pageNo - 1) * pageSize).Take(pageSize));
        }
    }
}
