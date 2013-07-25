using NHibernate;
using NHibernate.Criterion;
using whereless.Model.Entities;

namespace whereless.Model.Repository
{
    class LocationNHRepository : NHRepository<Location>, ILocationRepository
    {
        public LocationNHRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public Location GetLocationByName(string name, bool dirty = false)
        {
            using (var session = SessionFactory.OpenSession())
            {
                if (!dirty)
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        return session.CreateCriteria(typeof (Location)).Add(Restrictions.Eq("Name", name))
                                      .UniqueResult<Location>();
                    }
                }
                else
                {
                    return session.CreateCriteria(typeof(Location)).Add(Restrictions.Eq("Name", name))
                                      .UniqueResult<Location>();
                }
            }
            
        }
    }
}
