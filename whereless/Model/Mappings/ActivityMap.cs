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
            Map(x => x.Name);
            Map(x => x.IdPlace);
            Map(x => x.Pathfile);
            Map(x => x.Argument);
            Map(x => (int)x.Type);

        }
    }
}
