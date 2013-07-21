using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whereless.Model.Entities;

namespace whereless.Model.Repository
{
    public interface ILocationOperations
    {
        Location GetLocationByName(string name);
    }
}
