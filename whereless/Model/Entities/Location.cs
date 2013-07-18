using System.Collections.Generic;
using whereless.Model.Factory;
using whereless.NativeWiFi;

namespace whereless.Model.Entities
{
    public abstract class Location
    {
        protected static readonly EntitiesFactory Factory = EntitiesFactory.Factory;
        protected static readonly ulong T = 1000; //ms between probes TO BE MOVED

        public virtual int Id { get; protected set; }
        public virtual string Name { get; set; }
        public virtual ulong Time { get; set; }

        public abstract bool TestInput(IList<IMeasure> measures);
        public abstract void UpdateStats(IList<IMeasure> measures);
        
        public override string ToString()
        {
            return (this.GetType().Name + ": " + "Name = " + Name + "; Time = " + Time + ";");
        }
    }
}
