using FluentNHibernate.Mapping;
using whereless.Model.Entities;

namespace whereless.Model.Mappings
{
    internal class NetworkMap : ClassMap<Network>
    {
        public NetworkMap()
        {
            Id(x => x.Id);
            Map(x => x.Ssid)
                .Not.Nullable();
            References(x => x.Place)
                .Column("ZIndexPlace_id");
            DiscriminateSubClassesOnColumn("type");
        }
    }
}