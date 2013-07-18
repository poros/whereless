using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using whereless.NativeWiFi;

namespace whereless.Model.Entities
{
    public class ZIndexPlace : Place
    {
        //the two constant to change in order to refine recognition precision
        private static readonly double bigZ = 4D;
        private static readonly double k = 1.96D;
        

        private IDictionary<string, Network> _networks;
        

        public virtual IList<Network> Networks
        {
            get { return _networks.Values.ToList(); }
            protected set { _networks = value.ToDictionary(m => m.Ssid); }
        }

        //NHibernate specific in case of double-side reference
        public virtual void AddNetwork(string ssid, Network net)
        {
            _networks.Add(ssid, net);
            //net.PlaceReference = this;
        }
  

        public ZIndexPlace()
        {
            _networks = new Dictionary<string, Network>();
        }

        public ZIndexPlace(IList<IMeasure> measures)
        {
            _networks = new Dictionary<string, Network>();
            if (measures == null) return;
            foreach (var measure in measures)
            {
                AddNetwork(measure.Ssid, Factory.CreateNetwork(measure));
            }
        }


        public override bool TestInput(IList<IMeasure> measures)
        {
            Dictionary<String, IMeasure> dMeasures = measures.ToDictionary(m => m.Ssid);
            double zIndex = 0;
            ulong n = 0;

            foreach (var gNet in _networks.Values.Select(net => net as GaussianNetwork))
            {
                if (gNet == null)
                {
                    throw new InvalidOperationException("Network was not a GaussianNetwork");
                }
                //max stdDev possible -> great variance accounted for networks only once before
                //(useful for a location first setup)
                //another option: consider also a minimum time of permanence asked to the user
                //for a location first setup 
                n += gNet.N + 1;
                double stdDev = GaussianNetwork.SignalQualityMax;
                if(!gNet.StdDev.Equals(0D))
                {
                    stdDev = gNet.StdDev;
                }
                IMeasure measure;
                if(dMeasures.TryGetValue(gNet.Ssid, out measure))
                {
                    zIndex += (gNet.N + 1) * Math.Abs((measure.SignalQuality - gNet.Mean) / stdDev);
                }
                else
                {
                    //consider also gNet + 1 (change n above for coherence)
                    zIndex += (gNet.N + 1) * Math.Abs((0D - gNet.Mean) / stdDev);
                }
            }

            foreach (var measure in dMeasures.Values)
            {
                if (!_networks.ContainsKey(measure.Ssid))
                {
                    n += 1;
                    zIndex += (measure.SignalQuality / GaussianNetwork.SignalQualityMax) * bigZ; //penalty
                }
            }

            zIndex = zIndex / n;

            return (zIndex.CompareTo(k) <= 0); //double safe comparison
        }

        public override void UpdateStats(IList<IMeasure> measures)
        {
            foreach (IMeasure measure in measures)
            {
                Network net;
                if (_networks.TryGetValue(measure.Ssid, out net))
                {
                    net.UpdateStats(measure);
                }
                else
                {
                    AddNetwork(measure.Ssid, Factory.CreateNetwork(measure));
                }
            }
        }


        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();
            foreach (var network in Networks)
            {
                buffer.AppendLine(network.ToString());
            }
            return (base.ToString() + " Networks = {\n" + buffer + "} ");
        }
    }
}
