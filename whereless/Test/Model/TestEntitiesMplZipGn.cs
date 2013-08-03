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
using whereless.LocalizationService.WiFi;
using whereless.Model.Entities;
using whereless.Model.Factory;
using whereless.Model.ValueObjects;
using System.Linq;
using System.Text;

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

        private const ulong TimeVal = 9UL * WiFiSensor.ScanTime;
        private const ulong NVal = TimeVal / WiFiSensor.ScanTime;
        private const ulong TimeVal1 = 11UL * WiFiSensor.ScanTime;
        private const ulong TimeVal2 = 2UL * WiFiSensor.ScanTime;
        private const ulong TimeVal3 = 3UL * WiFiSensor.ScanTime;
        private const ulong TimeVal4 = 4UL * WiFiSensor.ScanTime;


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

        [TearDown]
        public void EmptyDb()
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var entities = session.CreateCriteria(typeof(Object))
                                           .List<Object>();
                    foreach (var entity in entities)
                    {
                        session.Delete(entity);
                    }
                    transaction.Commit();
                }
            }
            Log.Debug("Empty DB Assured");
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
                    .CheckList(x => x.ActivityList, MockActivity())
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
                    .CheckProperty(x => x.S, 2D)
                    .VerifyTheMappings();
            }
        }

        [Test(Description = "Activity Test with Fluent NHibernate PersistanceSpecification")]
        public void ActivityMappingTest()
        {
            using (var session = _sessionFactory.OpenSession())
            {
                new PersistenceSpecification<Activity>(session, new CustomEqualityComparer())
                    // .CheckProperty(x => x.Id, 1)
                    .CheckProperty(x => x.Name, "AlarmClock")
                    .CheckProperty(x => x.Argument, "")
                    .CheckProperty(x => x.Pathfile, "")
                    .CheckProperty(x => x.Type, Activity.ActivityType.BatchFile)
                    .VerifyTheMappings();
            }
        }

        [Test(Description = "GaussianNetwork Exception Test Constructor in case of out-of-range SignalQuality")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GaussianNetworkOutOfRangeMeasureConstructor()
        {
            var net = new GaussianNetwork(new SimpleMeasure("WrongNet", 101U));
        }

        [Test(Description = "GaussianNetwork Exception Test TestInput in case of out-of-range SignalQuality")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GaussianNetworkOutOfRangeMeasureTestInputMethod()
        {
            var net = new GaussianNetwork(new SimpleMeasure("WrongNet", 10U));
            net.TestInput(new SimpleMeasure("WrongNet", 101U));
        }

        [Test(Description = "GaussianNetwork Exception Test UpdateStats in case of out-of-range SignalQuality")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GaussianNetworkOutOfRangeMeasureUpdateStatsMethod()
        {
            var net = new GaussianNetwork(new SimpleMeasure("WrongNet", 10U));
            net.UpdateStats(new SimpleMeasure("WrongNet", 101U));
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

                    loc.AddActivity(new Activity("AlarmClock"));
                    loc.AddActivity(new Activity("OpenBrowser"));
                    loc2.AddActivity(new Activity("AlarmClock"));

                  

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
                        }
                        else if (location.Name == "Location2")
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

                    var activities = session.CreateCriteria(typeof(Activity))
                                          .List<Activity>();
                    foreach (var activity in activities)
                    {
                        Console.WriteLine(activity.ToString());
                    }
                    Assert.AreEqual(3, activities.Count);
                    foreach (var activity in activities)
                    {
                        if (activity.Name.Equals("AlarmClock"))
                        {
                            Assert.Pass();
                        }
                        else if (activity.Name.Equals("OpenBrowser"))
                        {
                            Assert.Pass();
                        }
                        else
                        {
                            Assert.Fail("Activity Name not matching");
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

        //REMARK: values used for test
        //K = 1.96D;
        //StableN = 10U;
        [Test(Description = "GaussianNetwork business logic test")]
        public void GaussianNetworkBusinessLogic()
        {
            GaussianNetwork net = new GaussianNetwork(new SimpleMeasure("Calvino", 80U));
            Assert.AreEqual(net.Mean, 80D, 0.000000000001D);
            Assert.AreEqual(net.S, 0D, 0.000000000001D);
            Assert.AreEqual(net.StdDev, 0, 0.000000000001D);
            Assert.AreEqual(net.N, 1);

            Assert.IsFalse(net.TestInput(new SimpleMeasure("Galileo", 80U)));
            // under stableN, the SSID is enough
            Assert.IsTrue(net.TestInput(new SimpleMeasure("Calvino", 80U)));
            Assert.IsTrue(net.TestInput(new SimpleMeasure("Calvino", 79U)));
            Assert.IsTrue(net.TestInput(new SimpleMeasure("Calvino", 10U)));


            net.UpdateStats(new SimpleMeasure("Calvino", 40U));
            Assert.AreEqual(net.Mean, 60D, 0.000000000001D); //12 digits precision
            Assert.AreEqual(net.S, 800D, 0.000000000001D);
            Assert.AreEqual(net.StdDev, 20D, 0.000000000001D);
            Assert.AreEqual(net.N, 2);

            Assert.IsFalse(net.TestInput(new SimpleMeasure("Galileo", 60U)));
            // under stableN, the SSID is enough
            Assert.IsTrue(net.TestInput(new SimpleMeasure("Calvino", 40U)));
            Assert.IsTrue(net.TestInput(new SimpleMeasure("Calvino", 59U)));
            Assert.IsTrue(net.TestInput(new SimpleMeasure("Calvino", 10U)));


            net.UpdateStats(new SimpleMeasure("Calvino", 0U));
            Assert.AreEqual(net.Mean, 40D, 0.000000000001D);
            Assert.AreEqual(net.S, 3200, 0.000000000001D);
            Assert.AreEqual(net.StdDev, 32.659863237109D, 0.000000000001D);
            Assert.AreEqual(net.N, 3);

            Assert.IsFalse(net.TestInput(new SimpleMeasure("Galileo", 40U)));
            // under stableN, the SSID is enough
            Assert.IsTrue(net.TestInput(new SimpleMeasure("Calvino", 40U)));
            Assert.IsTrue(net.TestInput(new SimpleMeasure("Calvino", 39U)));
            Assert.IsTrue(net.TestInput(new SimpleMeasure("Calvino", 10U)));

            for (int i = 0; i < 1000; i++)
            {
                net.UpdateStats(new SimpleMeasure("Calvino", 40U));
                net.UpdateStats(new SimpleMeasure("Calvino", 35U));
                net.UpdateStats(new SimpleMeasure("Calvino", 37U));
                net.UpdateStats(new SimpleMeasure("Calvino", 38U));
            }
            Assert.AreEqual(net.Mean, 37.501873594804D, 0.000000000001D);
            Assert.AreEqual(net.S, 16218.735948039D, 0.000000001D);
            Assert.AreEqual(net.StdDev, 2.01286990465602D, 0.000000000001D);
            Assert.AreEqual(net.N, 4003);

            Assert.IsFalse(net.TestInput(new SimpleMeasure("Galileo", 37U)));
            Assert.IsTrue(net.TestInput(new SimpleMeasure("Calvino", (uint)Math.Round(net.Mean))));
            // 1 StdDev
            double delta1 = 1 * net.StdDev - 0.001D;
            Assert.IsTrue(net.TestInput(new SimpleMeasure("Calvino", (uint)Math.Floor(net.Mean + delta1))));
            Assert.IsTrue(net.TestInput(new SimpleMeasure("Calvino", (uint)Math.Ceiling(net.Mean - delta1))));
            // K * StdDev
            double deltaK = GaussianNetwork.K * net.StdDev - 0.001D;
            Assert.IsTrue(net.TestInput(new SimpleMeasure("Calvino", (uint)Math.Floor(net.Mean + deltaK))));
            Assert.IsTrue(net.TestInput(new SimpleMeasure("Calvino", (uint)Math.Ceiling(net.Mean - deltaK))));
            // Over
            double deltaO = GaussianNetwork.K * net.StdDev + 1D;
            Assert.IsFalse(net.TestInput(new SimpleMeasure("Calvino", (uint)Math.Floor(net.Mean + deltaO))));
            Assert.IsFalse(net.TestInput(new SimpleMeasure("Calvino", (uint)Math.Ceiling(net.Mean - deltaO))));
            Assert.IsFalse(net.TestInput(new SimpleMeasure("Calvino", 0U)));
            Assert.IsFalse(net.TestInput(new SimpleMeasure("Calvino", 100U)));

            // Check K * StdDev < SignalQualityUnit special case
            for (int i = 0; i < 1000000; i++)
            {
                net.UpdateStats(new SimpleMeasure("Calvino", 37U));
            }
            Assert.AreEqual(net.Mean, 37D, 0.5D);
            Assert.Less((net.StdDev * GaussianNetwork.K).CompareTo(GaussianNetwork.SignalQualityUnit), 1);
            double deltaU = GaussianNetwork.SignalQualityUnit - 0.001D;
            double deltaUO = GaussianNetwork.SignalQualityUnit + 1D;
            Assert.IsTrue(net.TestInput(new SimpleMeasure("Calvino", (uint)Math.Floor(net.Mean + deltaU))));
            Assert.IsTrue(net.TestInput(new SimpleMeasure("Calvino", (uint)Math.Ceiling(net.Mean - deltaU))));
            Assert.IsFalse(net.TestInput(new SimpleMeasure("Calvino", (uint)Math.Floor(net.Mean + deltaUO))));
            Assert.IsFalse(net.TestInput(new SimpleMeasure("Calvino", (uint)Math.Ceiling(net.Mean - deltaUO))));
        }

        // REMARK values used fo test
        // bigZ = 4D;
        // K = 1D
        [Test(Description = "ZIndexPlace business logic test")]
        public void ZIndexPlaceBusinessLogic()
        {
            var input1 = new List<IMeasure> { new SimpleMeasure("ReteA", 10U), new SimpleMeasure("ReteB", 80U), new SimpleMeasure("ReteC", 100U) };
            var place = new ZIndexPlace(input1);

            var input2 = new List<IMeasure> { new SimpleMeasure("ReteB", 40U), new SimpleMeasure("ReteD", 5U), new SimpleMeasure("ReteA", 10U) };

            //TestInput method
            Assert.AreEqual(place.ZIndex(input1), 0D, Double.Epsilon);
            Assert.AreEqual(place.ZIndex(input2), 0.42857142857142D, 0.00000000000001D);
            Assert.True(place.TestInput(input2));

            //UpdateStats method
            place.UpdateStats(input2);
            Assert.AreEqual(place.Networks.Count, 4);
            Dictionary<string, GaussianNetwork> dict = place.Networks.ToDictionary(m => m.Ssid, m => m as GaussianNetwork);
            var flags = new bool[4] { false, false, false, false };
            foreach (var network in dict)
            {
                if (network.Key.Equals("ReteA"))
                {
                    flags[0] = true;
                    Assert.AreEqual(network.Value.N, 2);
                }
                else if (network.Key.Equals("ReteB"))
                {
                    flags[1] = true;
                    Assert.AreEqual(network.Value.N, 2);
                }
                else if (network.Key.Equals("ReteC"))
                {
                    flags[2] = true;
                    Assert.AreEqual(network.Value.N, 1);
                }
                else if (network.Key.Equals("ReteD"))
                {
                    flags[3] = true;
                    Assert.AreEqual(network.Value.N, 1);
                }
                else
                {
                    Assert.Fail();
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (!flags[i])
                {
                    Log.Debug(i);
                    Assert.Fail();
                }

            }

            var input3 = new List<IMeasure> { new SimpleMeasure("ReteB", 30U) };
            Assert.AreEqual(place.ZIndex(input3), 0.69D, Double.Epsilon);
            Assert.True(place.TestInput(input3));
            place.UpdateStats(input3);

            var input4 = new List<IMeasure> { new SimpleMeasure("ReteZ", 30U), new SimpleMeasure("ReteQ", 90U), new SimpleMeasure("ReteW", 80U) };
            Assert.False(place.TestInput(input4));
        }


        [Test(Description = "MultiPlacesLocation business logic test")]
        public void MultiPlacesLocationBusinessLogic()
        {
            var input1 = new List<IMeasure>
                {
                    new SimpleMeasure("ReteA", 100U),
                    new SimpleMeasure("ReteB", 100U),
                    new SimpleMeasure("ReteC", 100U)
                };
            var location = new MultiPlacesLocation("Polito", input1);

            var input2 = new List<IMeasure>
                {
                    new SimpleMeasure("ReteB", 40U),
                    new SimpleMeasure("ReteD", 5U),
                    new SimpleMeasure("ReteA", 10U)
                };
            Assert.True(location.TestInput(input2));
            location.UpdateStats(input2);

            var input3 = new List<IMeasure> { new SimpleMeasure("ReteB", 30U) };
            location.AddPlace(new ZIndexPlace(input3));
            Assert.True(location.TestInput(input3));

            var input4 = new List<IMeasure> { new SimpleMeasure("ReteZ", 30U), new SimpleMeasure("ReteQ", 90U), new SimpleMeasure("ReteW", 80U) };
            Assert.False(location.TestInput(input4));
        }

        private IList<Activity> MockActivity()
        {
            var list = new List<Activity> { new Activity("Activity1"), new Activity("Activity2") };
            return list;
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
                    return ((MultiPlacesLocation)x).Name.Equals(((MultiPlacesLocation)y).Name);
                }
                if (x is ZIndexPlace && y is ZIndexPlace)
                {
                    return ((ZIndexPlace)x).Id == ((ZIndexPlace)y).Id;
                }
                if (x is GaussianNetwork && y is GaussianNetwork)
                {
                    return ((GaussianNetwork)x).Ssid == ((GaussianNetwork)y).Ssid;
                }
                if (x is Activity && y is Activity)
                {
                    return ((Activity)x).Name.Equals(((Activity)y).Name);
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
