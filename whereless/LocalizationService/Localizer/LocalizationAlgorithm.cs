using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whereless.Model.Entities;
using whereless.Model.ValueObjects;

namespace whereless.LocalizationService.Localizer
{
    public abstract class LocalizationAlgorithm
    {
        public abstract void Initialize(IList<IMeasure> input, ref Location currLocation, ref bool unknown);
        public abstract void Localize(IList<IMeasure> input, ref Location currLocation, ref bool unknown);
        public abstract void ForceLocation(string name, IList<IMeasure> input, ref Location currLocation, ref bool unknown);
        public abstract void ForceUnknown(IList<IMeasure> input, ref Location currLocation, ref bool unknown);
        public abstract void RegisterLocation(string name, IList<IMeasure> input, ref Location currLocation, ref bool unknown);
    }
}
