using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whereless.Model.Entities
{
    public class Activity
    {
        public virtual int Id { get; protected set; }
        public virtual string Name { get; set; }

        public Activity()
        {
        }

        public Activity(string name)
        {
            Name = name;
        }

        public virtual void Start()
        {
            throw new NotImplementedException();
        }

        public virtual void Stop()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return (this.GetType().Name + ": " + "Name = " + Name);
        }
    }
}
