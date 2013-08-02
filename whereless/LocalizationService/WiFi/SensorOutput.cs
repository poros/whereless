using System.Collections.Generic;
using System.Text;
using whereless.Model.ValueObjects;

namespace whereless.LocalizationService.WiFi
{
    public class SensorOutput
    {
        public IList<IMeasure> Measures { get; set; }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            foreach (var measure in Measures)
            {
                buffer.AppendLine(measure.ToString());
            }
            return (base.ToString() + " Measures = {\n" + buffer + "} ");
        }
    }
}
