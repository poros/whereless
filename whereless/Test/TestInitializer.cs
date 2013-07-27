using System.IO;
using log4net;
using log4net.Config;

namespace whereless.Test
{
    using NUnit.Framework;

    // without namespace is called before each test in the assembly
    class TestInitializer
    {
        [SetUpFixture]
        public class TestsInitializer
        {
            [SetUp]
            public void InitializeLogger()
            {
                // BasicConfigurator replaced with XmlConfigurator.
                XmlConfigurator.Configure();
            }

            [TearDown]
            public void DeleteDb()
            {
                ILog Log = LogManager.GetLogger(typeof(TestsInitializer));
                if (File.Exists("Test.db"))
                {
                    File.Delete("Test.db");
                    Log.Info("Test db deleted");
                }
            }
        }
    }
}
