using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
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

    [TestFixture]
    internal class TestNHRepository
    {

        // REMARK MplZipGn class is used as dependency
        // REMARK Entities are NOT comparable. Ask implementation if needed

        // Define a static logger variable so that it references the logger instance
        private static readonly ILog Log = LogManager.GetLogger(typeof (TestNHUnitOfWork));

        private const string DbFile = "TestNHRepository.db";

        private IEntitiesFactory _entitiesFactory;

        private ISessionFactory _sessionFactory;

        private const ulong TimeVal = 9UL * WiFiSensor.ScanTime;
        private const ulong TimeVal1 = 11UL * WiFiSensor.ScanTime;
        private const ulong TimeVal2 = 2UL * WiFiSensor.ScanTime;
        private const ulong TimeVal3 = 3UL * WiFiSensor.ScanTime;

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
                                     .Conventions.Add(DefaultLazy.Never())
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

        [Test(Description = "Test operations of NHRepository")]
        public void NHRepositoryOperationsTest()
        {
            //Instantiate
            var repLoc = new NHRepository<Location>(_sessionFactory);
            
            // CREATE = Save
            // populate the database
            var input = new List<IMeasure> { new SimpleMeasure("ReteA", 10U) };
            var loc = _entitiesFactory.CreateLocation("Location1", input);
            loc.Time = TimeVal;

            var input2 = new List<IMeasure> { new SimpleMeasure("ReteB", 50U), new SimpleMeasure("ReteC", 100U) };
            var loc2 = _entitiesFactory.CreateLocation("Location2", input2);
            loc2.Time = TimeVal1; 

            //this saves everything else via cascading
            repLoc.Save(loc);
            repLoc.Save(loc2);

            // READ = Get and GetAll
            Location locA = null;
            IList<Location> locations = repLoc.GetAll();
            Assert.AreEqual(locations.Count, 2);
            foreach (var location in locations)
            {
                //For a visual feedback
                Console.WriteLine(location.ToString());
                if (location.Name == "Location1")
                {
                    Assert.AreEqual(location.Time, TimeVal);
                    locA = location;
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

            Assert.IsNotNull(locA);
            var locB = repLoc.Get(locA.Id);
            Assert.AreEqual(locB.Name, "Location1");
            Assert.AreEqual(locB.Time, TimeVal);

            var repNet = new NHRepository<Network>(_sessionFactory);
            var networks = repNet.GetAll();
            Assert.AreEqual(networks.Count, 3);

            //Dirty Get (without waiting for transactions)
            var locDirty = repLoc.Get(locA.Id, dirty: true);
            Assert.AreEqual(locDirty.Name, "Location1");
            Assert.AreEqual(locDirty.Time, TimeVal);

            var locDirties = repLoc.GetAll(dirty: true);
            Assert.AreEqual(locDirties.Count, 2);
            foreach (var location in locDirties)
            {
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


            // UPDATE = Update
            // BEWARE OF DIRTY WRITES!!! THEY ARE STILL DIFFERENT TRANSACTIONS!!!
            Location locUpdated = locA;
            locUpdated.Time = TimeVal2;
            repLoc.Update(locUpdated);

            Location tmp = repLoc.Get(locUpdated.Id);
            Assert.AreEqual(tmp.Time, TimeVal2);


            // DELETE = Delete
            locations = repLoc.GetAll();

            foreach (var location in locations)
            {
                repLoc.Delete(location);
            }
            
            //check cascading deletion
            locations = repLoc.GetAll();
            Assert.AreEqual(locations.Count, 0);

            networks = repNet.GetAll();
            Assert.AreEqual(networks.Count, 0);   
        }

        [Test(Description = "Test operations of LocationNHRepository")]
        public void LocationNHRepositoryGetByNameTest()
        {
            //Instantiate
            LocationNHRepository repLoc = new LocationNHRepository(_sessionFactory);

            // populate the database
            var input = new List<IMeasure> { new SimpleMeasure("ReteA", 10U) };
            var loc = _entitiesFactory.CreateLocation("Location1", input);
            loc.Time = TimeVal;

            var input2 = new List<IMeasure> { new SimpleMeasure("ReteB", 50U), new SimpleMeasure("ReteC", 100U) };
            var loc2 = _entitiesFactory.CreateLocation("Location2", input2);
            loc2.Time = TimeVal1;

            //this saves everything else via cascading
            repLoc.Save(loc);
            repLoc.Save(loc2);


            Location locNamed = repLoc.GetLocationByName("Location1");
            Assert.AreEqual(locNamed.Name, "Location1");
            Assert.AreEqual(locNamed.Time, TimeVal);

            Location locDirty = repLoc.GetLocationByName("Location1", dirty: true);
            Assert.AreEqual(locDirty.Name, "Location1");
            Assert.AreEqual(locDirty.Time, TimeVal);

            // Delete
            var locations = repLoc.GetAll();

            foreach (var location in locations)
            {
                repLoc.Delete(location);
            }
        }
    }
}
