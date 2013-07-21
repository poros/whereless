using System.Collections.Generic;
using whereless.Model.Factory;
using whereless.NativeWiFi;

namespace whereless.Model.Entities
{
    public abstract class Place
    {
        public virtual int Id { get; protected set; }
        // public virtual Location LocationReference { get; set; } // Reference for Inverse(). Causes problems, but saves an update.

        // test if the input measures are compatible with the place
        public abstract bool TestInput(IList<IMeasure> measures);
        // update statistics of the place with the input measures
        public abstract void UpdateStats(IList<IMeasure> measures);

        public override string ToString()
        {
            return this.GetType().Name + ":";
        }
    }
}
