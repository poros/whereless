using System.Collections.Generic;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace whereless.Model.Repository
{
    public class NHRepository<T> : IRepository<T> where T : class
    {
        private static readonly ISessionFactory sessionFactory = NHModel.SessionFactory;

        public T Get(object id)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    T returnVal = session.Get<T>(id);
                    transaction.Commit();
                    return returnVal;
                }
            }
        }

        public void Save(T value)
        {
            using (var session = sessionFactory.OpenSession())
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
            using (var session = sessionFactory.OpenSession())
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
            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Delete(value);
                    transaction.Commit();
                }
            }
        }

        public IList<T> GetAll()
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    IList<T> returnVal = session.CreateCriteria<T>().List<T>();
                    transaction.Commit();
                    return returnVal;
                }
            }
        }
    }
}
