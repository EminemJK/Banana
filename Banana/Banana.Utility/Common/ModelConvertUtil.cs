/***********************************
 * Coder：EminemJK
 * Date：2018-11-21
 * 
 * Last Update：2018-12-18
 **********************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Utility.Common
{
    /// <summary>
    /// 模型拷贝 -（利用表达树进行，提高效率）|The model copy
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public class ModelConvertUtil<TIn, TOut>
    {
        private static readonly Func<TIn, TOut> cache = GetFunc();
        private static Func<TIn, TOut> GetFunc()
        {

            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "model");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();
            var inFilds = typeof(TIn).GetProperties().ToList();
            foreach (var item in typeof(TOut).GetProperties())
            {
                if (!item.CanWrite)
                    continue;

                var inFild = inFilds.Find(f => f.Name.ToLower() == item.Name.ToLower());
                if (inFild == null)
                    continue;
                MemberExpression property = Expression.Property(parameterExpression, inFild);
                MemberBinding memberBinding = Expression.Bind(item, property);
                memberBindingList.Add(memberBinding);
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });
            return lambda.Compile();
        }

        /// <summary>
        /// 模型转换
        /// </summary>
        public static TOut ModelCopy(TIn tIn)
        {
            return cache(tIn);
        }

        /// <summary>
        /// 模型转换
        /// </summary>
        public static List<TOut> ModelCopy(List<TIn> sList)
        {
            List<TOut> list = new List<TOut>();
            foreach (var sModel in sList)
            {
                var t = ModelCopy(sModel);
                list.Add(t);
            }
            return list;
        }
    }
}
