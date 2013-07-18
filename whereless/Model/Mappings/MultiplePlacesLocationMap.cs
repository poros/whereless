using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using whereless.Entities;

namespace whereless.Mappings
{
    public class MultiplePlacesLocationMap : ClassMap<MultiPlacesLocation>
    {
        public MultiplePlacesLocationMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.N);
            Map(x => x.Time);
            HasMany(x => x.Places)
                //.Inverse()
                .Cascade.All();
        }
    }
}
