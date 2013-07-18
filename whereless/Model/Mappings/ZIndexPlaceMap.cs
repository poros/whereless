using FluentNHibernate.Mapping;
using whereless.Model.Entities;

namespace whereless.Model.Mappings
{
    class ZIndexPlaceMap : SubclassMap<ZIndexPlace>
    {
        public ZIndexPlaceMap()
        {
            DiscriminatorValue("ZIndexPlace");

            HasMany(x => x.Networks)
                // .Inverse()
                .Cascade.All();
        }
    }
}
