using FluentNHibernate.Mapping;
using whereless.Model.Entities;

namespace whereless.Model.Mappings
{
    class PlaceMap : ClassMap<Place>
    {
        public PlaceMap()
        {
            Id(x => x.Id);
            References(x => x.Location)
                .Column("MultiplePlacesLocation_id");
            DiscriminateSubClassesOnColumn("type");
        }
    }
}
