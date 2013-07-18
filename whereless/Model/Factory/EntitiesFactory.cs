using System;
using System.Collections.Generic;
using whereless.Model.Entities;
using whereless.NativeWiFi;

namespace whereless.Model.Factory
{
    // I have no time for studying a dependency injection framework and it should not be the case
    // Consider abstract factory pattern over factory/template pattern if things get messy
    // Things got messy :P
    public abstract class EntitiesFactory
    {
        private static readonly EntitiesFactory Instance = InstantiateFactory();

        protected EntitiesFactory() {}

        // TODO implement logic in order to instantiate the correct factory
        private static EntitiesFactory InstantiateFactory()
        {
            return new MplZipGn();
        }

        public static EntitiesFactory Factory
        {
            get
            {
                return Instance;
            }
        }

        internal abstract Type LocationType { get; }
        internal abstract Type PlaceType { get; }
        internal abstract Type NetworkType { get; }

        public abstract Location CreateLocation(String name);
        public abstract Location CreateLocation(String name, IList<IMeasure> measures);
        public abstract Place CreatePlace();
        public abstract Place CreatePlace(IList<IMeasure> measures);
        public abstract Network CreateNetwork(IMeasure measure);
    }
}
