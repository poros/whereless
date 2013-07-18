using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whereless.NativeWiFi;

namespace whereless.Interfaces
{
    public interface ILocation
    {
        string Name { get; set; }
        ulong Time { get; set; }

        bool TestInput(IList<IMeasure> input);
        void UpdateStats(IList<IMeasure> input);
    }
}
