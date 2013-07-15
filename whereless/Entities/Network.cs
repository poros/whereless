using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whereless.NativeWiFi;

namespace whereless.Entities
{
    public abstract class Network
    {
        public virtual int Id { get; protected set; }
        public virtual string Ssid { get; set; }

        public abstract void UpdateStats(IMeasure measure);
        public virtual bool IsSameNetwork(IMeasure measure)
        {
            if (measure.Ssid == null) throw new ArgumentNullException("measure");
            return Ssid.Equals(measure.Ssid, System.StringComparison.Ordinal);
        }
    }
}
