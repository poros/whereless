using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using whereless.Entities;

namespace whereless.Mappings
{
    class PlaceMap : ClassMap<Place>
    {
        public PlaceMap()
        {
            Id(x => x.Id);
            DiscriminateSubClassesOnColumn("type");
            
        }
    }
}
