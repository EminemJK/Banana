using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
