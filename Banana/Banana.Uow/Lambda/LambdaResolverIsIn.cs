using Banana.Uow.SQLBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Uow.Lambda
{
    partial class LambdaResolver
    {
        public void QueryByIsIn<T>(Expression<Func<T, object>> expression, ISqlBuilder sqlQuery)
        {
            var fieldName = GetColumnName(expression);
            var columnAlias = GetPropertyInfoName(expression);
            _builder.QueryByIsIn(GetTableName<T>(), fieldName, columnAlias, sqlQuery);
        }

        public void QueryByIsIn<T>(Expression<Func<T, object>> expression, IEnumerable<object> values)
        {
            var fieldName = GetColumnName(expression);
            var columnAlias = GetPropertyInfoName(expression);
            _builder.QueryByIsIn(GetTableName<T>(), fieldName, columnAlias, values);
        }

        public void QueryByNotIn<T>(Expression<Func<T, object>> expression, ISqlBuilder sqlQuery)
        {
            var fieldName = GetColumnName(expression);
            var columnAlias = GetPropertyInfoName(expression);
            _builder.Not();
            _builder.QueryByIsIn(GetTableName<T>(), fieldName, columnAlias, sqlQuery);
        }

        public void QueryByNotIn<T>(Expression<Func<T, object>> expression, IEnumerable<object> values)
        {
            var fieldName = GetColumnName(expression);
            var columnAlias = GetPropertyInfoName(expression);
            _builder.Not();
            _builder.QueryByIsIn(GetTableName<T>(), fieldName, columnAlias, values);
        }
    }
}
