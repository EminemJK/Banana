/***********************************
 * Coder：EminemJK
 * Create Date：2018-12-06
 * 
 * Last Update：2018-12-18
 **********************************/
using System;

namespace Banana.Uow.Models
{
    /// <summary>
    /// Specifies that this is a computed column.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ComputedAttribute : Attribute
    {
    }
}
