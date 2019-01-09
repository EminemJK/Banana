using Banana.Uow.Models;
using Banana.Uow.SQLBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Banana.Uow.Lambda
{
    partial class LambdaResolver
    {
        private Dictionary<ExpressionType, string> _operationDictionary = new Dictionary<ExpressionType, string>()
                                                                              {
                                                                                  { ExpressionType.Equal, "="},
                                                                                  { ExpressionType.NotEqual, "!="},
                                                                                  { ExpressionType.GreaterThan, ">"},
                                                                                  { ExpressionType.LessThan, "<"},
                                                                                  { ExpressionType.GreaterThanOrEqual, ">="},
                                                                                  { ExpressionType.LessThanOrEqual, "<="}
                                                                              };

        private ISqlBuilder _builder { get; set; }

        public LambdaResolver(ISqlBuilder builder)
        {
            _builder = builder;
        }

        #region helpers
        public static string GetColumnName<T>(Expression<Func<T, object>> selector)
        {
            return GetColumnName(GetMemberExpression(selector.Body));
        }

        public static string GetColumnName(Expression expression)
        {
            var member = GetMemberExpression(expression);
            var column = member.Member.GetCustomAttributes(false).OfType<ColumnAttribute>().FirstOrDefault();
            if (column != null)
                return column.ColumnName;
            else
                return member.Member.Name;
        }

        public static string GetPropertyInfoName(Expression expression)
        {
            var member = GetMemberExpression(expression);
            return member.Member.Name;
        }

        public static string GetTableName<T>()
        {
            return GetTableName(typeof(T));
        }

        private static string GetTableName(MemberExpression expression)
        {
            return GetTableName(expression.Member.DeclaringType);
        }

        private static BinaryExpression GetBinaryExpression(Expression expression)
        {
            if (expression is BinaryExpression)
                return expression as BinaryExpression;

            throw new ArgumentException("Binary expression expected");
        }
        public static string GetTableName(Type type)
        {
            return Extension.SqlMapperExtensions.GetTableName(type);
        }

        private static MemberExpression GetMemberExpression(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Convert:
                    return GetMemberExpression((expression as UnaryExpression).Operand);
                case ExpressionType.Equal:
                case ExpressionType.MemberAccess:
                    return expression as MemberExpression;
                case ExpressionType.Lambda:
                    return GetMemberExpression((expression as LambdaExpression).Body);
            }

            throw new ArgumentException("Member expression expected");
        }
        #endregion
    }
}
