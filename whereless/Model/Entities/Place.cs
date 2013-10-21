using System.Collections.Generic;
using whereless.Model.ValueObjects;

namespace whereless.Model.Entities
{
    public abstract class Place
    {
        public virtual int Id { get; protected set; }
        public virtual Location Location { get; set; } // Reference for Inverse(). Causes problems, but saves an update.

        // test if the input measures are compatible with the place
        public abstract double TestInput(IList<IMeasure> measures);
        // update statistics of the place with the input measures
        public abstract void UpdateStats(IList<IMeasure> measures);
        public abstract ulong GetObservations();
        public abstract void RestartLearning();

        public override string ToString()
        {
            return this.GetType().Name + ":";
        }
    }
}
