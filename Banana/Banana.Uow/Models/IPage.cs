using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
