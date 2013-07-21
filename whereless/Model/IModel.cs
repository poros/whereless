using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whereless.Model.Factory;
using whereless.Model.Repository;

namespace whereless.Model
{
    public interface IModel
    {
        IRepository<T> GetRepository<T>() where T : class;

        ILocationRepository GetLocationRepository();

        IUnitOfWork GetUnitOfWork();

        IEntitiesFactory EntitiesFactory { get; }
    }
}
