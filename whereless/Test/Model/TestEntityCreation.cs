using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Tool.hbm2ddl;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using whereless.Model;
using whereless.Model.Entities;
using whereless.Model.Factory;
using whereless.NativeWiFi;

namespace whereless.Test.Model
{
    using NUnit.Framework;

    [TestFixture]
    class TestEntityCreation
    {
        // Define a static logger variable so that it references the
        // Logger instance named "MyApp".
        private static readonly ILog Log = LogManager.GetLogger(typeof(TestEntityCreation));

        private const string DbFile = "firstTest.db";

        [Test]
        public void DummyTest()
        {
            var entitiesFactory =  NHModel.EntitiesFactory;

            // create our NHibernate session factory
            var sessionFactory = CreateSessionFactory();

            using (var session = sessionFactory.OpenSession())
            {
                // populate the database
                using (var transaction = session.BeginTransaction())
                {
                    List<IMeasure> input = new List<IMeasure> { new SimpleMeasure("ReteA", 10U) };
                    Location loc = entitiesFactory.CreateLocation("Location1", input);
                    loc.Prova = 9;

                    //this saves everything else via cascading
                    session.SaveOrUpdate(loc);

                    transaction.Commit();
                }
            }

            Location longLocation;

            using (var session = sessionFactory.OpenSession())
            {
                // populate the database
                using (var transaction = session.BeginTransaction())
                {
                    longLocation = session.CreateCriteria(entitiesFactory.LocationType).Add(Restrictions.Eq("Name", "Location1"))
                                           .UniqueResult<Location>();
                    longLocation.Prova = 101;
                    

                    transaction.Commit();
                }
            }

            using (var session = sessionFactory.OpenSession())
            {
                longLocation.Prova = 102;
                session.Update(longLocation);
                session.Flush();
            }

            using (var session = sessionFactory.OpenSession())
            {
                // populate the database
                using (var transaction = session.BeginTransaction())
                {
                   
                    longLocation.Prova = 103;
                    session.Update(longLocation);
                    transaction.Commit();
                }
            }


            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    //var locations = session.CreateCriteria(entitiesFactory.LocationType)
                    //                       .List<Location>();

                    var locations = session.CreateCriteria(typeof(Location))
                                           .List<Location>();

                    foreach (var location in locations)
                    {
                        Console.WriteLine(location.ToString());
                        //session.Delete(location);
                    }

                    transaction.Commit();
                }
            }

            //using (var session = sessionFactory.OpenSession())
            //{
            //    using (session.BeginTransaction())
            //    {

            //        var networks = session.CreateCriteria(entitiesFactory.NetworkType)
            //                .List<Network>();

            //        foreach (var network in networks)
            //        {
            //            Console.WriteLine(network.ToString());
            //        }
            //    }
            //}
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

        private static void BuildSchema(Configuration config)
        {
            // delete the existing db on each run
            if (File.Exists(DbFile))
                File.Delete(DbFile);

            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            new SchemaExport(config)
                .Create(false, true);
        }
    }
}
