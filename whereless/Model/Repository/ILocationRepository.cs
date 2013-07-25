using whereless.Model.Entities;

namespace whereless.Model.Repository
{
    public interface ILocationRepository : IRepository<Location>
    {
        Location GetLocationByName(string name, bool dirty = false);
    }
}
