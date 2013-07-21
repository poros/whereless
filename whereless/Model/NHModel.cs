using System.IO;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using log4net;
using whereless.Model.Factory;
using whereless.Model.Repository;
using whereless.Test.Model;


namespace whereless.Model
{
    public class NHModel : IModel
    {
        private static readonly NHModel Instance = new NHModel();

        private NHModel() {}

        public static NHModel Handle
        {
            get { return Instance; }
        }

        private const string DbFile = "whereless.db";
        // REMARK The order of these two is really important!!!
        // the entitiesFactory is fetched into Entities before having been set, if swapped
        // static are executed in the order they are declared when one of them is referenced
        // the first time during execution (except in case of static constructor)
        private static readonly IEntitiesFactory _entitiesFactory = CreateEntitiesFactory();
        private static readonly ISessionFactory _sessionFactory = CreateSessionFactory();

        

        internal static ISessionFactory SessionFactory
        {
            get { return _sessionFactory; }
        }

        public static IRepository<T> GetRepository<T>() where T : class 
        {
            return new NHRepository<T>();
        }

        public static ILocationRepository GetLocationRepository()
        {
            return new LocationNHRepository();
        }

        public static IUnitOfWork GetUnitOfWork()
        {
            return new NHUnitOfWork();
        }

        public static IEntitiesFactory EntitiesFactory
        {
            get { return _entitiesFactory; }
        }

        private static IEntitiesFactory CreateEntitiesFactory()
        {
            return new MplZipGn();
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
    }
}
