using System.Collections.Generic;
using System.Diagnostics;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace whereless.Model.Repository
{
    public class NHRepository<T> : IRepository<T> where T : class
    {
        protected readonly ISessionFactory SessionFactory;

        public NHRepository(ISessionFactory sessionFactory)
        {
            Debug.Assert(sessionFactory != null, "sessionFactory != null");
            this.SessionFactory = sessionFactory;
        }

        public T Get(object id, bool dirty = false)
        {
            using (var session = SessionFactory.OpenSession())
            {
                if (!dirty)
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        T returnVal = session.Get<T>(id);
                        transaction.Commit();
                        return returnVal;
                    }
                }
                else
                {
                    return session.Get<T>(id);
                }
            }
        }

        public void Save(T value)
        {
            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(value);
                    transaction.Commit();
                }
            }
        }

        public void Update(T value)
        {
            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Update(value);
                    transaction.Commit();
                }
            }
        }

        public void Delete(T value)
        {
            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Delete(value);
                    transaction.Commit();
                }
            }
        }

        public IList<T> GetAll(bool dirty = false)
        {
            using (var session = SessionFactory.OpenSession())
            {
                if (!dirty)
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        IList<T> returnVal = session.CreateCriteria<T>().List<T>();
                        transaction.Commit();
                        return returnVal;
                    }
                }
                else
                {
                    return session.CreateCriteria<T>().List<T>();
                }
            }
        }
    }
}
