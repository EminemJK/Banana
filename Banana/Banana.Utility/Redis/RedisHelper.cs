/***********************************
 * Developer: Lio.Huang
 * Date：2018-11-21
 **********************************/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Banana.Utility.Redis
{
    /// <summary>
    /// Redis帮助类
    /// </summary>
    public class RedisHelper
    {
        /// <summary>
        /// 缓存失效时长
        /// </summary>
        public const int EXPIRY = 30;

        private static int CheckDbIndex(int dbIndex)
        {
            if (dbIndex > 16 || dbIndex < 0)
            {
                dbIndex = 0;
            }
            return dbIndex;
        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbIndex">Redis数据库索引</param>
        /// <param name="key">Redis键</param>
        /// <param name="fun">从其他地方获取数据源，并缓存到Redis中</param>
        /// <param name="timeout">过期时间，单位：分钟, Null将不过期</param>
        /// <returns></returns>
        public static T GetObject<T>(int dbIndex, string key, Func<T> fun, int? timeout = EXPIRY) where T : class
        {
            dbIndex = CheckDbIndex(dbIndex);
            T data = RedisUtils.StringGet<T>(dbIndex, key);
            if (data != null)
            {
                return data;
            }
            if (fun != null)
            {
                data = fun();
            }
            if (data != null)
            {
                TimeSpan? timeSp = null;
                if (timeout != null)
                    timeSp = TimeSpan.FromMinutes(Convert.ToDouble(timeout));
                RedisUtils.StringSet<T>(dbIndex, key, data, timeSp);
            }
            return data;
        }

        /// <summary>
        /// KV
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbIndex">库</param>
        /// <param name="key">键</param>
        /// <param name="func">如找不到则从func获取</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public static T GetObject_KV<T>(int dbIndex, string key, Func<T> func, TimeSpan? timeout) where T : class
        {
            T data = RedisUtils.StringGet<T>(dbIndex, key);
            if (data != null)
            {
                return data;
            }
            if (func != null)
            {
                data = func();
            }
            if (data != null)
            {
                RedisUtils.StringSet<T>(dbIndex, key, data, timeout);
            }
            return data;
        }

        /// <summary>
        /// 异步获取缓存数据
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="dbIndex">数据库</param>
        /// <param name="key">键</param>
        /// <param name="fun">从其他地方获取数据源，并缓存到Redis中</param>
        /// <param name="timeout">过期时间，单位：分钟</param>
        /// <returns></returns>
        public static async Task<T> GetObjectAsync<T>(int dbIndex, string key, Func<T> fun, int timeout = EXPIRY) where T : class
        {
            dbIndex = CheckDbIndex(dbIndex);
            T data = RedisUtils.StringGet<T>(dbIndex, key);
            if (data != null)
            {
                return data;
            }

            if (fun != null)
            {
                data = await Task.Run(() =>
                {
                    return fun();
                });
            }
            if (data != null)
            {
                RedisUtils.StringSet<T>(dbIndex, key, data, TimeSpan.FromMinutes(timeout));
            }
            return data;
        }

        /// <summary>
        /// 异步获取缓存数据
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="dbIndex">数据库</param>
        /// <param name="key">键</param>
        /// <param name="fun">从其他地方获取数据源，并缓存到Redis中</param>
        /// <param name="timeout">过期时间，单位：分钟</param>
        /// <returns></returns>
        public static async Task<T> GetObjectAsync<T>(int dbIndex, string key, Func<RedisCache<T>> fun, int timeout = EXPIRY) where T : class
        {
            dbIndex = CheckDbIndex(dbIndex);
            RedisCache<T> cache = new RedisCache<T>();
            cache.CacheData = RedisUtils.StringGet<T>(dbIndex, key);
            if (cache.CacheData != null)
            {
                return cache.CacheData;
            }

            var temp = await Task.Run(() =>
            {
                return fun();
            });

            if (temp != null) cache = temp;

            if (cache.UseCache)
            {
                RedisUtils.StringSet<T>(dbIndex, key, cache.CacheData, TimeSpan.FromMinutes(timeout));
            }
            return cache.CacheData;
        }

        /// <summary>
        /// 异步获取数据集合
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="dbIndex">数据库</param>
        /// <param name="key">键</param>
        /// <param name="fun">从其他地方获取数据源，并缓存到Redis中</param>
        /// <param name="timeout">过期时间，单位：分钟</param>
        /// <returns></returns>
        public static async Task<List<T>> GetListAsync<T>(int dbIndex, string key, Func<List<T>> fun, int timeout = EXPIRY) where T : class
        {
            dbIndex = CheckDbIndex(dbIndex);
            List<T> datas = RedisUtils.StringGet<List<T>>(dbIndex, key);
            if (datas != null && datas.Count > 0)
            {
                return datas;
            }

            datas = await Task.Run(() =>
            {
                return fun();
            });

            if (datas != null && datas.Count > 0)
            {
                RedisUtils.StringSet<List<T>>(dbIndex, key, datas, TimeSpan.FromMinutes(timeout));
            }
            return datas;
        }

        /// <summary>
        /// ZSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbIndex">库</param>
        /// <param name="key">键</param>
        /// <param name="func">如找不到则从func获取</param>
        /// <returns></returns>
        public static List<T> GetObject_ZSet<T>(int dbIndex, string key, Func<List<T>> func) where T : class
        {
            List<T> data = RedisUtils.SortedSetRangeByRank<T>(dbIndex, key);
            if (data != null && data.Count > 0)
            {
                return data;
            }
            if (func != null)
            {
                data = func();
            }
            if (data != null)
            {
                RedisUtils.SortedSetAdd<T>(dbIndex, key, data);
            }
            return data;
        }


        /// <summary>
        /// Hash
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbIndex">库</param>
        /// <param name="hashID">hashID</param>
        /// <param name="key">键</param>
        /// <param name="func">如找不到则从func获取</param>
        /// <returns></returns>
        public static T GetObject_Hash<T>(int dbIndex, string hashID, string key, Func<T> func) where T : class
        {
            T data = RedisUtils.HashGet<T>(dbIndex, hashID, key);
            if (data != null)
            {
                return data;
            }
            if (func != null)
            {
                data = func();
            }
            if (data != null)
            {
                RedisUtils.HashSet<T>(dbIndex, hashID, key, data);
            }
            return data;
        }
    }
}
