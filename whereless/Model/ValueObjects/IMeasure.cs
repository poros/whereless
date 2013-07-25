namespace whereless.Model.ValueObjects
{
    public interface IMeasure
    {
        string Ssid { get; set; }
        uint SignalQuality { get; set; }
    }
}
