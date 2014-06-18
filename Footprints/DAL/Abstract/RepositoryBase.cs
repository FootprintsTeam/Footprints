using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using Neo4jClient;
using CypherNet.Configuration;

namespace Footprints.DAL.Abstract
{
    public abstract class RepositoryBase<T> where T : class
    {

        public RepositoryBase(IGraphClient client) {
            Db = client;
        }

        protected IGraphClient Db
        {
            get;
            private set;
        }

        public virtual void Add(T entity)
        {
            
            return;
        }
        public virtual void Update(T entity)
        {
            return;
        }
        public virtual void Delete(T entity)
        {
            return;
        }
        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            return;
        }
        public virtual T GetById(long id)
        {
            return null;
        }
        public virtual T GetById(string id)
        {
            return null;
        }

        public virtual T Get(Expression<Func<T, bool>> where){
            return null;
        }
        public virtual IEnumerable<T> GetAll()
        {
            return null;
        }

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return null;
        }
    }
}