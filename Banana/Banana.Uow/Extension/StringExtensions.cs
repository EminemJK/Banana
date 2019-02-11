using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Uow.Extension
{
    internal static class StringExtensions
    {
        public static string RevomeThePrefix(this string str, string prefix)
        {
            str = str.TrimStart();
            if (str.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
            {
                return str.Substring(prefix.Length);
            }
            return str;
        }
    }
}
