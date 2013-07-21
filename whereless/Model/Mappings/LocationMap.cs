using FluentNHibernate.Mapping;
using whereless.Model.Entities;

namespace whereless.Model.Mappings
{
    class LocationMap : ClassMap<Location>
    {
        public LocationMap()
        {
            Id(x => x.Id);
            Map(x => x.Name)
                .Not.Nullable()
                .Unique();
            Map(x => x.Time);
            DiscriminateSubClassesOnColumn("type");
        }
    }
}
