using System;
using System.Collections.Generic;
using System.Text;
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
        public IDbConnection DBConnection { get; private set; }

        public Repository()
        {
            DBConnection = ConnectionBuilder.OpenConnection();
        }

        public Repository(IDbConnection dbConnection)
        {
            this.DBConnection = DBConnection;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="dbTransaction">事务</param>
        public bool Delete(T entity, IDbTransaction dbTransaction = null)
        {
            return DBConnection.Delete(entity, dbTransaction);
        }

        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="dbTransaction">事务</param>
        /// <returns></returns>
        public long Insert(T entity, IDbTransaction dbTransaction = null)
        {
            return DBConnection.Insert(entity, dbTransaction);
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
        public virtual int Execute(string sql, dynamic parms, IDbTransaction dbTransaction = null)
        {
            return DBConnection.Execute(sql, (object)parms, transaction: dbTransaction);
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        public virtual bool InsertBatch(string sql, IEnumerable<T> entities)
        {
            using (IDbTransaction trans = OpenTransaction)
            {
                try
                {
                    int res = Execute(sql, entities, trans);
                    trans.Commit();
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
                catch
                {
                    trans.Rollback();
                    return false;
                }
            }
        }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName
        {
            get
            {
                TableAttribute attr = (TableAttribute)typeof(T).GetCustomAttribute(typeof(TableAttribute));
                return attr.Name;
            }
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        public IDbTransaction OpenTransaction { get => DBConnection.BeginTransaction(); }
    }
}
