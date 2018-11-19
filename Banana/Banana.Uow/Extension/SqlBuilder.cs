using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Banana.Uow.Extension
{
    public class SqlBuilder
    {
        public SqlBuilder()
        {
        }

        public SqlBuilder(string sql, params object[] args)
        {
            _sql = sql;
            _args = args;
        }

        private readonly string _sql;
        private readonly object[] _args;
        private SqlBuilder _rhs;
        private string _sqlFinal;
        private object[] _argsFinal;

        private void Build()
        {
            // already built?
            if (_sqlFinal != null)
                return;

            // Build it
            var sb = new StringBuilder();
            var args = new List<object>();
            Build(sb, args, null);
            _sqlFinal = sb.ToString();
            _argsFinal = args.ToArray();
        }

        public string SQL
        {
            get
            {
                Build();
                return _sqlFinal;
            }
        }

        public object[] Arguments
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

        public SqlBuilder Where(string sql, params object[] args)
        {
            return Append(new SqlBuilder("WHERE " + sql, args));
        }

        public SqlBuilder OrderBy(params object[] args)
        {
            return Append(new SqlBuilder("ORDER BY " + string.Join(", ",  args )));
        }

        public SqlBuilder Select(params object[] args)
        {
            return Append(new SqlBuilder("SELECT " + string.Join(", ", (from x in args select x.ToString()).ToArray())));
        }

        public SqlBuilder Select(Type type)
        {
            var column = TypePropertiesCache(type).Select(p=>p.Name);  
            return Select(column);
        }

        public SqlBuilder From(params object[] args)
        {
            return Append(new SqlBuilder("FROM " + string.Join(", ", (from x in args select x.ToString()).ToArray())));
        }

        private static bool Is(SqlBuilder sql, string sqltype)
        {
            return sql?._sql != null && sql._sql.StartsWith(sqltype, StringComparison.InvariantCultureIgnoreCase);
        }

        public void Build(StringBuilder sb, List<object> args, SqlBuilder lhs)
        {
            if (!string.IsNullOrEmpty(_sql))
            {
                // Add SQL to the string
                if (sb.Length > 0)
                {
                    sb.Append("\n");
                }

                var sql = ProcessParams(_sql, _args, args);

                if (Is(lhs, "WHERE ") && Is(this, "WHERE "))
                    sql = "AND " + sql.Substring(6);
                if (Is(lhs, "ORDER BY ") && Is(this, "ORDER BY "))
                    sql = ", " + sql.Substring(9);

                sb.Append(sql);
            }

            // Now do rhs
            _rhs?.Build(sb, args, this);
        } 

        private static readonly Regex rxParams = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);
        private static string ProcessParams(string _sql, object[] args_src, List<object> args_dest)
        {
            return rxParams.Replace(_sql, m =>
            {
                string param = m.Value.Substring(1);

                if (int.TryParse(param, out int paramIndex))
                { 
                    if (paramIndex < 0 || paramIndex >= args_src.Length)
                        throw new ArgumentOutOfRangeException(string.Format("参数 '@{0}' 已指定，但只提供了参数 {1} (sql: `{2}`)", paramIndex, args_src.Length, _sql));
                    args_dest.Add(args_src[paramIndex]);
                }
                else
                { 
                    bool found = false;
                    foreach (var o in args_src)
                    {
                        var pi = o.GetType().GetProperty(param);
                        if (pi != null)
                        {
                            args_dest.Add(pi.GetValue(o, null));
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        throw new ArgumentException(string.Format("参数 '@{0}' 已指定， 但传递的参数中没有一个具有该名称的属性 (sql: '{1}')", param, _sql));
                }

                return "@" + (args_dest.Count - 1).ToString();
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
    }
}
