/***********************************
 * Developer: Lio.Huang
 * Create Date：2019-04-03
 **********************************/

using System;
namespace Banana.Uow.Models
{
    /// <summary>
    /// 不做Update操作|Exclude update operation
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ExceptUpdateAttribute : Attribute
    {

    }
}
