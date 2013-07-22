using System.Configuration;
using log4net;
using whereless.Model.Factory;
using whereless.Model.Repository;
using whereless.Test.Model;

namespace whereless.Model
{
    //Facade class for ModelHelper instantiation and access

    public class ModelHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ModelHelper));
        private static readonly IModel Instance = InstantiateModel();

        private ModelHelper() {}

        // REMARK Logic to instantiate correct factory is demanded to IModel implementation
        private static IModel InstantiateModel()
        {
            IModel tmp;
            string modelName = ConfigurationManager.AppSettings["model"];
            if (modelName == null)
            {
                throw new ConfigurationErrorsException("Unable to find model key");
            }
            if (modelName.Equals("NHibernate"))
            {
                tmp = new NHModel();
                Log.Debug("NHibernate Model Instantiated");
            }
            else
            {
                throw new ConfigurationErrorsException("Model configuration value not allowed");
            }
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
