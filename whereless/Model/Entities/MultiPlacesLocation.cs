using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using whereless.Model.Factory;
using whereless.WiFi;

namespace whereless.Model.Entities
{
    public class MultiPlacesLocation : Location
    {
        private ulong _n = 0;
        // Locations are saved from most recent to oldest,
        // because in case of locations with similar footprints (test may pass for both)
        // more recent ones MUST be preferred, since the algorithm look for locations in order!!!
        // (users are stupid, they can register the same location twice,
        // let's give them the last, it would be the less disappointing thing to do)
        private Stack<Place> _places;
        private Place _currPlace;
        //REMARK!!! This is an emergency solution. Impossible to move it in ModelHelper
        // but in general it should not be here.
        // Consider to redisgn the whole entity layer
        private readonly IEntitiesFactory _Factory = NHModel.GetEntitiesFactory();

        // number of observations
        public virtual ulong N
        {
            get { return _n; }
            set { _n = value;  }
        }
        
        //time is given in terms of observations and default time between them
        public override ulong Time
        {
            get { return _n * T; }
            set { _n = value / T; }
        }


        public virtual IList<Place> Places
        {
            get { return _places.ToList(); }
            set { _places = new Stack<Place>(value); }
        }

        //NHibernate specific in case of double-side reference
        public virtual void AddPlace(Place place)
        {
            _places.Push(place);
            //place.LocationReference = this;
        }


        protected MultiPlacesLocation() { }

        public MultiPlacesLocation(String name) : base(name)
        {
            _places = new Stack<Place>();
        }

        public MultiPlacesLocation(string name, IList<IMeasure> measures) : base(name)
        {
            _places = new Stack<Place>();
            AddPlace(_Factory.CreatePlace(measures));
        }

        // REMARK side effect: always set current place if return true
        public override bool TestInput(IList<IMeasure> measures)
        {
            // proximity preference
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
            // REMARK current place was not set up
            return false;
        }

        public override void UpdateStats(IList<IMeasure> measures)
        {
            //just to be sure that a current place is set
            if (_currPlace == null && !TestInput(measures))
            {
                _currPlace = _places.Peek();
            }
            // TODO find a better way than a side effect
            Debug.Assert(_currPlace != null, "_currPlace != null");
            _currPlace.UpdateStats(measures);
            N += 1;
        }

        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();
            foreach (var place in Places)
            {
                buffer.AppendLine(place.ToString());
            }
            return (base.ToString() + " N = " + N + "; Places = {\n" + buffer + "} ");
        }
    }
}