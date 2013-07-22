using System.Collections.Generic;

namespace whereless.Model.Repository
{
    // ASK FOR MORE OPERATIONS IMPLEMENTATION IF NEEDED (EVEN ENTITY COMPARISON)
    public interface IRepository<T>
    {
        T Get(object id, bool dirty = false);
        void Save(T value);
        void Update(T value);
        void Delete(T value);
        IList<T> GetAll(bool dirty = false);
    }
}