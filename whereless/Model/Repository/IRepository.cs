using System.Collections.Generic;

namespace whereless.Model.Repository
{
    /// <summary>
    /// Should you ever need to add functionality specific to a single class, extend
    /// the interface. See IMonkeyRepository.
    /// </summary>
    public interface IRepository<T>
    {
        T Get(object id);
        void Save(T value);
        void Update(T value);
        void Delete(T value);
        IList<T> GetAll();
    }
}