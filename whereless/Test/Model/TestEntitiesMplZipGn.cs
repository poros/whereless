using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Testing;
using log4net;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using whereless.Model.Entities;
using whereless.Model.Factory;
using whereless.Model.ValueObjects;

namespace whereless.Test.Model
{
    using NUnit.Framework;

    [TestFixture(Description = "Test for entities MultiPlacesLocation, ZIndexPlace and GaussianNetwork")]
    class TestEntitiesMplZipGn
    {
        // Define a static logger variable so that it references the logger instance
        private static readonly ILog Log = LogManager.GetLogger(typeof(TestEntitiesMplZipGn));

        private const string DbFile = "TestEntitiesMplZipGn.db";

        private ISessionFactory _sessionFactory;

        private const ulong TimeVal = 9000UL;
        private const ulong NVal = TimeVal / 1000UL;
        private const ulong TimeVal1 = 11000UL;
        private const ulong TimeVal2 = 2000UL;
        private const ulong TimeVal3 = 3000UL;
        private const ulong TimeVal4 = 4000UL;


        [TestFixtureSetUp]
        public void CreateDb()
        {
            _sessionFactory = CreateSessionFactory();
            Log.Info("Db created");
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
                    m.FluentMappings.AddFromAssemblyOf<Location>()
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

        // REMARK Comment it if you want to check the db by hand
        [TestFixtureTearDown]
        public void DeleteDb()
        {
            if (File.Exists(DbFile))
            {
                File.Delete(DbFile);
                Log.Info("Db deleted");
            }
        }

        [Test(Description = "MultiPlacesLocation Test with Fluent NHibernate PersistanceSpecification")]
        public void MultiPlacesLocationMappingTest()
        {
            using (var session = _sessionFactory.OpenSession())
            {
                new PersistenceSpecification<MultiPlacesLocation>(session, new CustomEqualityComparer())
                    // .CheckProperty(x => x.Id, 1)
                    .CheckProperty(x => x.Name, "Casa")
                    .CheckProperty(x => x.Time, TimeVal)
                    .CheckProperty(x => x.N, NVal)
                    .CheckList(x => x.Places, MockPlace())
                    .VerifyTheMappings();
            }
        }

        [Test(Description = "ZIndexPlace Test with Fluent NHibernate PersistanceSpecification")]
        public void ZIndexPlaceMappingTest()
        {
            using (var session = _sessionFactory.OpenSession())
            {
                new PersistenceSpecification<ZIndexPlace>(session, new CustomEqualityComparer())
                    // .CheckProperty(x => x.Id, 1)
                    .CheckList(x => x.Networks, MockNetwork())
                    .VerifyTheMappings();
            }
        }

        [Test(Description = "GaussianNetwork Test with Fluent NHibernate PersistanceSpecification")]
        public void GaussianNetworkMappingTest()
        {
            using (var session = _sessionFactory.OpenSession())
            {
                new PersistenceSpecification<GaussianNetwork>(session, new CustomEqualityComparer())
                    // .CheckProperty(x => x.Id, 1)
                    .CheckProperty(x => x.Ssid, "Sitecom")
                    .CheckProperty(x => x.N, 10UL)
                    .CheckProperty(x => x.Mean, 80D)
                    .CheckProperty(x => x.StdDev, 2D)
                    .VerifyTheMappings();
            }
        }

        [Test(Description = "MplZipGn Factory Test")]
        public void FactoryTest()
        {
            AuxiliaryFactoryTest(new MplZipGn());

        }

        public static void AuxiliaryFactoryTest(IEntitiesFactory factory)
        {
            //Location
            var inputL = new List<IMeasure> { new SimpleMeasure("ReteA", 10U) };
            var loc = factory.CreateLocation("Location1", inputL);
            Assert.IsInstanceOf<Location>(loc);
            Assert.AreEqual(loc.Name, "Location1");
            loc.Time = TimeVal;
            Assert.AreEqual(loc.Time, TimeVal);

            //Place
            var pla = factory.CreatePlace(inputL);
            Assert.IsInstanceOf<Place>(pla);

            //Location
            var inputS = new SimpleMeasure("ReteA", 10U);
            var net = factory.CreateNetwork(inputS);
            Assert.IsInstanceOf<Network>(net);
            Assert.AreEqual(net.Ssid, "ReteA");
        }

        [Test(Description =
            "CRUD Test (also useful to understand NHibernate save and update logic and sessions/transactions)")]
        public void CRUDTest()
        {
            IEntitiesFactory factory = new MplZipGn();
            // CREATE Test
            using (var session = _sessionFactory.OpenSession())
            {
                // populate the database
                using (var transaction = session.BeginTransaction())
                {
                    var input = new List<IMeasure> { new SimpleMeasure("ReteA", 10U) };
                    var loc = factory.CreateLocation("Location1", input);
                    loc.Time = TimeVal;

                    Assert.IsInstanceOf<Location>(loc);
                    Assert.AreEqual(loc.Name, "Location1");
                    Assert.AreEqual(loc.Time, TimeVal);

                    var input2 = new List<IMeasure> { new SimpleMeasure("ReteB", 50U), new SimpleMeasure("ReteC", 100U) };
                    var loc2 = factory.CreateLocation("Location2", input2);
                    loc2.Time = TimeVal1;
                    Assert.IsInstanceOf<Location>(loc2);
                    Assert.AreEqual(loc2.Name, "Location2");
                    Assert.AreEqual(loc2.Time, TimeVal1);


                    //this saves everything else via cascading
                    session.SaveOrUpdate(loc); //the same of using session.Save(loc)
                    session.SaveOrUpdate(loc2);

                    transaction.Commit();
                }
            }

            // READ Test
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    // First example of query
                    var locations = session.CreateCriteria(typeof(Location))
                                           .List<Location>();
                    Assert.AreEqual(locations.Count, 2);
                    foreach (var location in locations)
                    {
                        //For a visual feedback
                        Console.WriteLine(location.ToString());
                        if (location.Name == "Location1")
                        {
                            Assert.AreEqual(location.Time, TimeVal);
                        } else if (location.Name == "Location2")
                        {
                            Assert.AreEqual(location.Time, TimeVal1);
                        }
                        else
                        {
                            Log.Debug(location.Name);
                            Assert.Fail("Location name not matching");
                        }
                    }

                    // REMARK Testing ZIndexPlace and GaussianNetwork, not Place and Network
                    var places = session.CreateCriteria(typeof(ZIndexPlace))
                                          .List<ZIndexPlace>();

                    Assert.AreEqual(places.Count, 2);
                    Assert.AreEqual(places[0].Networks.Count, 1);
                    Assert.AreEqual(places[1].Networks.Count, 2);

                    var networks = session.CreateCriteria(typeof(GaussianNetwork))
                                          .List<GaussianNetwork>();
                    Assert.AreEqual(networks.Count, 3);
                    foreach (var network in networks)
                    {
                        if (network.Ssid == "ReteA")
                        {
                            Assert.AreEqual(network.Mean, 10D);
                            Assert.AreEqual(network.N, 1UL);
                            Assert.AreEqual(network.StdDev, 0D);
                        }
                        else if (network.Ssid == "ReteB")
                        {
                            Assert.AreEqual(network.Mean, 50D);
                            Assert.AreEqual(network.N, 1UL);
                            Assert.AreEqual(network.StdDev, 0D);
                        }
                        else if (network.Ssid == "ReteC")
                        {
                            Assert.AreEqual(network.Mean, 100D);
                            Assert.AreEqual(network.N, 1UL);
                            Assert.AreEqual(network.StdDev, 0D);
                        }
                        else
                        {
                            Assert.Fail("Network Ssid not matching");
                        }
                    }
                    
                    transaction.Commit();
                }
            }


            // UPDATE test
            Location longLivedLocation;
            const string locName = "Location1";

            // update within a single session/transaction
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    // second query example
                    longLivedLocation = session.CreateCriteria(typeof(Location)).Add(Restrictions.Eq("Name", locName))
                                           .UniqueResult<Location>();
                    Assert.AreEqual(longLivedLocation.Name, locName);
                    
                    longLivedLocation.Time = TimeVal2;

                    transaction.Commit();
                }
            }

            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var tmp = session.CreateCriteria(typeof(Location)).Add(Restrictions.Eq("Name", locName))
                                           .UniqueResult<Location>();
                    Assert.AreEqual(tmp.Time, TimeVal2);
                }
            }
            

            // update of an entity retrieved in a different session
            using (var session = _sessionFactory.OpenSession())
            {
                longLivedLocation.Time = TimeVal3;
                session.Update(longLivedLocation);
                session.Flush();
            }

            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var tmp = session.CreateCriteria(typeof(Location)).Add(Restrictions.Eq("Name", locName))
                                           .UniqueResult<Location>();
                    Assert.AreEqual(tmp.Time, TimeVal3);
                }
            }

            // update of an entity retrieved in a different session (using transactions)
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {

                    longLivedLocation.Time = TimeVal4;
                    session.SaveOrUpdate(longLivedLocation);
                    transaction.Commit();
                }
            }

            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var tmp = session.CreateCriteria(typeof(Location)).Add(Restrictions.Eq("Name", locName))
                                           .UniqueResult<Location>();
                    Assert.AreEqual(tmp.Time, TimeVal4);
                }
            }

            // DELETE test
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var locations = session.CreateCriteria(typeof(Location))
                                           .List<Location>();

                    foreach (var location in locations)
                    {
                        session.Delete(location);
                    }

                    transaction.Commit();
                }
            }

            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var locations = session.CreateCriteria(typeof(Location))
                                           .List<Location>();
                    Assert.AreEqual(locations.Count, 0);

                    // test cascade deletion
                    var entities = session.CreateCriteria<Object>().List();
                    Assert.AreEqual(entities.Count, 0);

                    transaction.Commit();
                }
            }
           
        }



        private IList<Place> MockPlace()
        {
            var input = new List<IMeasure> { new SimpleMeasure("Sitecom", 99U) };
            var list = new List<Place> { new ZIndexPlace(input) };
            return list;
        }

        private IList<Network> MockNetwork()
        {
            var input = new SimpleMeasure("Sitecom", 99U);
            var list = new List<Network> { new GaussianNetwork(input) };
            return list;
        }

        public class CustomEqualityComparer : IEqualityComparer
        {
            public new bool Equals(object x, object y)
            {
                Log.Debug(x.GetType().Name + ":" + x + " == " + y.GetType().Name + ":" + y);
                if (x == null || y == null)
                {
                    return false;
                }
                if (x is MultiPlacesLocation && y is MultiPlacesLocation)
                {
                    return ((MultiPlacesLocation)x).Name == ((MultiPlacesLocation)y).Name;
                }
                if (x is ZIndexPlace && y is ZIndexPlace)
                {
                    return ((ZIndexPlace)x).Id == ((ZIndexPlace)y).Id;
                }
                if (x is GaussianNetwork && y is GaussianNetwork)
                {
                    return ((GaussianNetwork)x).Ssid == ((GaussianNetwork)y).Ssid;
                }
                return x.Equals(y);
            }

            public int GetHashCode(object obj)
            {
                throw new NotImplementedException();
            }
        }

    }
}
