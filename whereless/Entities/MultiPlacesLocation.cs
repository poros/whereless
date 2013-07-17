using System;
using System.Collections.Generic;
using System.Linq;
using whereless.NativeWiFi;

namespace whereless.Entities
{
    public class MultiPlacesLocation : Location
    {
        public virtual ulong N { get; set; }

        // Locations are saved from most recent to oldest,
        // because in case of locations with similar footprints (test may pass for both)
        // more recent ones MUST be preferred, since the algorithm look for locations in order!!!
        // (users are stupid, they can register the same location twice,
        // let's give them the last, it would be the less disappointing thing to do)
        private Stack<Place> _places;

        public virtual IList<Place> Places
        {
            get { return _places.ToList(); }
            set { _places = new Stack<Place>(value); }
        }

        private Place _currPlace;

        protected virtual Place PlaceFactory(IList<IMeasure> measures)
        {
            return new ZIndexPlace(measures);
        }

        public virtual void AddPlace(Place place)
        {
            _places.Push(place);
            //place.LocationReference = this;
        }


        protected MultiPlacesLocation()
        {
        }

        public MultiPlacesLocation(String name)
        {
            this.Name = name;
            _places = new Stack<Place>();
        }

        public MultiPlacesLocation(string name, IList<IMeasure> measures)
        {
            this.Name = name;
            _places = new Stack<Place>();
            AddPlace(PlaceFactory(measures));
        }

        public override ulong Time
        {
            get { return N*Location.T; }
            set { N = value/Location.T; }
        }

        public override bool TestInput(IList<IMeasure> measures)
        {
            if (_currPlace != null && _currPlace.TestInput(measures))
            {
                return true;
            }
            var oldPlace = _currPlace;
            _currPlace = null;
            foreach (
                var place in _places.Where(place => place != oldPlace).Where(place => place.TestInput(measures)))
            {
                _currPlace = place;
                return true;
            }
            return false;
        }

        public override void UpdateStats(IList<IMeasure> measures)
        {
            //just to be sure that a current place is set
            if (_currPlace == null && !TestInput(measures))
            {
                _currPlace = _places.Peek();
            }
            _currPlace.UpdateStats(measures);
            N += 1;
        }
    }
}