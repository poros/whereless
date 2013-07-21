using System;
using System.Collections.Generic;
using whereless.Model.Entities;
using whereless.NativeWiFi;

namespace whereless.Model.Factory
{
    // I have no time for studying a dependency injection framework and it should not be the case
    // Consider abstract factory pattern over factory/template pattern if things get messy
    // Things got messy :P
    public interface IEntitiesFactory
    {
        Type LocationType { get; }
        Type PlaceType { get; }
        Type NetworkType { get; }
        Location CreateLocation(string name);
        Location CreateLocation(string name, IList<IMeasure> measures);
        Place CreatePlace();
        Place CreatePlace(IList<IMeasure> measures);
        Network CreateNetwork(IMeasure measure);
    }
}
