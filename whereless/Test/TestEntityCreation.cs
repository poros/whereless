using System;
using System.IO;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whereless.Entities;
using whereless.NativeWiFi;

namespace whereless.Test
{
    using NUnit.Framework;

    [TestFixture]
    class TestEntityCreation
    {
        private const string DbFile = "firstTest.db";

        [Test]
        public void TestNetworkCreate()
        {
            // create our NHibernate session factory
            var sessionFactory = CreateSessionFactory();

            using (var session = sessionFactory.OpenSession())
            {
                // populate the database
                using (var transaction = session.BeginTransaction())
                {
                    Network net = new GaussianNetwork(new SimpleMeasure("ReteA", 10U));

                    ZIndexPlace pl = new ZIndexPlace();
                    pl.AddNetwork(net.Ssid, net);

                    MultiPlacesLocation loc = new MultiPlacesLocation("Location1");
                    loc.AddPlace(pl);


                    // save both stores, this saves everything else via cascading
                    //session.SaveOrUpdate(net);
                    //session.SaveOrUpdate(pl);
                    session.SaveOrUpdate(loc);

                    transaction.Commit();
                }
            }

            using (var session = sessionFactory.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    //var networks = session.CreateCriteria(typeof(GaussianNetwork))
                    //    .List<GaussianNetwork>();

                    //foreach (var tmp in networks)
                    //{
                    //    WriteNetworkPretty(tmp);
                    //}

                    //var places = session.CreateCriteria(typeof(ZIndexPlace))
                    //    .List<ZIndexPlace>();

                    //foreach (var tmp in places)
                    //{
                    //    WritePlacePretty(tmp);
                    //}
                    
                    var locations = session.CreateCriteria(typeof(MultiPlacesLocation))
                        .List<MultiPlacesLocation>();

                    foreach (var tmp in locations)
                    {
                        WriteLocationPretty(tmp);
                    }

                    var networks = session.CreateCriteria(typeof(Network))
                        .List<Network>();

                    foreach (var network in networks)
                    {
                        Console.WriteLine(network.ToString());
                    }
                }
            }
        }

        private static void WriteNetworkPretty(GaussianNetwork network)
        {
            Console.WriteLine(network.Ssid + " " + network.Mean + " " + network.StdDev + " " + network.N);
        }

        private static void WritePlacePretty(ZIndexPlace place)
        {
            Console.WriteLine("Place " + place.Id + ":");
            foreach (var network in place.Networks)
            {
                WriteNetworkPretty((GaussianNetwork)network);
            }
        }

        private static void WriteLocationPretty(MultiPlacesLocation location)
        {
            Console.WriteLine("Location "+ location.Name + ":");
            foreach (var place in location.Places)
            {
                WritePlacePretty((ZIndexPlace)place);
            }
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
