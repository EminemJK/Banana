using Banana.Uow.Lambda.ExperssionTress;
using Banana.Uow.Models.QueryEnum;
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
        void BuildSql(Node node)
        {
            BuildSql((dynamic)node);
        }

        void BuildSql(LikeNode node)
        {
            if (node.Method == ELikeMethod.Equals)
            {
                _builder.QueryByField(node.MemberNode.TableName, node.MemberNode.FieldName, node.MemberNode.ColumnAlias,
                    _operationDictionary[ExpressionType.Equal], node.Value);
            }
            else
            {
                string value = node.Value;
                switch (node.Method)
                {
                    case ELikeMethod.StartsWith:
                        value = node.Value + "%";
                        break;
                    case ELikeMethod.EndsWith:
                        value = "%" + node.Value;
                        break;
                    case ELikeMethod.Contains:
                        value = "%" + node.Value + "%";
                        break;
                }
                _builder.QueryByFieldLike(node.MemberNode.TableName, node.MemberNode.FieldName, node.MemberNode.ColumnAlias, value);
            }
        }

        void BuildSql(OperationNode node)
        {
            BuildSql((dynamic)node.Left, (dynamic)node.Right, node.Operator);
        }

        void BuildSql(MemberNode memberNode)
        {
            _builder.QueryByField(memberNode.TableName, memberNode.FieldName, memberNode.ColumnAlias, _operationDictionary[ExpressionType.Equal], true);
        }

        void BuildSql(SingleOperationNode node)
        {
            if (node.Operator == ExpressionType.Not)
                _builder.Not();
            BuildSql(node.Child);
        }

        void BuildSql(MemberNode memberNode, ValueNode valueNode, ExpressionType op)
        {
            if (valueNode.Value == null)
            {
                ResolveNullValue(memberNode, op);
            }
            else
            {
                _builder.QueryByField(memberNode.TableName, memberNode.FieldName,  memberNode.ColumnAlias, _operationDictionary[op], valueNode.Value);
            }
        }

        void BuildSql(ValueNode valueNode, MemberNode memberNode, ExpressionType op)
        {
            BuildSql(memberNode, valueNode, op);
        }

        void BuildSql(MemberNode leftMember, MemberNode rightMember, ExpressionType op)
        {
            _builder.QueryByFieldComparison(leftMember.TableName, leftMember.FieldName, leftMember.ColumnAlias, _operationDictionary[op], rightMember.TableName, rightMember.FieldName);
        }

        void BuildSql(SingleOperationNode leftMember, Node rightMember, ExpressionType op)
        {
            if (leftMember.Operator == ExpressionType.Not)
                BuildSql(leftMember as Node, rightMember, op);
            else
                BuildSql((dynamic)leftMember.Child, (dynamic)rightMember, op);
        }

        void BuildSql(Node leftMember, SingleOperationNode rightMember, ExpressionType op)
        {
            BuildSql(rightMember, leftMember, op);
        }

        void BuildSql(Node leftNode, Node rightNode, ExpressionType op)
        {
            _builder.BeginExpression();
            BuildSql((dynamic)leftNode);
            ResolveOperation(op);
            BuildSql((dynamic)rightNode);
            _builder.EndExpression();
        }

        void ResolveNullValue(MemberNode memberNode, ExpressionType op)
        {
            switch (op)
            {
                case ExpressionType.Equal:
                    _builder.QueryByFieldNull(memberNode.TableName, memberNode.FieldName, memberNode.ColumnAlias);
                    break;
                case ExpressionType.NotEqual:
                    _builder.QueryByFieldNotNull(memberNode.TableName, memberNode.FieldName, memberNode.ColumnAlias);
                    break;
            }
        }

        void ResolveSingleOperation(ExpressionType op)
        {
            switch (op)
            {
                case ExpressionType.Not:
                    _builder.Not();
                    break;
            }
        }

        void ResolveOperation(ExpressionType op)
        {
            switch (op)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    _builder.And();
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    _builder.Or();
                    break;
                default:
                    throw new ArgumentException(string.Format("Unrecognized binary expression operation '{0}'", op.ToString()));
            }
        }
    }
}
