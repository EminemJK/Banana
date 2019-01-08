/***********************************
 * Developer: Lio.Huang
 * Date：2018-11-20
 * 
 * Last Update：2018-12-18
 **********************************/

using Banana.Uow.Interface;
using Banana.Uow.Models.QueryEnum;
using Banana.Uow.SQLBuilder;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Banana.Uow.Extension
{
    /// <summary>
    /// SQL builder
    /// </summary>
    internal class SqlBuilder: ISqlBuilder
    {
        /// <summary>
        /// SQL builder
        /// </summary>
        public SqlBuilder()
        {
        }

        /// <summary>
        /// SQL builder
        /// </summary>
        public SqlBuilder(string sql, params object[] args)
        {
            _sql = sql;
            _args = args;
        }

        private readonly string _sql;
        private readonly object[] _args;
        private SqlBuilder _rhs;
        private string _sqlFinal;
        private object _argsFinal;

        private void Build()
        {
            if (_sqlFinal != null)
                return;
            
            var sb = new StringBuilder();

            Dictionary<string, object> argsObj = new Dictionary<string, object>();
            Build(sb, argsObj, null);
            _sqlFinal = sb.ToString();

            //动态创建对象
            dynamic obj = new ExpandoObject(); 
            foreach (KeyValuePair<string, object> item in argsObj)
            {
                ((IDictionary<string, object>)obj).Add(item.Key, item.Value);
            } 
            _argsFinal = obj;
        }

        public string SQL
        {
            get
            {
                Build();
                return _sqlFinal;
            }
        }

        public object Arguments
        {
            get
            {
                Build();
                return _argsFinal;
            }
        }

        public IDictionary<string, object> Parameters => throw new NotImplementedException();

        public List<string> TableNames => throw new NotImplementedException();

        public List<string> JoinExpressions => throw new NotImplementedException();

        public List<string> SelectionList => throw new NotImplementedException();

        public List<string> WhereConditions => throw new NotImplementedException();

        public List<string> OrderByList => throw new NotImplementedException();

        public List<string> GroupByList => throw new NotImplementedException();

        public List<string> HavingConditions => throw new NotImplementedException();

        public List<string> SplitColumns => throw new NotImplementedException();

        public int CurrentParamIndex => throw new NotImplementedException();

        public SqlBuilder Append(SqlBuilder sql)
        {
            if (_rhs != null)
                _rhs.Append(sql);
            else
                _rhs = sql;

            return this;
        }

        public SqlBuilder Append(string sql, params object[] args)
        {
            return Append(new SqlBuilder(sql, args));
        }

        public SqlBuilder IsAse(bool asc)
        {
            if (asc)
            {
                return Append(new SqlBuilder("ASC"));
            }
            else
            {
                return Append(new SqlBuilder("DESC"));
            }
        }

        public SqlBuilder Where(string sql, params object[] args)
        {
            sql = RevomeFlag(sql, "WHERE");
            return Append(new SqlBuilder("WHERE " + sql, args));
        }

        public SqlBuilder OrderBy(params object[] args)
        {
            return Append(new SqlBuilder("ORDER BY " + GetArgsString("ORDER BY", args: args)));
        }

        public SqlBuilder Select(string prefix = "",params object[] args)
        {
            return Append(new SqlBuilder("SELECT " + GetArgsString("SELECT", prefix: prefix, args: args)));
        }

        public SqlBuilder From(params object[] args)
        {
            return Append(new SqlBuilder("FROM " + string.Join(", ", (from x in args select x.ToString()).ToArray())));
        } 

        private static bool Is(SqlBuilder sql, string sqltype)
        {
            return sql?._sql != null && sql._sql.StartsWith(sqltype, StringComparison.InvariantCultureIgnoreCase);
        }

        public void Build(StringBuilder sb, Dictionary<string, object> argsObj, SqlBuilder lhs)
        {
            if (!string.IsNullOrEmpty(_sql))
            {
                if (sb.Length > 0)
                {
                    sb.Append("\n");
                    sb.Append(" ");
                }

                var sql = ProcessParams(_sql, _args, argsObj);

                if (Is(lhs, "WHERE ") && Is(this, "WHERE "))
                    sql = "AND " + sql.Substring(6);
                if (Is(lhs, "ORDER BY ") && Is(this, "ORDER BY "))
                    sql = ", " + sql.Substring(9);

                sb.Append(sql);
            }
            
            _rhs?.Build(sb, argsObj, this);
        }

        public static string GetArgsString(string dbKeywordFix, string prefix = "", params object[] args)
        {
            return string.Join(", ", (from x in args select prefix + RevomeFlag(x.ToString(), dbKeywordFix)).ToArray());
        }
        
        private static readonly Regex rxParams = new Regex(@"(?<!@)@\w+|(?<!:):\w+", RegexOptions.Compiled); 
        private static string ProcessParams(string _sql, object[] args_src, Dictionary<string, object> temp)
        {
            return rxParams.Replace(_sql, m =>
            {
                string param = m.Value.Substring(1);

                bool found = false;
                if (int.TryParse(param, out int paramIndex))
                { 
                    if (paramIndex < 0 || paramIndex >= args_src.Length)
                        throw new ArgumentOutOfRangeException(string.Format("参数 '@{0}' 已指定，但只提供了参数 {1}|The parameter '@{0}' is specified, but only the parameter {1} is provided. (sql: `{2}`)", paramIndex, args_src.Length, _sql)); 
                    var o = args_src[paramIndex]; 
                    var pi = o.GetType().GetProperty(param);
                    if (pi != null)
                    {
                        if (temp.ContainsKey(pi.Name))
                        {
                            throw new ArgumentOutOfRangeException("参数重名|parameter has same name：" + pi.Name);
                        }
                        temp.Add(pi.Name, pi.GetValue(o, null));
                        found = true; 
                    } 
                }
                else
                {  
                    foreach (var o in args_src)
                    { 
                        var pi = o.GetType().GetProperty(param);
                        if (pi != null)
                        {
                            if (temp.ContainsKey(pi.Name))
                            {
                                throw new ArgumentOutOfRangeException("参数重名|parameter has same name：" + pi.Name);
                            }
                            temp.Add(pi.Name, pi.GetValue(o, null));
                            found = true;
                            break;
                        }
                        else if (o is ExpandoObject)
                        {
                            IDictionary<string, object> dic = o as System.Dynamic.ExpandoObject;
                            foreach (var key in dic.Keys)
                            {
                                if (temp.ContainsKey(key))
                                {
                                    throw new ArgumentOutOfRangeException("参数重名|parameter has same name：" + key);
                                }
                                found = true;
                                temp.Add(key, dic[key]);
                            } 
                            break;
                        }
                    }  
                }
                if (!found)
                    throw new ArgumentException(string.Format("参数 '@{0}' 已指定， 但传递的参数中没有一个具有该名称的属性|The parameter '@{0}' is specified, but none of the passed parameters has an attribute with that name. (sql: '{1}')", param, _sql));
                //return "@" + (args_dest.Count - 1).ToString();
                return m.Value;
            }
            );
        }

        public static string RevomeFlag(string OldString, string prefix)
        { 
            if (OldString.TrimStart().StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
            {
                return OldString.Substring(prefix.Length);
            }
            return OldString;
        }

        public void BeginExpression()
        {
            throw new NotImplementedException();
        }

        public void EndExpression()
        {
            throw new NotImplementedException();
        }

        public void And()
        {
            throw new NotImplementedException();
        }

        public void Or()
        {
            throw new NotImplementedException();
        }

        public void Not()
        {
            throw new NotImplementedException();
        }

        public string QueryStringPage(int pageSize, int? pageNumber = null)
        {
            throw new NotImplementedException();
        }

        public void QueryByField(string tableName, string fieldName, string columnAlias, string op, object fieldValue)
        {
            throw new NotImplementedException();
        }

        public void QueryByFieldLike(string tableName, string fieldName, string columnAlias, string fieldValue)
        {
            throw new NotImplementedException();
        }

        public void QueryByFieldNull(string tableName, string fieldName, string columnAlias)
        {
            throw new NotImplementedException();
        }

        public void QueryByFieldNotNull(string tableName, string fieldName, string columnAlias)
        {
            throw new NotImplementedException();
        }

        public void QueryByFieldComparison(string leftTableName, string leftFieldName, string columnAlias, string op, string rightTableName, string rightFieldName)
        {
            throw new NotImplementedException();
        }

        public void QueryByIsIn(string tableName, string fieldName, string columnAlias, ISqlBuilder sqlQuery)
        {
            throw new NotImplementedException();
        }

        public void QueryByIsIn(string tableName, string fieldName, string columnAlias, IEnumerable<object> values)
        {
            throw new NotImplementedException();
        }

        public void Join(string originalTableName, string joinTableName, string leftField, string rightField)
        {
            throw new NotImplementedException();
        }

        public void OrderBy(string tableName, string fieldName, bool desc = false)
        {
            throw new NotImplementedException();
        }

        public void Query(Type type)
        {
            throw new NotImplementedException();
        }

        public void Query(string tableName, string fieldName, string columnAlias)
        {
            throw new NotImplementedException();
        }

        public void Query(string tableName, string fieldName, string columnAlias, ESelectFunction selectFunction)
        {
            throw new NotImplementedException();
        }

        public void GroupBy(string tableName, string fieldName)
        {
            throw new NotImplementedException();
        }
    }
}
