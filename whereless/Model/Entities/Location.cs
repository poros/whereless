using System.Collections.Generic;
using whereless.Model.Factory;
using whereless.WiFi;

namespace whereless.Model.Entities
{
    public abstract class Location
    {
        // factory for creating child entities 
        protected static readonly ulong T = 1000; //ms between probes TO BE MOVED
        private string _name;

        public virtual int Id { get; protected set; }
        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual ulong Time { get; set; }


        protected Location() { }

        protected Location(string name)
        {
            _name = name;
        }

        // test if the input measures are compatible with the location
        public abstract bool TestInput(IList<IMeasure> measures);
        // update statistics of the location with the input measures
        public abstract void UpdateStats(IList<IMeasure> measures);
        
        public override string ToString()
        {
            return (this.GetType().Name + ": " + "Name = " + Name + "; Time = " + Time + ";");
        }
    }
}
