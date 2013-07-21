using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using log4net;
using whereless.Model.Entities;

namespace whereless.Model.Repository
{

    public class NHUnitOfWork : IUnitOfWork, ILocationOperations
    {

        private static readonly ILog Log = LogManager.GetLogger(typeof(NHUnitOfWork));
        private readonly ISessionFactory _sessionFactory;

        private readonly ISession _session;
        private readonly ITransaction _transaction;

        private bool _disposed = false;

        public NHUnitOfWork(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
            _session = _sessionFactory.OpenSession();
            _transaction = _session.BeginTransaction();
        }

        // Use C# destructor syntax for finalization code.
        ~NHUnitOfWork()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }

        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                    if (_session.IsOpen)
                    {
                        if (_transaction.IsActive)
                        {
                            _transaction.Rollback();
                        }
                        ((IDisposable)_transaction).Dispose();
                        _session.Close();
                    }
                    ((IDisposable)_session).Dispose();
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                _disposed = true;
            }
        }

        public void Commit()
        {
            //if (!_transaction.IsActive)
            //{
            //    throw new InvalidOperationException("Transaction not active or disposed");
            //}
            _transaction.Commit();
        }

        public void Rollback()
        {
            //if (!_transaction.IsActive)
            //{
            //    throw new InvalidOperationException("Transaction not active or disposed");
            //}
            _transaction.Rollback();
            Log.Info("Rollbacked transaction");
        }

        public void Save(object value)
        {
            _session.SaveOrUpdate(value);
        }


        public void Delete(object value)
        {
            _session.Delete(value);
        }


        public T Get<T>(object id)
        {
            T returnVal = _session.Get<T>(id);
            return returnVal;
        }


        public IList<T> GetAll<T>() where T : class
        {
            IList<T> returnVal = _session.CreateCriteria<T>().List<T>();
            return returnVal;
        }

        public Location GetLocationByName(string name)
        {
            return _session.CreateCriteria(typeof(Location)).Add(Restrictions.Eq("Name", name))
                                            .UniqueResult<Location>();
        }
        

    }
}

