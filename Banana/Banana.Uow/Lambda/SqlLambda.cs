using Banana.Uow.Interface;
using Banana.Uow.Models.QueryEnum;
using Banana.Uow.SQLBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Uow.Lambda
{
    public class SqlLambda<T> : SqlLambdaBase
    {
        public SqlLambda(string tableName, ISqlAdapter adapter)
        {
            _builder = new SqlQueryBuilder(tableName, typeof(T), adapter);
            _resolver = new LambdaResolver(_builder);
        }

        public SqlLambda(string tableName, ISqlAdapter adapter, Expression<Func<T, bool>> expression) : this(tableName, adapter)
        {
            Where(expression);
        }

        internal SqlLambda(ISqlBuilder builder, LambdaResolver resolver)
        {
            _builder = builder;
            _resolver = resolver;
        }

        public SqlLambda<T> Where(Expression<Func<T, bool>> expression)
        {
            return And(expression);
        }

        public SqlLambda<T> And(Expression<Func<T, bool>> expression)
        {
            _builder.And();
            _resolver.ResolveQuery(expression);
            return this;
        }

        public SqlLambda<T> Or(Expression<Func<T, bool>> expression)
        {
            _builder.Or();
            _resolver.ResolveQuery(expression);
            return this;
        }

        public SqlLambda<T> WhereIsIn(Expression<Func<T, object>> expression, SqlLambdaBase sqlQuery)
        {
            _builder.And();
            _resolver.QueryByIsIn(expression, sqlQuery.SqlBuilder);
            return this;
        }

        public SqlLambda<T> WhereIsIn(Expression<Func<T, object>> expression, IEnumerable<object> values)
        {
            _builder.And();
            _resolver.QueryByIsIn(expression, values);
            return this;
        }

        public SqlLambda<T> WhereNotIn(Expression<Func<T, object>> expression, SqlLambdaBase sqlQuery)
        {
            _builder.And();
            _resolver.QueryByNotIn(expression, sqlQuery.SqlBuilder);
            return this;
        }

        public SqlLambda<T> WhereNotIn(Expression<Func<T, object>> expression, IEnumerable<object> values)
        {
            _builder.And();
            _resolver.QueryByNotIn(expression, values);
            return this;
        }

        public SqlLambda<T> OrderBy(Expression<Func<T, object>> expression)
        {
            _resolver.OrderBy(expression);
            return this;
        }

        public SqlLambda<T> OrderByDescending(Expression<Func<T, object>> expression)
        {
            _resolver.OrderBy(expression, true);
            return this;
        }

        public SqlLambda<T> Select(params Expression<Func<T, object>>[] expressions)
        {
            foreach (var expression in expressions)
                _resolver.Select(expression);
            return this;
        }

        public SqlLambda<T> SelectCount(Expression<Func<T, object>> expression)
        {
            _resolver.SelectWithFunction(expression, ESelectFunction.COUNT);
            return this;
        }

        public SqlLambda<T> SelectDistinct(Expression<Func<T, object>> expression)
        {
            _resolver.SelectWithFunction(expression, ESelectFunction.DISTINCT);
            return this;
        }

        public SqlLambda<T> SelectSum(Expression<Func<T, object>> expression)
        {
            _resolver.SelectWithFunction(expression, ESelectFunction.SUM);
            return this;
        }

        public SqlLambda<T> SelectMax(Expression<Func<T, object>> expression)
        {
            _resolver.SelectWithFunction(expression, ESelectFunction.MAX);
            return this;
        }

        public SqlLambda<T> SelectMin(Expression<Func<T, object>> expression)
        {
            _resolver.SelectWithFunction(expression, ESelectFunction.MIN);
            return this;
        }

        public SqlLambda<T> SelectAverage(Expression<Func<T, object>> expression)
        {
            _resolver.SelectWithFunction(expression, ESelectFunction.AVG);
            return this;
        }

        public SqlLambda<TResult> Join<T2, TKey, TResult>(SqlLambda<T2> joinQuery,
            Expression<Func<T, TKey>> primaryKeySelector,
            Expression<Func<T, TKey>> foreignKeySelector,
            Func<T, T2, TResult> selection)
        {
            var query = new SqlLambda<TResult>(SqlBuilder, _resolver);
            _resolver.Join<T, T2, TKey>(primaryKeySelector, foreignKeySelector);
            return query;
        }

        public SqlLambda<T2> Join<T2>(Expression<Func<T, T2, bool>> expression)
        {
            var joinQuery = new SqlLambda<T2>(SqlBuilder, _resolver);
            _resolver.Join(expression);
            return joinQuery;
        }

        public SqlLambda<T> GroupBy(Expression<Func<T, object>> expression)
        {
            _resolver.GroupBy(expression);
            return this;
        }
    }
}
