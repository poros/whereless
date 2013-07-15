using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whereless.NativeWiFi;

namespace whereless.Entities
{
    public abstract class Location
    {
        protected static readonly ulong T = 1000; //ms between probes TO BE MOVED

        public virtual int Id { get; protected set; }
        public virtual string Name { get; set; }

        public abstract bool TestInput(IList<IMeasure> measures);
        public abstract void UpdateStats(IList<IMeasure> measures);
    }
}
