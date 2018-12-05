/***********************************
 * Coder：EminemJK
 * Date：2018-11-20
 **********************************/

using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Banana.Uow.Extension
{
    /// <summary>
    /// 数据库语句
    /// </summary>
    public class SqlBuilder
    {
        /// <summary>
        /// 数据库语句
        /// </summary>
        public SqlBuilder()
        {
        }

        /// <summary>
        /// 数据库语句
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
            return Append(new SqlBuilder("ORDER BY " + GetArgsString("ORDER BY", args)));
        }

        public SqlBuilder Select(params object[] args)
        {
            return Append(new SqlBuilder("SELECT " + GetArgsString("SELECT", args)));
        }

        public SqlBuilder Select(Type type)
        {
            var column = TypePropertiesCache(type).Select(p => p.Name).ToList();
            object[] obj = new object[column.Count];
            for (int idx = 0; idx < column.Count; idx++)
            {
                obj[idx] = column[idx];
            } 
            return Select(obj);
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

        public static string GetArgsString(string fix, params object[] args)
        {
            return string.Join(", ", (from x in args select RevomeFlag(x.ToString(), fix)).ToArray());
        }
        
        private static readonly Regex rxParams = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled); 
        private static string ProcessParams(string _sql, object[] args_src, Dictionary<string, object> temp)
        {
            return rxParams.Replace(_sql, m =>
            {
                string param = m.Value.Substring(1);

                bool found = false;
                if (int.TryParse(param, out int paramIndex))
                { 
                    if (paramIndex < 0 || paramIndex >= args_src.Length)
                        throw new ArgumentOutOfRangeException(string.Format("参数 '@{0}' 已指定，但只提供了参数 {1} (sql: `{2}`)", paramIndex, args_src.Length, _sql)); 
                    var o = args_src[paramIndex]; 
                    var pi = o.GetType().GetProperty(param);
                    if (pi != null)
                    {
                        if (temp.ContainsKey(pi.Name))
                        {
                            throw new ArgumentOutOfRangeException("参数重名：" + pi.Name);
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
                                throw new ArgumentOutOfRangeException("参数重名：" + pi.Name);
                            }
                            temp.Add(pi.Name, pi.GetValue(o, null));
                            found = true;
                            break;
                        }
                        else if (o is System.Dynamic.ExpandoObject)
                        {
                            IDictionary<string, object> dic = o as System.Dynamic.ExpandoObject;
                            foreach (var key in dic.Keys)
                            {
                                if (temp.ContainsKey(key))
                                {
                                    throw new ArgumentOutOfRangeException("参数重名：" + key);
                                }
                                found = true;
                                temp.Add(key, dic[key]);
                            } 
                            break;
                        }
                    }  
                }
                if (!found)
                    throw new ArgumentException(string.Format("参数 '@{0}' 已指定， 但传递的参数中没有一个具有该名称的属性 (sql: '{1}')", param, _sql));
                //return "@" + (args_dest.Count - 1).ToString();
                return "@" + param;
            }
            );
        }

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

        private static List<PropertyInfo> TypePropertiesCache(Type type)
        {
            if (TypeProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> pis))
            {
                return pis.ToList();
            }

            var properties = type.GetProperties().Where(IsWriteable).ToArray();
            TypeProperties[type.TypeHandle] = properties;
            return properties.ToList();
        }

        public static bool IsWriteable(PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(typeof(WriteAttribute), false).AsList();
            if (attributes.Count != 1)
                return true;

            var writeAttribute = (WriteAttribute)attributes[0];
            return writeAttribute.Write;
        }

        public static string RevomeFlag(string OldString, string flag)
        {
            var temp = OldString.ToUpper();
            flag = flag.ToUpper();
            if (temp.StartsWith(flag))
            {
                return OldString.Substring(flag.Length);
            }
            return OldString;
        }
    }
}
