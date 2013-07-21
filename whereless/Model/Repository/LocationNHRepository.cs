using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Criterion;
using whereless.Model.Entities;

namespace whereless.Model.Repository
{
    class LocationNHRepository : NHRepository<Location>, ILocationRepository
    {
        public Location GetByName(string name)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    return session.CreateCriteria(typeof(Location)).Add(Restrictions.Eq("Name", name))
                                            .UniqueResult<Location>();
                }
            }
            
        }
    }
}
