using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using whereless.Entities;

namespace whereless.Mappings
{
    internal class NetworkMap : ClassMap<Network>
    {
        public NetworkMap()
        {
            Id(x => x.Id);
            Map(x => x.Ssid)
                .Not.Nullable();
            DiscriminateSubClassesOnColumn("type");
        }
    }
}