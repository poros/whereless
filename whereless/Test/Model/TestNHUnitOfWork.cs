using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using log4net;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.IO;
using whereless.Controller.WiFi;
using whereless.Model.Entities;
using whereless.Model.Factory;
using whereless.Model.Repository;
using whereless.Model.ValueObjects;

namespace whereless.Test.Model
{
    using NUnit.Framework;

    // REMARK MplZipGn class is used as dependency
    // REMARK Entities are NOT comparable. Ask implementation if needed

    [TestFixture(Description = "Test for NHUnitOfWork")]
    class TestNHUnitOfWork
    {
        // Define a static logger variable so that it references the logger instance
        private static readonly ILog Log = LogManager.GetLogger(typeof(TestNHUnitOfWork));

        private const string DbFile = "TestNHUnitOfWork.db";

        private IEntitiesFactory _entitiesFactory;

        private ISessionFactory _sessionFactory;

        private const ulong TimeVal = 9UL * WiFiSensor.ScanTime;
        private const ulong TimeVal1 = 11UL * WiFiSensor.ScanTime;
        private const ulong TimeVal2 = 2UL * WiFiSensor.ScanTime;
        private const ulong TimeVal3 = 3UL * WiFiSensor.ScanTime;
        private const ulong TimeVal4 = 4UL * WiFiSensor.ScanTime;

        [TestFixtureSetUp]
        public void CreateDb()
        {
            _sessionFactory = CreateSessionFactory();
            _entitiesFactory = new MplZipGn();
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

        [Test(Description = "Test to be sure NHUnitofWork has been disposed invoking commit")]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void NHUnitOfWorkDisposeTransactionTest()
        {
            var uow = new NHUnitOfWork(_sessionFactory);
            uow.Dispose();  
            uow.Commit();
        }

        [Test(Description = "Test to be sure NHUnitofWork has been disposed invoking add")]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void NHUnitOfWorkDisposeSessionTest()
        {
            var input = new List<IMeasure> { new SimpleMeasure("ReteA", 10U) };
            var loc = _entitiesFactory.CreateLocation("Location1", input);
            var uow = new NHUnitOfWork(_sessionFactory);
            uow.Dispose();
            uow.Save(loc);
        }

        [Test(Description = "Test operations of NHUnitOfWork")]
        public void NHUnitOfWorkOperationsTest()
        {
            //CREATE = SAVE AND COMMIT
            using (var uow = new NHUnitOfWork(_sessionFactory))
            {
                // populate the database
                var input = new List<IMeasure> { new SimpleMeasure("ReteA", 10U) };
                var loc = _entitiesFactory.CreateLocation("Location1", input);
                loc.Time = TimeVal;  
  
                var input2 = new List<IMeasure> { new SimpleMeasure("ReteB", 50U), new SimpleMeasure("ReteC", 100U) };
                var loc2 = _entitiesFactory.CreateLocation("Location2", input2);
                loc2.Time = TimeVal1; 

                //this saves everything else via cascading
                uow.Save(loc);
                uow.Save(loc2);

                uow.Commit();
            }

            // READ =
            // Get, GetLocationByName and GetAll
            using (var uow = new NHUnitOfWork(_sessionFactory))
            {
                var locA = uow.GetLocationByName("Location1");
                Assert.AreEqual(locA.Name, "Location1");
                Assert.AreEqual(locA.Time, TimeVal);

                var locB = uow.Get<Location>(locA.Id);
                Assert.AreEqual(locB.Name, "Location1");
                Assert.AreEqual(locB.Time, TimeVal);

                IList<Location> locations = uow.GetAll<Location>();
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

                var networks = uow.GetAll<Network>();
                Assert.AreEqual(networks.Count, 3);

                uow.Commit();
            }

            Location longLivedLocation;
            const string locName = "Location1";

            // UPDATE = SAVE AND COMMIT
            // update within a single unit of work
            using (var uow = new NHUnitOfWork(_sessionFactory))
            {
                longLivedLocation = uow.GetLocationByName(locName);
                longLivedLocation.Time = TimeVal2;
                uow.Commit();
            }

            using (var uow = new NHUnitOfWork(_sessionFactory))
            {
                var locB = uow.Get<Location>(longLivedLocation.Id);
                Assert.AreEqual(locB.Time, TimeVal2);
            }

            // update of an entity retrieved in a unit of work
            // BEWARE OF DIRTY WRITES!!! THEY ARE STILL DIFFERENT TRANSACTIONS!!!
            using (var uow = new NHUnitOfWork(_sessionFactory))
            {
                longLivedLocation.Time = TimeVal3;
                uow.Save(longLivedLocation);
                uow.Commit();
            }

            using (var uow = new NHUnitOfWork(_sessionFactory))
            {
                var tmp = uow.GetLocationByName(locName);
                Assert.AreEqual(tmp.Time, TimeVal3);
                uow.Commit();
            }
                    

            // ROLLBACK
            using (var uow = new NHUnitOfWork(_sessionFactory))
            {
                longLivedLocation = uow.GetLocationByName("Location1");
                longLivedLocation.Time = TimeVal4;

                var input = new List<IMeasure> { new SimpleMeasure("ReteC", 10U) };
                var loc = _entitiesFactory.CreateLocation("Location3", input);
                loc.Time = TimeVal;
                uow.Save(loc);

                uow.Rollback();
            }

            using (var uow = new NHUnitOfWork(_sessionFactory))
            {
                var locB = uow.Get<Location>(longLivedLocation.Id);
                //old value
                Assert.AreEqual(locB.Time, TimeVal3);

                var locations = uow.GetAll<Location>();
                Assert.AreEqual(locations.Count, 2);
                foreach (var location in locations)
                {
                    if(location.Name!="Location1" && location.Name!="Location2")
                    {
                        Log.Debug(location.Name);
                        Assert.Fail("Location added in rollbacked unit of work");
                    }
                }
            }


            // LAZY LOADING
            // It seems it does not work (always eager loading). Less performances, zero problems... 
            // Check it by hand, looking at logs
            Log.Debug("LAZY LOADING");
            Location locLazy;
            using (var uow = new NHUnitOfWork(_sessionFactory))
            {
                locLazy = uow.GetLocationByName("Location1");
            }
            Assert.AreEqual(locLazy.Name, "Location1");
            Assert.AreEqual(locLazy.Time, TimeVal3);

            var up = new List<IMeasure> { new SimpleMeasure("ReteA", 20U) };
            locLazy.UpdateStats(up);
            Log.Debug(locLazy.ToString());

            IList<Location> locLazies;
            using (var uow = new NHUnitOfWork(_sessionFactory))
            {
                locLazies = uow.GetAll<Location>();
            }

            up = new List<IMeasure> { new SimpleMeasure("ReteA", 10U), new SimpleMeasure("ReteB", 40U) };
            locLazies[1].UpdateStats(up);
            Log.Debug(locLazies[1].ToString());

            // DELETE
            using (var uow = new NHUnitOfWork(_sessionFactory))
            {
                var locations = uow.GetAll<Location>();

                    foreach (var location in locations)
                    {
                        uow.Delete(location);
                    }

                    uow.Commit();
            }

            // check deletion cascade
            using (var uow = new NHUnitOfWork(_sessionFactory))
            {
                var locations = uow.GetAll<Location>();
                Assert.AreEqual(locations.Count, 0);

                var networks = uow.GetAll<Network>();
                Assert.AreEqual(networks.Count, 0);
            }
        }
    }
}
