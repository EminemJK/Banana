using Banana.Uow.Lambda.ExperssionTress;
using Banana.Uow.Models.QueryEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Banana.Uow.Lambda
{
    partial class LambdaResolver
    {
        public void ResolveQuery<T>(Expression<Func<T, bool>> expression)
        {
            var expressionTree = ResolveQuery((dynamic)expression.Body);
            BuildSql(expressionTree);
        }

        private Node ResolveQuery(ConstantExpression constantExpression)
        {
            return new ValueNode() { Value = constantExpression.Value };
        }

        private Node ResolveQuery(UnaryExpression unaryExpression)
        {
            return new SingleOperationNode()
            {
                Operator = unaryExpression.NodeType,
                Child = ResolveQuery((dynamic)unaryExpression.Operand)
            };
        }

        private Node ResolveQuery(BinaryExpression binaryExpression)
        {
            return new OperationNode
            {
                Left = ResolveQuery((dynamic)binaryExpression.Left),
                Operator = binaryExpression.NodeType,
                Right = ResolveQuery((dynamic)binaryExpression.Right)
            };
        }

        private Node ResolveQuery(MethodCallExpression callExpression)
        {
            ELikeMethod callFunction;
            if (Enum.TryParse(callExpression.Method.Name, true, out callFunction))
            {
                var member = callExpression.Object as MemberExpression;
                var fieldValue = (string)GetExpressionValue(callExpression.Arguments.First());

                return new LikeNode()
                {
                    MemberNode = new MemberNode()
                    {
                        TableName = GetTableName(member),
                        FieldName = GetColumnName(callExpression.Object),
                        ColumnAlias = GetPropertyInfoName(callExpression.Object)
                    },
                    Method = callFunction,
                    Value = fieldValue
                };
            }
            else
            {
                var value = ResolveMethodCall(callExpression);
                return new ValueNode() { Value = value };
            }
        }

        private Node ResolveQuery(MemberExpression memberExpression, MemberExpression rootExpression = null)
        {
            rootExpression = rootExpression ?? memberExpression;
            switch (memberExpression.Expression.NodeType)
            {
                case ExpressionType.Parameter:
                    return new MemberNode()
                    { TableName = GetTableName(rootExpression), FieldName = GetColumnName(rootExpression), ColumnAlias= GetPropertyInfoName(rootExpression) };
                case ExpressionType.MemberAccess:
                    return ResolveQuery(memberExpression.Expression as MemberExpression, rootExpression);
                case ExpressionType.Call:
                case ExpressionType.Constant:
                    return new ValueNode() { Value = GetExpressionValue(rootExpression) };
                default:
                    throw new ArgumentException("Expected member expression");
            }
        }

        #region Helpers

        private object GetExpressionValue(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    return (expression as ConstantExpression).Value;
                case ExpressionType.Call:
                    return ResolveMethodCall(expression as MethodCallExpression);
                case ExpressionType.MemberAccess:
                    var memberExpr = (expression as MemberExpression);
                    var obj = GetExpressionValue(memberExpr.Expression);
                    return ResolveValue((dynamic)memberExpr.Member, obj);
                default:
                    throw new ArgumentException("Expected constant expression");
            }
        }

        private object ResolveMethodCall(MethodCallExpression callExpression)
        {
            var arguments = callExpression.Arguments.Select(GetExpressionValue).ToArray();
            var obj = callExpression.Object != null ? GetExpressionValue(callExpression.Object) : arguments.First();

            return callExpression.Method.Invoke(obj, arguments);
        }

        private object ResolveValue(PropertyInfo property, object obj)
        {
            return property.GetValue(obj, null);
        }

        private object ResolveValue(FieldInfo field, object obj)
        {
            return field.GetValue(obj);
        }

        #endregion

        #region Fail functions

        private void ResolveQuery(Expression expression)
        {
            throw new ArgumentException(string.Format("The provided expression '{0}' is currently not supported", expression.NodeType));
        }

        #endregion
    }
}
