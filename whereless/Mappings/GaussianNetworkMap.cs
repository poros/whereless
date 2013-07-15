using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using whereless.Entities;

namespace whereless.Mappings
{
    class GaussianNetworkMap : SubclassMap<GaussianNetwork>
    {
        public GaussianNetworkMap()
        {
            DiscriminatorValue("GaussianNetwork"); //typeOf(GaussianNetwork)

            Map(x => x.N);
            Map(x => x.Mean);
            Map(x => x.StdDev);
            //References(x => x.PlaceReference);
        }
    }
}
