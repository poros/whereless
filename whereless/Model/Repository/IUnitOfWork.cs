using System;
using System.Collections.Generic;
using whereless.Model.Entities;

namespace whereless.Model.Repository
{
    public interface IUnitOfWork : IDisposable
    {

        void Commit();
        void Rollback();

        /// <summary>
        /// Saves or updates the object to the database, depending on the value of its identifier property.
        /// </summary>
        /// <param name="value">A transient instance containing a new or updated state.</param>
        void Save(object value);

        /// <summary>
        /// Removes a persistent instance from the database.
        /// </summary>
        /// <param name="value">The instance to be removed.</param>
        void Delete(object value);

        /// <summary>
        /// Returns a strong typed persistent instance of the given named entity with the given identifier, or null if there is no such persistent instance.
        /// </summary>
        /// <typeparam name="T">The type of the given persistant instance.</typeparam>
        /// <param name="id">An identifier.</param>
        T Get<T>(object id);

        /// <summary>
        /// Returns a list of all instances of type T from the database.
        /// </summary>
        /// <typeparam name="T">The type of the given persistant instance.</typeparam>
        IList<T> GetAll<T>() where T : class;

        Location GetLocationByName(string name);
    }
}