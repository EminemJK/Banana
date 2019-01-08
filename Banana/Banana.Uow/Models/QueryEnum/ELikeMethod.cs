using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Uow.Models.QueryEnum
{
    /// <summary>
    /// Like语法枚举|An enumeration of the supported string methods for the SQL LIKE statement.
    /// </summary>
    public enum ELikeMethod
    {
        StartsWith,

        EndsWith,

        Contains,

        Equals
    }
}
