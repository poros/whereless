using whereless.NativeWiFi;

namespace whereless.Model.Entities
{
    public abstract class Network
    {
        private string _ssid;

        public virtual int Id { get; protected set; }
        public virtual string Ssid
        {
            get { return _ssid; }
            set { _ssid = value; }
        }

        //public virtual Place Place { get; set; } // Reference for Inverse(). Causes problems, but saves an update.


        protected Network() {}

        protected Network(IMeasure measure)
        {
            _ssid = measure.Ssid;
        }

        // test if the input measures are compatible with the network
        public abstract bool TestInput(IMeasure measure);
        // update statistics of the network with the input measures
        public abstract void UpdateStats(IMeasure measure);
       

        public override string ToString()
        {
            return (this.GetType().Name + ": " + "Ssid = " + Ssid + ";");
        }
    }
}
