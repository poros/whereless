using System.Configuration;
using System.IO;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using log4net;
using whereless.Model.Factory;
using whereless.Model.Repository;
using Configuration = NHibernate.Cfg.Configuration;


namespace whereless.Model
{
    public class NHModel : IModel
    {
        // REMARK KEEP THE ORDER OF ALL THE STATIC FIELDS AND KEEP ATTENTION TO MUTUAL REFERMENTS
        // Oh man, I need to fix this...
        private static readonly ILog Log = LogManager.GetLogger(typeof(NHModel));
        private static readonly string DbFile = ReadDbName();
        // REMARK The order of these two is really important!!!
        // the entitiesFactory is fetched into Entities before having been set, if swapped
        // static are executed in the order they are declared when one of them is referenced
        // the first time during execution (except in case of static constructor)
        private static readonly IEntitiesFactory _entitiesFactory = CreateEntitiesFactory();
        private static readonly ISessionFactory _sessionFactory = CreateSessionFactory();

        public NHModel()
        {

        }

        public IRepository<T> GetRepository<T>() where T : class 
        {
            return new NHRepository<T>(_sessionFactory);
        }

        public ILocationRepository GetLocationRepository()
        {
            return new LocationNHRepository(_sessionFactory);
        }

        public IUnitOfWork GetUnitOfWork()
        {
            return new NHUnitOfWork(_sessionFactory);
        }

        public IEntitiesFactory EntitiesFactory
        {
            get { return _entitiesFactory; }
        }

        internal ISessionFactory SessionFactory
        {
            get { return _sessionFactory; }
        }



        private static string ReadDbName()
        {
            string tmp = ConfigurationManager.AppSettings["databaseName"];
            if (tmp == null)
            {
                throw new ConfigurationErrorsException("Unable to find databaseName key");
            }
            Log.Debug("DB Name = " + tmp);
            return tmp;
        }

        private static IEntitiesFactory CreateEntitiesFactory()
        {
            IEntitiesFactory tmp;
            string factoryName = ConfigurationManager.AppSettings["entities"];
            if (factoryName == null)
            {
                throw new ConfigurationErrorsException("Unable to find entities key");
            }
            if (factoryName.Equals("MplZipGn"))
            {
                tmp = new MplZipGn();
                Log.Debug("MplZipGn Factory Instantiated");
            }
            else
            {
                throw new ConfigurationErrorsException("Entities configuration value not allowed");
            }
            return tmp;
        }

        /// <summary>
        /// Configure NHibernate. This method returns an ISessionFactory instance that is
        /// populated with mappings created by Fluent NHibernate.
        /// 
        /// Line 1:   Begin configuration
        ///      2+3: Configure the database being used (SQLite file db)
        ///      4+5: Specify what mappings are going to be used
        ///      6:   Expose the underlying configuration instance to the BuildSchema method,
        ///           this creates the database.
        ///      7:   Finally, build the session factory.
        /// </summary>
        /// <returns></returns>
        private static ISessionFactory CreateSessionFactory()
        {
            // delete the existing db on each run
            //if (File.Exists(DbFile))
            //    File.Delete(DbFile);

            if (!File.Exists(DbFile))
            {
                Log.Debug("Creating new DB");
                return Fluently.Configure()
                               .Database(SQLiteConfiguration.Standard
                                                            .UsingFile(DbFile))
                               .Mappings(m =>
                                         m.FluentMappings.AddFromAssemblyOf<App>()
                                        //.Conventions.Add(DefaultCascade.All())
                                )
                               .ExposeConfiguration(BuildSchema)
                               .BuildSessionFactory();
            }
            else
            {
                Log.Debug("Using already existent DB");
                return Fluently.Configure()
                               .Database(SQLiteConfiguration.Standard
                                                            .UsingFile(DbFile))
                               .Mappings(m =>
                                         m.FluentMappings.AddFromAssemblyOf<App>()
                                        //.Conventions.Add(DefaultCascade.All())
                                )
                               .BuildSessionFactory();
            }
           
        }

        private static void BuildSchema(Configuration config)
        {
            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            new SchemaExport(config)
                .Create(false, true);
        }

        internal static IEntitiesFactory GetEntitiesFactory()
        {
            return _entitiesFactory;
        }
    }
}
