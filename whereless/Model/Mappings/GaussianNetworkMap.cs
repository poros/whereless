using FluentNHibernate.Mapping;
using whereless.Model.Entities;

namespace whereless.Model.Mappings
{
    class GaussianNetworkMap : SubclassMap<GaussianNetwork>
    {
        public GaussianNetworkMap()
        {
            DiscriminatorValue("GaussianNetwork"); //typeOf(GaussianNetwork)
            Map(x => x.N);
            Map(x => x.Mean);
            Map(x => x.S);
        }
    }
}
