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
    /// Specifies that this field is a primary key in the database
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
        public KeyAttribute() { }

        public KeyAttribute(string oracleSequence)
        {
            OracleSequence = oracleSequence;
        }

        /// <summary>
        /// Oracle Sequence
        /// </summary>
        public string OracleSequence { get; set; }
    }
}
