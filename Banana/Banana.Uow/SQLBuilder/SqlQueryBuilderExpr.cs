using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Banana.Uow.SQLBuilder
{
    /// <summary>
    /// Implements the expression buiding for the WHERE statement
    /// </summary>
    partial class SqlQueryBuilder : ISqlBuilder
    {
        public void BeginExpression()
        {
            WhereConditions.Add("(");
        }

        public void EndExpression()
        {
            WhereConditions.Add(")");
        }

        public void And()
        {
            if (WhereConditions.Count > 0)
                WhereConditions.Add(" AND ");
        }

        public void Or()
        {
            if (WhereConditions.Count > 0)
                WhereConditions.Add(" OR ");
        }

        public void Not()
        {
            WhereConditions.Add(" NOT ");
        }

        public void QueryByField(string tableName, string fieldName, string columnAlias, string op, object fieldValue)
        {
            var paramId = NextParamId();
            string newCondition = string.Format("{0} {1} {2}",
                Adapter.AppendColumnName(fieldName, columnAlias, tableName),
                op,
                Adapter.AppendParametr(paramId));

            WhereConditions.Add(newCondition);
            AddParameter(paramId, fieldValue);
        }

        public void QueryByFieldLike(string tableName, string fieldName, string columnAlias, string fieldValue)
        {
            var paramId = NextParamId();
            string newCondition = string.Format("{0} LIKE {1}",
                Adapter.AppendColumnName(fieldName, columnAlias, tableName),
                Adapter.AppendParametr(paramId));

            WhereConditions.Add(newCondition);
            AddParameter(paramId, fieldValue);
        }

        public void QueryByFieldNull(string tableName, string fieldName, string columnAlias)
        {
            WhereConditions.Add(string.Format("{0} IS NULL", Adapter.AppendColumnName(fieldName, columnAlias, tableName)));
        }

        public void QueryByFieldNotNull(string tableName, string fieldName, string columnAlias)
        {
            WhereConditions.Add(string.Format("{0} IS NOT NULL", Adapter.AppendColumnName(fieldName, columnAlias, tableName)));
        }

        public void QueryByFieldComparison(string leftTableName, string leftFieldName, string columnAlias, string op,
            string rightTableName, string rightFieldName)
        {
            string newCondition = string.Format("{0} {1} {2}",
            Adapter.AppendColumnName(leftFieldName, columnAlias, leftTableName),
            op,
            Adapter.AppendColumnName(rightFieldName, columnAlias, rightTableName));

            WhereConditions.Add(newCondition);
        }

        public void QueryByIsIn(string tableName, string fieldName, string columnAlias, ISqlBuilder sqlQuery)
        {
            var innerQuery = sqlQuery.SQL;
            foreach (var param in sqlQuery.Parameters)
            {
                var innerParamKey = "Inner" + param.Key;
                innerQuery = Regex.Replace(innerQuery, param.Key, innerParamKey);
                AddParameter(innerParamKey, param.Value);
            }

            var newCondition = string.Format("{0} IN ({1})", Adapter.AppendColumnName(fieldName, columnAlias, tableName), innerQuery);

            WhereConditions.Add(newCondition);
        }

        public void QueryByIsIn(string tableName, string fieldName, string columnAlias, IEnumerable<object> values)
        {
            var paramIds = values.Select(x =>
            {
                var paramId = NextParamId();
                AddParameter(paramId, x);
                return Adapter.AppendParametr(paramId);
            });

            var newCondition = string.Format("{0} IN ({1})", Adapter.AppendColumnName(fieldName, columnAlias, tableName), string.Join(",", paramIds));
            WhereConditions.Add(newCondition);
        }
    }
}
