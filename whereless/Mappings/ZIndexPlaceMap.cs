using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using whereless.Entities;

namespace whereless.Mappings
{
    class ZIndexPlaceMap : SubclassMap<ZIndexPlace>
    {
        public ZIndexPlaceMap()
        {
            DiscriminatorValue("ZIndexPlace");

            HasMany(x => x.Networks)
                .Cascade.All();
        }
    }
}
