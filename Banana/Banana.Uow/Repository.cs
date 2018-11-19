using System;
using System.Collections.Generic;
using Dapper;
using System.Data;
using Dapper.Contrib.Extensions;
using System.Linq;
using System.Reflection;
using Banana.Uow.Models;
using Banana.Uow.Interface;

namespace Banana.Uow
{
    /// <summary>
    /// Coder：EminemJK 
    /// Date：2018-11-16
    /// 仓储基类
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        private IDbConnection _dbConnection;
        public IDbConnection DBConnection
        {
            get
            {
                if (_dbConnection == null)
                {
                    _dbConnection = ConnectionBuilder.OpenConnection();
                }
                if (_dbConnection.State == ConnectionState.Closed)
                {
                    _dbConnection.Open();
                }
                return _dbConnection;
            }
            private set { this._dbConnection = value; }
        }

        public Repository()
        {
            _dbConnection = ConnectionBuilder.OpenConnection();
        }

        public Repository(IDbConnection dbConnection, IDbTransaction dbTransaction = null)
        {
            this._dbConnection = dbConnection;
            this._dbTransaction = dbTransaction;
        }

        private string _tableName;
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName
        {
            get
            {
                if (string.IsNullOrEmpty(_tableName))
                {
                    _tableName = ((TableAttribute)typeof(T).GetCustomAttribute(typeof(TableAttribute))).Name;
                }
                return _tableName;
            }
        }

        private IDbTransaction _dbTransaction;
        /// <summary>
        /// 开启事务
        /// </summary>
        public IDbTransaction OpenTransaction()
        {
            _dbTransaction = DBConnection.BeginTransaction();
            return _dbTransaction;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="dbTransaction">事务</param>
        public bool Delete(T entity)
        {
            return DBConnection.Delete(entity, _dbTransaction);
        }

        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="dbTransaction">事务</param>
        /// <returns></returns>
        public long Insert(T entity)
        {
            return DBConnection.Insert(entity, _dbTransaction);
        }

        /// <summary>
        /// 查询单个实体
        /// </summary>
        /// <param name="id">Id</param>
        public T Query(int id)
        {
            return DBConnection.Get<T>(id);
        }

        /// <summary>
        /// 查询全部
        /// </summary>
        public List<T> QueryAll()
        {
            return DBConnection.GetAll<T>().ToList();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageNum">页码</param>
        /// <param name="pagesize">页大小</param>
        /// <param name="order">按照</param>
        /// <param name="asc"></param>
        /// <returns></returns>
        public List<T> QueryAll(int pageNum, int pagesize, string order = null, bool asc = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dbTransaction"></param>
        /// <returns></returns>
        public bool Update(T entity, IDbTransaction dbTransaction = null)
        {
            return DBConnection.Update<T>(entity, dbTransaction);
        }

        /// <summary>
        /// 执行单条语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parms"></param>
        /// <param name="dbTransaction">事务</param>
        /// <returns></returns>
        public int Execute(string sql, dynamic parms)
        {
            return DBConnection.Execute(sql, (object)parms, transaction: _dbTransaction);
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        public virtual bool InsertBatch(string sql, IEnumerable<T> entities)
        {
            using (IDbTransaction trans = OpenTransaction())
            {
                try
                {
                    int res = Execute(sql, entities); 
                    if (res > 0)
                    {
                        trans.Commit();
                        return true;
                    }
                    else
                    {
                        trans.Rollback();
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    trans.Rollback();
                    return false;
                }
            }
        }

        public bool Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
