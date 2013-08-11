using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using log4net;
using whereless.Model.Factory;
using whereless.Model.ValueObjects;

namespace whereless.Model.Entities
{
    public class MultiPlacesLocation : Location
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MultiPlacesLocation));

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
        public override ulong TotalTime
        {
            get { return _n * T; }
            set { _n = value / T; }
        }

        private ulong _currentN = 0;
        public override ulong CurrentStreak
        {
            get { return _currentN * T; }
            set { _currentN = value / T; }
        }

        private ulong _longestN = 0;
        public override ulong LongestStreak
        {
            get { return _longestN * T; }
            set { _longestN = value/T; }
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
            _n = 1;
            _currentN = 1;
            _longestN = 1;
            SetArrivedAt();
        }

        // REMARK side effect: always set current place if return true
        public override bool TestInput(IList<IMeasure> measures)
        {
            Log.Debug("To be tested " + this.Name);

            if (measures.Count == 0)
            {
                return false;
            }

            // proximity preference
            if (_currPlace != null && _currPlace.TestInput(measures))
            {
                Log.Debug("Place recognized = " + _currPlace.Id);
                return true;
            }
            var oldPlace = _currPlace;
            _currPlace = null;
            foreach (
                var place in _places.Where(place => place != oldPlace).Where(place => place.TestInput(measures)))
            {
                _currPlace = place;
                Log.Debug("Place recognized = " + _currPlace.Id);
                return true;
            }
            // REMARK current place was not set up
            Log.Debug("No place recognized");
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
            Log.Debug("Place to be updated = " + _currPlace.Id);
            _currPlace.UpdateStats(measures);
            UpdateTimeStats();
        }

        private void UpdateTimeStats()
        {
            _n += 1;
            _currentN += 1;
            if (_currentN > _longestN)
            {
                _longestN = _currentN;
            }
        }

        public override void ForceLocation(IList<IMeasure> measures)
        {
            // it needs to be called before UpdateStats
            SetUpCurrentTimeStats();

            if (TestInput(measures))
            {
                // currPlace is setup by side-effect
                Debug.Assert(_currPlace != null, "_currPlace != null");
                UpdateStats(measures);
            }
            else
            {
                AddPlace(_Factory.CreatePlace(measures));
                _currPlace = _places.Peek();
                // if UpdateStats is not called, time stats need to be updated
                UpdateTimeStats();
            }


            
        }

        public override void ResetCurrentStreak()
        {
            _currentN = 0;
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