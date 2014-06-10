using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;

namespace Footprints.DAL.Core
{
    public abstract class RepositoryBase<T> where T : class
    {

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
        public virtual IEnumerable<T> GetAll()
        {
            return null;
        }

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return null;
        }
    }

    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> where);
        T GetById(long id);
        T GetById(string id);
        T Get(Expression<Func<T, bool>> where);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetMany(Expression<Func<T, bool>> where);
    }
}