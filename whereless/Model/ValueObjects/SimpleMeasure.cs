namespace whereless.Model.ValueObjects
{
    class SimpleMeasure : IMeasure
    {
        public string Ssid { get; set; }
        public uint SignalQuality { get; set; }

        public SimpleMeasure(string ssid, uint signalQuality)
        {
            this.Ssid = ssid;
            this.SignalQuality = signalQuality;
        }

        public override string ToString()
        {
            return (this.GetType().Name + ": Ssid = " + Ssid + " SignalQuality = " + SignalQuality);
        }
    }
}
