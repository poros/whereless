using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whereless.NativeWiFi;

namespace whereless.Entities
{
    public class MultiPlacesLocation : Location
    {
        public virtual ulong N { get; set; }
        public virtual ulong Time
        {
            get { return N * Location.T; }
            set { N = value / Location.T; }
        }

        //Locations are saved from most recent to oldest,
        //because in case of locations with similar footprints (test may pass for both)
        //more recent ones MUST be preferred, since the algorithm look for locations in order!!!
        //(users are stupid, they can register the same location twice,
        //let's give them the last, it would be the less disappointing thing to do)
        private Stack<Place> PlacesStack { get; set; }
        public virtual IList<Place> Places
        {
            get { return PlacesStack.ToList(); }
            set { PlacesStack = new Stack<Place>(value); }
        } 
        private Place currPlace;

        protected virtual Place PlaceFactory(IList<IMeasure> measures)
        {
            return new ZIndexPlace(measures);
        }

        protected MultiPlacesLocation() { }

        public MultiPlacesLocation(String name)
        {
            this.Name = name;
            PlacesStack = new Stack<Place>();
        }

        public MultiPlacesLocation(string name, IList<IMeasure> measures)
        {
            this.Name = name;
            PlacesStack = new Stack<Place>();
            AddPlace(PlaceFactory(measures));           
        }

        public override bool TestInput(IList<IMeasure> measures)
        {
            if (currPlace != null && currPlace.TestInput(measures))
            {
                return true;
            }
            var oldPlace = currPlace;
            currPlace = null;
            foreach (var place in PlacesStack.Where(place => place != oldPlace).Where(place => place.TestInput(measures)))
            {
                currPlace = place;
                return true;
            }
            return false;
        }

        public override void UpdateStats(IList<IMeasure> measures)
        {
            //just to be sure that a current place is set
            if (currPlace == null && !TestInput(measures))
            {
                currPlace = PlacesStack.Peek();
            }
            currPlace.UpdateStats(measures);
            N += 1;
        }

        public virtual void AddPlace(Place place)
        {
            PlacesStack.Push(place);
            //((ZIndexPlace) place).LocationReference = this;
        }
    }
}
