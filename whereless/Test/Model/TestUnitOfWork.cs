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
    class TestUnitOfWork
    {
        // Define a static logger variable so that it references the
        // Logger instance named "MyApp".
        private static readonly ILog Log = LogManager.GetLogger(typeof(TestUnitOfWork));

        [Test]
        public void UowTest()
        {
            var entitiesFactory = NHModel.EntitiesFactory;

            using (var uow = NHModel.GetUnitOfWork())
            {
                // populate the database

                var input = new List<IMeasure> { new SimpleMeasure("ReteD", 10U) };
                var loc = entitiesFactory.CreateLocation("Location3", input);

                //this saves everything else via cascading
                uow.Add(loc);

                //uow.Rollback();
                uow.Commit();
            }

            using (var uow = NHModel.GetUnitOfWork())
            {
                var loc = uow.GetLocationByName("Location3");
                Console.WriteLine(loc.ToString());
                uow.Commit();
            }

            //using (var uow = NHModel.GetUnitOfWork())
            //    {
            //        var locations = session.CreateCriteria(entitiesFactory.LocationType)
            //                               .List<Location>();

            //        foreach (var location in locations)
            //        {
            //            Console.WriteLine(location.ToString());
            //            session.Delete(location);
            //        }

            //        transaction.Commit();
            //    }
            //}

            //using (var uow = NHModel.GetUnitOfWork())
            //{

            //    var networks = session.CreateCriteria(entitiesFactory.NetworkType)
            //            .List<Network>();

            //    foreach (var network in networks)
            //    {
            //        Console.WriteLine(network.ToString());
            //    }
            //}
        }
    }
}
