using whereless.Model.Factory;
using whereless.Model.Repository;

namespace whereless.Model
{
    //Facade class for ModelHelper instantiation and access

    public class ModelHelper
    {
        private static readonly IModel Instance = InstantiateModel();

        private ModelHelper() {}

        // TODO implement logic in order to instantiate the correct model + factory
        private static IModel InstantiateModel()
        {
            IModel tmp = new NHModel();
            return tmp;
        }

        public static IModel Handle
        {
            get { return Instance; }
        }

        public static IEntitiesFactory EntitiesFactory
        {
            get { return Instance.EntitiesFactory; }
        }
        
        public static IRepository<T> GetRepository<T>() where T : class
        {
            return Instance.GetRepository<T>();
        }

        public static ILocationRepository GetLocationRepository()
        {
            return Instance.GetLocationRepository();
        }

        public static IUnitOfWork GetUnitOfWork()
        {
            return Instance.GetUnitOfWork();
        }
    }
}
