using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using whereless.Entities;

namespace whereless.Mappings
{
    class MultiPlacesLocationMap : SubclassMap<MultiPlacesLocation>
    {
        public MultiPlacesLocationMap()
        {
            DiscriminatorValue("MultiplePlacesMap");
            Map(x => x.N);
            HasMany(x => x.Places)
                .Cascade.All();
        }
    }
}
