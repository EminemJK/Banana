using Banana.Uow.Interface;
using Banana.Uow.Models;
using Banana.Uow.Lambda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dapper;
using System.Text;
using System.Threading.Tasks;

namespace Banana.Uow
{
    public partial class Repository<T> : IRepository<T> where T : class, IEntity
    {
        public List<T> QueryList(Expression<Func<T, bool>> expression)
        {
            var sqlLam = new SqlLambda<T>(TableName, ConnectionBuilder.GetAdapter(this.DBConnection), expression);
            return DBConnection.Query<T>(sqlLam.SQL, sqlLam.QueryParameters, transaction: _dbTransaction).ToList();
        }

        public List<T> WhereIsIn(Expression<Func<T, object>> expression, IEnumerable<object> values)
        {
            var sqlLam = new SqlLambda<T>(TableName, ConnectionBuilder.GetAdapter(this.DBConnection));
            sqlLam.WhereIsIn(expression, values);
            return DBConnection.Query<T>(sqlLam.SQL, sqlLam.QueryParameters, transaction: _dbTransaction).ToList();
        }

        public SqlLambda<T> Create()
        {
            return new SqlLambda<T>(TableName, ConnectionBuilder.GetAdapter(this.DBConnection));
        }
    }
}
