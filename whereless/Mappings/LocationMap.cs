using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using whereless.Entities;

namespace whereless.Mappings
{
    class LocationMap : ClassMap<Location>
    {
        public LocationMap()
        {
            Id(x => x.Id);
            Map(x => x.Name)
                .Not.Nullable()
                .Unique();
            Map(x => x.Time);
            DiscriminateSubClassesOnColumn("type");
        }
    }
}
