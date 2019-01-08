using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Uow.Models.QueryEnum
{
    /// <summary>
    /// SQL函数的枚举|An enumeration of the supported aggregate SQL functions. The item names should match the related function names
    /// </summary>
    public enum ESelectFunction
    {
        COUNT,

        DISTINCT,

        SUM,

        MIN,

        MAX,

        AVG
    }
}
