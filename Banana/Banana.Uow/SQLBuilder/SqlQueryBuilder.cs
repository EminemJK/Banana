using Banana.Uow.Interface;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Uow.SQLBuilder
{
    /// <summary>
    /// Implements the whole SQL building logic. Continually adds and stores the SQL parts as the requests come. 
    /// When requested to return the QueryString, the parts are combined and returned as a single query string.
    /// The query parameters are stored in a dictionary implemented by an ExpandoObject that can be requested by QueryParameters.
    /// </summary>
    internal partial class SqlQueryBuilder : ISqlBuilder
    {
        internal ISqlAdapter Adapter { get; set; }

        private const string PARAMETER_PREFIX = "BananaParam";

        public Type type { get; set; }
        public List<string> TableNames { get; } = new List<string>();
        public List<string> JoinExpressions { get; } = new List<string>();
        public List<string> SelectionList { get; } = new List<string>();
        public List<string> WhereConditions { get; } = new List<string>();
        public List<string> OrderByList { get; } = new List<string>();
        public List<string> GroupByList { get; } = new List<string>();
        public List<string> HavingConditions { get; } = new List<string>();
        public List<string> SplitColumns { get; } = new List<string>();
        public int CurrentParamIndex { get; private set; }

        private string Source
        {
            get
            {
                var joinExpression = string.Join(" ", JoinExpressions);
                return string.Format("{0} {1}", Adapter.AppendColumnName(TableNames.First(), "", ""), joinExpression);
            }
        }

        private string Selection
        {
            get
            {
                if (SelectionList.Count == 0)
                {
                    Query(type);
                }
                return string.Join(", ", SelectionList);
            }
        }

        private string Conditions
        {
            get
            {
                if (WhereConditions.Count == 0)
                    return "";
                else
                    return "WHERE " + string.Join("", WhereConditions);
            }
        }

        private string Order
        {
            get
            {
                if (OrderByList.Count == 0)
                    return "";
                else
                    return "ORDER BY " + string.Join(", ", OrderByList);
            }
        }

        private string Grouping
        {
            get
            {
                if (GroupByList.Count == 0)
                    return "";
                else
                    return "GROUP BY " + string.Join(", ", GroupByList);
            }
        }

        private string Having
        {
            get
            {
                if (HavingConditions.Count == 0)
                    return "";
                else
                    return "HAVING " + string.Join(" ", HavingConditions);
            }
        }

        public IDictionary<string, object> Parameters { get; private set; }

        public string SQL => Adapter.QueryString(Selection, Source, Conditions, Grouping, Having, Order);

        public string QueryStringPage(int pageSize, int? pageNumber = null)
        {
            if (pageNumber.HasValue)
            {
                if (OrderByList.Count == 0)
                    throw new Exception("Pagination requires the ORDER BY statement to be specified");

                return Adapter.GetPageList(Source, Selection, Conditions, Order, pageSize, pageNumber.Value);
            }
            return Adapter.GetPageList(Source, Selection, Conditions, Order, pageSize);
        }

        internal SqlQueryBuilder(string tableName,Type type, ISqlAdapter adapter)
        {
            TableNames.Add(tableName);
            Adapter = adapter;
            Parameters = new ExpandoObject();
            CurrentParamIndex = 0;
            this.type = type;
        }

        #region helpers
        private string NextParamId()
        {
            ++CurrentParamIndex;
            return PARAMETER_PREFIX + CurrentParamIndex.ToString(CultureInfo.InvariantCulture);
        }

        private void AddParameter(string key, object value)
        {
            if (!Parameters.ContainsKey(key))
                Parameters.Add(key, value);
        }
        #endregion
    }
}
