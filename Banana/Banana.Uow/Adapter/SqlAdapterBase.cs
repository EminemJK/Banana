using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Uow.Adapter
{
    class SqlAdapterBase
    {
        public virtual string QueryString(string selection, string source, string conditions, string order, string grouping, string having)
        {
            return string.Format("SELECT {0} FROM {1} {2} {3} {4} {5}",
                                 selection, source, conditions, order, grouping, having);
        }
    }
}
