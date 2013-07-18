using System.Collections.Generic;
using whereless.Model.Factory;
using whereless.NativeWiFi;

namespace whereless.Model.Entities
{
    public abstract class Place
    {
        protected static readonly EntitiesFactory Factory = EntitiesFactory.Factory;
        
        public virtual int Id { get; protected set; }
        // public virtual Location LocationReference { get; set; } // Reference for Inverse(). Causes problems, but saves an update.

        public abstract bool TestInput(IList<IMeasure> measures);
        public abstract void UpdateStats(IList<IMeasure> measures);

        public override string ToString()
        {
            return this.GetType().Name + ":";
        }
    }
}
