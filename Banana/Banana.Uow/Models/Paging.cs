using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Uow.Models
{
    /// <summary>
    /// 分页数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Paging<T>
    {
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
        /// 当前分页数据
        /// </summary>
        public List<T> data { get; set; }

        public Paging()
        {
            data = new List<T>();
        }

        public Paging(int pageNo, int pageSize) : this()
        {
            this.pageNo = pageNo;
            this.pageSize = pageSize;
        }
    }
}
