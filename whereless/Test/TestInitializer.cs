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
        }
    }
}
