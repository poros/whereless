using FluentNHibernate.Mapping;
using whereless.Model.Entities;

namespace whereless.Model.Mappings
{
    class MultiPlacesLocationMap : SubclassMap<MultiPlacesLocation>
    {
        public MultiPlacesLocationMap()
        {
            DiscriminatorValue("MultiplePlacesMap");
            Map(x => x.N);
            HasMany(x => x.Places)
                // .Inverse()
                .Cascade.All();
        }
    }
}
