using Banana.Uow.Interface;
using Banana.Uow.SQLBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Uow.Lambda
{
    public class SqlLambdaBase
    {
        internal static ISqlAdapter _defaultAdapter;
        internal ISqlBuilder _builder;
        internal LambdaResolver _resolver;

        internal ISqlBuilder SqlBuilder => _builder;

        public string SQL => _builder.SQL;

        public string QueryStringPage(int pageSize, int? pageNumber = null)
        {
            return _builder.QueryStringPage(pageSize, pageNumber);
        }

        public IDictionary<string, object> QueryParameters => _builder.Parameters;

        public string[] SplitColumns => _builder.SplitColumns.ToArray();

        public static void SetAdapter(ISqlAdapter adapter)
        {
            _defaultAdapter = adapter;
        }
    }
}
