using System;
using System.Collections.Generic;
using NHibernate;
using log4net;
using whereless.Test.Model;

namespace whereless.Model.Repository
{

    public class NHUnitOfWork : IUnitOfWork
    {

        private static readonly ISessionFactory SessionFactory = NHModel.SessionFactory;
        private static readonly ILog Log = LogManager.GetLogger(typeof(NHUnitOfWork));

        private readonly ISession _session;
        private readonly ITransaction _transaction;

        private bool _disposed = false;

        public NHUnitOfWork()
        {
            _session = SessionFactory.OpenSession();
            _transaction = _session.BeginTransaction();

            //The following lines are needed in order to force DLL's to copy out during build. (optimization excludes them otherwise)
            //var pf = new NHibernate.ByteCode.Castle.ProxyFactory();
            //var gn = new Castle.Core.GraphNode();
            //var dpb = new Castle.DynamicProxy.DefaultProxyBuilder();
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
                        _transaction.Dispose();
                        _session.Close();
                    }
                    _session.Dispose();
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                _disposed = true;
            }
        }

        public void Commit()
        {
            if (!_transaction.IsActive)
            {
                throw new InvalidOperationException("Oops! We don't have an active transaction");
            }
            _transaction.Commit();
        }

        public void Rollback()
        {
            if (!_transaction.IsActive)
            {
                throw new InvalidOperationException("Oops! We don't have an active transaction");
            }
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

    }
}

