/***********************************
 * Developer: Lio.Huang
 * Date：2018-12-17
 * 
 * Last Update：2018-12-18
 **********************************/

using Banana.Uow.Models.QueryEnum;
using System;
using System.Collections.Generic;

namespace Banana.Uow.SQLBuilder
{
    /// <summary>
    /// The interface for all SqlBuilder operations.
    /// </summary>
    public interface ISqlBuilder
    {
        /// <summary>
        /// SQL
        /// </summary>
        string SQL { get; }

        /// <summary>
        /// Query Parameters
        /// </summary>
        IDictionary<string, object> Parameters { get; }

        Type type { get; }
        List<string> TableNames { get; }
        List<string> JoinExpressions { get; }
        List<string> SelectionList { get; }
        List<string> WhereConditions { get; }
        List<string> OrderByList { get; }
        List<string> GroupByList { get; }
        List<string> HavingConditions { get; }
        List<string> SplitColumns { get; }
        int CurrentParamIndex { get; }
        
        void BeginExpression();
        void EndExpression();
        void And();
        void Or();
        void Not();

        string QueryStringPage(int pageSize, int? pageNumber = null);
        void QueryByField(string tableName, string fieldName, string columnAlias, string op, object fieldValue);
        void QueryByFieldLike(string tableName, string fieldName, string columnAlias, string fieldValue);
        void QueryByFieldNull(string tableName, string fieldName, string columnAlias);
        void QueryByFieldNotNull(string tableName, string fieldName, string columnAlias);
        void QueryByFieldComparison(string leftTableName, string leftFieldName, string columnAlias, string op,
            string rightTableName, string rightFieldName);
        void QueryByIsIn(string tableName, string fieldName, string columnAlias, ISqlBuilder sqlQuery);
        void QueryByIsIn(string tableName, string fieldName, string columnAlias, IEnumerable<object> values);

        void Join(string originalTableName, string joinTableName, string leftField, string rightField);
        void OrderBy(string tableName, string fieldName, bool desc = false);
        void Query(Type type);
        void Query(string tableName, string fieldName, string columnAlias);
        void Query(string tableName, string fieldName, string columnAlias, ESelectFunction selectFunction);
        void GroupBy(string tableName, string fieldName);
    }
}
