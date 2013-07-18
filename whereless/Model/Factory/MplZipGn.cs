using System;
using System.Collections.Generic;
using whereless.Model.Entities;
using whereless.NativeWiFi;

namespace whereless.Model.Factory
{
    public sealed class MplZipGn : EntitiesFactory
    {
        internal override Type LocationType
        {
            get { return typeof(MultiPlacesLocation); }
        }

        internal override Type PlaceType
        {
            get { return typeof (ZIndexPlace); }
        }

        internal override Type NetworkType
        {
            get { return typeof(GaussianNetwork); }
        }

        public override Location CreateLocation(String name)
        {
            return new MultiPlacesLocation(name);
        }

        public override Location CreateLocation(String name, IList<IMeasure> measures)
        {
            return new MultiPlacesLocation(name, measures);
        }

        public override Place CreatePlace()
        {
            return new ZIndexPlace();
        }

        public override Place CreatePlace(IList<IMeasure> measures)
        {
            return new ZIndexPlace(measures);
        }

        public override Network CreateNetwork(IMeasure measure)
        {
            return new GaussianNetwork(measure);
        }

    }
}
