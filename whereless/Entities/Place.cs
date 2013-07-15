using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whereless.NativeWiFi;

namespace whereless.Entities
{
    public abstract class Place
    {
        public virtual int Id { get; protected set; }

        //I have no time for studying a depency injection framework and it should not be the case
        //Consider abstract factory pattern over factory/template pattern (actually commented) if things get messy
        protected abstract Network NetworkFactory(IMeasure measure); 

        public abstract bool TestInput(IList<IMeasure> measures);
        public abstract void UpdateStats(IList<IMeasure> measures);
    }

}
