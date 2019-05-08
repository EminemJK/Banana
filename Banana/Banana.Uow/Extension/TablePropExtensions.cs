/***********************************
 * Developer: Lio.Huang
 * Create Date：2019-03-22
 * 
 * Last Update：
 * 2019-03-22  1.模型属性的支持扩展   
 **********************************/
using Banana.Uow.Models;
using System; 
namespace Banana.Uow.Extension
{
    /// <summary>
    /// Table property Extensions
    /// </summary>
    public static class TablePropExtensions<T> where T : class, IEntity
    {
        /// <summary>
        /// Entity Type
        /// </summary>
        public static Type EntityType => typeof(T);

        /// <summary>
        /// Get TableName
        /// </summary>
        public static string TableName
        {
            get
            {
                return SqlMapperExtensions.GetTableName(EntityType);
            }
        }
    }
}
