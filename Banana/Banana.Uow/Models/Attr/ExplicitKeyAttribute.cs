/***********************************
 * Developer: Lio.Huang
 * Create Date：2018-12-06
 * 
 * Last Update：2018-12-18
 **********************************/
using System;

namespace Banana.Uow.Models
{
    /// <summary>
    /// Specifies that this field is a explicitly set primary key in the database
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ExplicitKeyAttribute : Attribute
    {
    }
}
