using Banana.Uow.Models.QueryEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Uow.SQLBuilder
{
    /// <summary>
    /// 实现Select、Join、Group by、Order by SQL语句构建|Implements the SQL building for JOIN, ORDER BY, SELECT, and GROUP BY
    /// </summary>
    partial class SqlQueryBuilder : ISqlBuilder
    {
        public void Join(string originalTableName, string joinTableName, string leftField, string rightField)
        {
            var joinString = string.Format("JOIN {0} ON {1} = {2}",
                                           Adapter.AppendColumnName(joinTableName, "", ""),
                                           Adapter.AppendColumnName(leftField, "", originalTableName),
                                           Adapter.AppendColumnName(rightField, "", joinTableName));
            TableNames.Add(joinTableName);
            JoinExpressions.Add(joinString);
            SplitColumns.Add(rightField);
        }

        public void OrderBy(string tableName, string fieldName, bool desc = false)
        {
            var order = Adapter.AppendColumnName(fieldName, "", tableName);
            if (desc)
                order += " DESC";

            OrderByList.Add(order);
        }

        public void Query(Type type)
        {
            string tableName = Extension.SqlMapperExtensions.GetTableName(type);
            var sbColumnList = new StringBuilder(null);
            var allProperties = Extension.SqlMapperExtensions.TypePropertiesCache(type);
            for (var i = 0; i < allProperties.Count; i++)
            {
                var property = allProperties[i];
                sbColumnList.Append(Adapter.AppendColumnName(Extension.SqlMapperExtensions.GetColumnName(property), property.Name, tableName));
                if (i < allProperties.Count - 1)
                    sbColumnList.Append(", ");
            }
            SelectionList.Add(sbColumnList.ToString());
        }

        public void Query(string tableName, string fieldName, string columnAlias)
        {
            SelectionList.Add(Adapter.AppendColumnName(fieldName, columnAlias, tableName));
        }

        public void Query(string tableName, string fieldName, string columnAlias, ESelectFunction selectFunction)
        {
            var selectionString = string.Format("{0}({1})", selectFunction.ToString(), Adapter.AppendColumnName(fieldName, columnAlias, tableName));
            SelectionList.Add(selectionString);
        }

        public void GroupBy(string tableName, string fieldName)
        {
            GroupByList.Add(Adapter.AppendColumnName(fieldName, "", tableName));
        }
    }
}
