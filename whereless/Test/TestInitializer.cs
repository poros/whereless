using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using log4net.Config;

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
