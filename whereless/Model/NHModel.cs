using System.IO;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using whereless.Model.Repository;


namespace whereless.Model
{
    public class NHModel : IModel
    {
        private const string DbFile = "whereless.db";

        private static readonly ISessionFactory _sessionFactory = CreateSessionFactory();

        internal static ISessionFactory SessionFactory
        {
            get { return _sessionFactory; }
        }

        public static IRepository<T> GetRepository<T>() where T : class 
        {
            return new NHRepository<T>();
        } 

        public static IUnitOfWork GetUnitOfWork()
        {
            return new NHUnitOfWork();
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
