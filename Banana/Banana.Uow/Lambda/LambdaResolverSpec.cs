using Banana.Uow.Models.QueryEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Banana.Uow.Lambda
{
    partial class LambdaResolver
    {
        public void Join<T1, T2>(Expression<Func<T1, T2, bool>> expression)
        {
            var joinExpression = GetBinaryExpression(expression.Body);
            var leftExpression = GetMemberExpression(joinExpression.Left);
            var rightExpression = GetMemberExpression(joinExpression.Right);

            Join<T1, T2>(leftExpression, rightExpression);
        }

        public void Join<T1, T2, TKey>(Expression<Func<T1, TKey>> leftExpression, Expression<Func<T1, TKey>> rightExpression)
        {
            Join<T1, T2>(GetMemberExpression(leftExpression.Body), GetMemberExpression(rightExpression.Body));
        }

        public void Join<T1, T2>(MemberExpression leftExpression, MemberExpression rightExpression)
        {
            _builder.Join(GetTableName<T1>(), GetTableName<T2>(), GetColumnName(leftExpression), GetColumnName(rightExpression));
        }

        public void OrderBy<T>(Expression<Func<T, object>> expression, bool desc = false)
        {
            var fieldName = GetColumnName(GetMemberExpression(expression.Body));
            _builder.OrderBy(GetTableName<T>(), fieldName, desc);
        }

        public void Select<T>(Expression<Func<T, object>> expression)
        {
            Select<T>(expression.Body);
        }

        private void Select<T>(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Parameter:
                    _builder.Query(expression.Type);
                    break;
                case ExpressionType.Convert:
                case ExpressionType.MemberAccess:
                    Select<T>(GetMemberExpression(expression));
                    break;
                case ExpressionType.New:
                    foreach (MemberExpression memberExp in (expression as NewExpression).Arguments)
                    {
                        Select<T>(memberExp);
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid expression");
            }
        }

        private void Select<T>(MemberExpression expression)
        {
            if (expression.Type.IsClass && expression.Type != typeof(String))
                _builder.Query(expression.Type);
            else
                _builder.Query(GetTableName<T>(), GetColumnName(expression), GetPropertyInfoName(expression));
        }

        public void SelectWithFunction<T>(Expression<Func<T, object>> expression, ESelectFunction selectFunction)
        {
            SelectWithFunction<T>(expression.Body, selectFunction);
        }

        private void SelectWithFunction<T>(Expression expression, ESelectFunction selectFunction)
        {
            var fieldName = GetColumnName(GetMemberExpression(expression));
            var columnAlias = GetPropertyInfoName(expression);
            _builder.Query(GetTableName<T>(), fieldName, columnAlias, selectFunction);
        }

        public void GroupBy<T>(Expression<Func<T, object>> expression)
        {
            GroupBy<T>(GetMemberExpression(expression.Body));
        }

        private void GroupBy<T>(MemberExpression expression)
        {
            var fieldName = GetColumnName(GetMemberExpression(expression));
            _builder.GroupBy(GetTableName<T>(), fieldName);
        }
    }
}
