using System.Configuration;
using System.IO;
using log4net;
using System;
using System.Collections.Generic;
using whereless.Controller.WiFi;
using whereless.Model;
using whereless.Model.Entities;
using whereless.Model.Repository;
using whereless.Model.ValueObjects;

namespace whereless.Test.Model
{
    using NUnit.Framework;

    [TestFixture(Description = "Test for ModelHelper and NHModel")]
    class TestModel
    {
        // Define a static logger variable so that it references the logger instance
        private static readonly ILog Log = LogManager.GetLogger(typeof(TestNHUnitOfWork));
        private static readonly IModel StaticModel = ModelHelper.Handle;

        private const ulong TimeVal = 9UL * WiFiSensor.ScanTime;
        private const ulong TimeVal1 = 11UL * WiFiSensor.ScanTime;
        private const ulong TimeVal2 = 2UL * WiFiSensor.ScanTime;
        private const ulong TimeVal3 = 3UL * WiFiSensor.ScanTime;
        private const ulong TimeVal4 = 4UL * WiFiSensor.ScanTime;

        [TestFixtureSetUp]
        public void CheckTestDbIsUsed()
        {
            string tmp = ConfigurationManager.AppSettings["databaseName"];
            if (tmp == null)
            {
                throw new ConfigurationErrorsException("Unable to find databaseName key");
            }
            Log.Debug("DB Name = " + tmp);
            if (tmp != "Test.db")
            {
                throw new ConfigurationErrorsException(
                    "You should run this test with Test.db application configuration");
            }
        }

        [TestFixtureTearDown]
        public void DeleteDb()
        {
            if (File.Exists("Test.db"))
            {
                File.Delete("Test.db");
                Log.Info("Test db deleted");
            }
        }

        [Test(Description = "Test ModelHelper and NHModel Instantion")]
        public void ModelInstantiationTest()
        {
            Assert.AreSame(StaticModel, ModelHelper.Handle);
            Assert.IsNotNull(StaticModel);
            Assert.IsInstanceOf<IModel>(StaticModel);
            Assert.IsInstanceOf<NHModel>(StaticModel);
        }

        [Test(Description = "Test EntitiesFactory")]
        public void EntitiesFactoryTest()
        {
            // Refer to EntitiesFactory
            Assert.AreSame(StaticModel.EntitiesFactory, ModelHelper.EntitiesFactory);
            TestEntitiesMplZipGn.AuxiliaryFactoryTest(ModelHelper.EntitiesFactory);
        }

        [Test(Description = "Test Repository")]
        public void RepositoryTest()
        {
            // Get a new Repository
            var repLoc = ModelHelper.GetRepository<Location>();
            Assert.IsNotNull(repLoc);
            Assert.IsInstanceOf<IRepository<Location>>(repLoc);

            var input = new List<IMeasure> { new SimpleMeasure("Calvino", 50U), new SimpleMeasure("Galileo", 100U) };
            var loc = ModelHelper.EntitiesFactory.CreateLocation("Polito", input);
            loc.Time = TimeVal;

            repLoc.Save(loc);

            IList<Location> locations = repLoc.GetAll();
            Assert.AreEqual(locations.Count, 1);
            foreach (var location in locations)
            {
                //For a visual feedback
                Console.WriteLine(location.ToString());
                if (location.Name == "Polito")
                {
                    Assert.AreEqual(location.Time, TimeVal);
                }
                else
                {
                    Log.Debug(location.Name);
                    Assert.Fail("Location name not matching");
                }
            }

            var repNet = ModelHelper.GetRepository<Network>();
            var networks = repNet.GetAll();
            Assert.AreEqual(networks.Count, 2);

            locations = repLoc.GetAll();
            foreach (var location in locations)
            {
                repLoc.Delete(location);
            }
        }

        [Test(Description = "Test LocationRepository")]
        public void LocationRepositoryTest()
        {
            // Get a new LocationRepository
            var repLoc = ModelHelper.GetLocationRepository();
            Assert.IsNotNull(repLoc);
            Assert.IsInstanceOf<ILocationRepository>(repLoc);

            var input = new List<IMeasure> { new SimpleMeasure("Calvino", 50U), new SimpleMeasure("Galileo", 100U) };
            var loc = ModelHelper.EntitiesFactory.CreateLocation("Polito", input);
            loc.Time = TimeVal;

            repLoc.Save(loc);

            IList<Location> locations = repLoc.GetAll();
            Assert.AreEqual(locations.Count, 1);
            foreach (var location in locations)
            {
                //For a visual feedback
                Console.WriteLine(location.ToString());
                if (location.Name == "Polito")
                {
                    Assert.AreEqual(location.Time, TimeVal);
                }
                else
                {
                    Log.Debug(location.Name);
                    Assert.Fail("Location name not matching");
                }
            }

            Location locNamed = repLoc.GetLocationByName("Polito");
            Assert.AreEqual(locNamed.Name, "Polito");
            Assert.AreEqual(locNamed.Time, TimeVal);

            Location locDirty = repLoc.GetLocationByName("Polito", dirty: true);
            Assert.AreEqual(locDirty.Name, "Polito");
            Assert.AreEqual(locDirty.Time, TimeVal);

            locations = repLoc.GetAll();
            foreach (var location in locations)
            {
                repLoc.Delete(location);
            }
        }

        [Test(Description = "Test UnitOfWork")]
        public void UnitOfWorkTest()
        {
            // Get a new UnitOfWork
            using (var uow = ModelHelper.GetUnitOfWork())
            {
                Assert.IsNotNull(uow);
                Assert.IsInstanceOf<IUnitOfWork>(uow);

                var input = new List<IMeasure> { new SimpleMeasure("ReteA", 10U) };
                var loc = ModelHelper.EntitiesFactory.CreateLocation("Location1", input);
                loc.Time = TimeVal;

                var input2 = new List<IMeasure> { new SimpleMeasure("ReteB", 50U), new SimpleMeasure("ReteC", 100U) };
                var loc2 = ModelHelper.EntitiesFactory.CreateLocation("Location2", input2);
                loc2.Time = TimeVal1;

                uow.Save(loc);
                uow.Save(loc2);

                uow.Commit();
            }

            using (var uow = ModelHelper.GetUnitOfWork())
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

            using (var uow = ModelHelper.GetUnitOfWork())
            {
                var locations = uow.GetAll<Location>();

                foreach (var location in locations)
                {
                    uow.Delete(location);
                }

                uow.Commit();
            }
        }

    }
}
