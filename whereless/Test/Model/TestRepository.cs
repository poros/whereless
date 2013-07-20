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
using whereless.Model.Repository;
using whereless.NativeWiFi;

namespace whereless.Test.Model
{
    using NUnit.Framework;

    [TestFixture]
    class TestRepository
    {
        // Define a static logger variable so that it references the
        // Logger instance named "MyApp".
        private static readonly ILog Log = LogManager.GetLogger(typeof(TestRepository));

        [Test]
        public void RepoTest()
        {
            var entitiesFactory = EntitiesFactory.Factory;


            // populate the database

            var input = new List<IMeasure> { new SimpleMeasure("ReteZ", 11U) };
            var loc = entitiesFactory.CreateLocation("Location101", input);

            //this saves everything else via cascading
            var repLoc = NHModel.GetRepository<Location>();
            repLoc.Save(loc);

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
