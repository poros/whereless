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
            Map(x => x.TotalTime);
            Map(x => x.LongestStreak);
            Map(x => x.CurrentStreak);
            Map(x => x.ArrivedAt);
            HasMany(x => x.ActivityList)
                //.Inverse()
                .Cascade.All();
            DiscriminateSubClassesOnColumn("type");
        }
    }
}
