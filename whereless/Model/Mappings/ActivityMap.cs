using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using whereless.Model.Entities;

namespace whereless.Model.Mappings
{
    class ActivityMap : ClassMap<Activity>
    {
        public ActivityMap()
        {
            Id(x => x.Id);
            References(x => x.Location)
                .Column("MultiplePlacesLocation_id");
            Map(x => x.Name);
            Map(x => x.Argument);
            Map(x => x.Pathfile);  
            Map(x => x.Type).CustomType<int>();

        }
    }
}
