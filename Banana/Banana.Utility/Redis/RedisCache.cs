/***********************************
 * Developer: Lio.Huang
 * Date：2018-11-21
 **********************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banana.Utility.Redis
{
    /// <summary>
    ///  Redis缓存
    /// </summary>
    /// <typeparam name="T">被缓存的数据类型</typeparam>
    public class RedisCache<T>
    {
        public RedisCache()
        {
            UseCache = false;
            CacheData = default(T);
        }

        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public bool UseCache { get; set; }

        /// <summary>
        ///  需要缓存的数据
        /// </summary>
        public T CacheData { get; set; }
    }
}
