using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using whereless.Model.Factory;
using whereless.Model.ValueObjects;

namespace whereless.Model.Entities
{

    // implements a check of compatibility based on the z-score of the input measure of each network
    // weighted by the number of observation collected per each network
    public class ZIndexPlace : Place
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ZIndexPlace));

        // factory for creating child entities
        //REMARK!!! This is an emergency solution. Impossible to move it in ModelHelper
        // but in general it should not be here.
        // Consider to redisgn the whole entity layer
        private readonly IEntitiesFactory _Factory = NHModel.GetEntitiesFactory();

        // the two constant to change in order to refine recognition precision
        // define the penalty in terms of standard deviation for just discovered networks
        private static readonly double bigZ = 2D;
        // defines the range of input acceptability (99.99% -> 1.96D, 68.2% -> 1)
        private static readonly double k = 8D;
        

        private IDictionary<string, Network> _networks;

        private ulong _n;
        public virtual ulong N
        {
            get { return _n; }
            set { _n = value; }
        }


        public virtual IList<Network> Networks
        {
            get { return _networks.Values.ToList(); }
            protected set { _networks = value.ToDictionary(m => m.Ssid); }
        }

        //NHibernate specific in case of double-side reference
        public virtual void AddNetwork(string ssid, Network net)
        {
            _networks.Add(ssid, net);
            net.Place = this;
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
                AddNetwork(measure.Ssid, _Factory.CreateNetwork(measure));
            }
            _n = 1;
        }

        public virtual double ZIndex(IList<IMeasure> measures)
        {
            Dictionary<String, IMeasure> dMeasures = measures.ToDictionary(m => m.Ssid);
            double zIndex = 0;
            ulong n = 0;

            // foreach network already registered
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
                // in order to avoid 0 division for newly added networks (standard dev = 0)
                if (!gNet.StdDev.Equals(0D))
                {
                    stdDev = gNet.StdDev;
                }
                IMeasure measure;
                // if the network is present in the input measures
                if (dMeasures.TryGetValue(gNet.Ssid, out measure))
                {
                    zIndex += (gNet.N + 1) * Math.Abs((measure.SignalQuality - gNet.Mean) / stdDev);
                }
                // if not consider a measure with signal quality equal to 0
                else
                {
                    //consider also gNet.N (change n above for coherence, too)
                    zIndex += (gNet.N + 1) * Math.Abs((0D - gNet.Mean) / stdDev);
                }
                //Log.Debug(zIndex);
            }

            // foreach just discovered network
            foreach (var measure in dMeasures.Values)
            {
                if (!_networks.ContainsKey(measure.Ssid))
                {
                    // accounts for a big z-score (is it enough???)
                    n += 1;
                    zIndex += (measure.SignalQuality / GaussianNetwork.SignalQualityMax) * bigZ;
                }
            }

            zIndex = zIndex / n;
            Log.Debug("Z-Index = " + zIndex);
            return zIndex;
        }

        public override double TestInput(IList<IMeasure> measures)
        {
            if (measures.Count == 0)
            {
                return -1D;
            }
            double z = ZIndex(measures);
            if (z.CompareTo(k) <= 0)
            {
                return z;     
            }
            return -1D;
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
                    AddNetwork(measure.Ssid, _Factory.CreateNetwork(measure));
                }
            }
            _n++;
        }

        public override ulong GetObservations()
        {
            return _n;
        }

        public override void RestartLearning()
        {
            _n = 0;
        }

        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();
            foreach (var network in Networks)
            {
                buffer.AppendLine(network.ToString());
            }
            return (base.ToString() + " N = " + N + "; Networks = {\n" + buffer + "} ");
        }
    }
}
