using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whereless.NativeWiFi;

namespace whereless.Entities
{
    public class ZIndexPlace : Place
    {
        private IDictionary<string, Network> NetworksDictionary { get; set; }
        public virtual IList<Network> Networks {
            get { return NetworksDictionary.Values.ToList(); }
            set { NetworksDictionary = value.ToDictionary(m => m.Ssid); }
        }

        //the two constant to change in order to refine recognition precision
        private static readonly double bigZ = 4D;
        private static readonly double k = 1.96D;

        protected override Network NetworkFactory(IMeasure measure)
        {
            return new GaussianNetwork(measure);
        }

        protected ZIndexPlace() { }

        public ZIndexPlace(IList<IMeasure> measures = null)
        {
            NetworksDictionary = new Dictionary<string, Network>();
            if (measures == null) return;
            foreach (var measure in measures)
            {
                AddNetwork(measure.Ssid, NetworkFactory(measure));
            }
        }

        public override bool TestInput(IList<IMeasure> measures)
        {
            Dictionary<String, IMeasure> dMeasures = measures.ToDictionary(m => m.Ssid);
            double zIndex = 0;
            ulong n = 0;

            foreach (var gNet in NetworksDictionary.Values.Select(net => net as GaussianNetwork))
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
                IMeasure measure = null;
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
                if (!NetworksDictionary.ContainsKey(measure.Ssid))
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
                Network net = null;
                if (NetworksDictionary.TryGetValue(measure.Ssid, out net))
                {
                    net.UpdateStats(measure);
                }
                else
                {
                    AddNetwork(measure.Ssid, NetworkFactory(measure));
                }
            }
        }

        //public virtual Location LocationReference { get; set; }
        public virtual void AddNetwork(string ssid, Network net)
        {
            NetworksDictionary.Add(ssid, net);
            //((GaussianNetwork) net).PlaceReference = this;
        }
    }
}
