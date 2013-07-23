using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whereless.NativeWiFi;

namespace whereless.Test.NativeWiFi
{
    using NUnit.Framework;

    [TestFixture(Description = "Test for Example")]
    class TestExample
    {
        [Test(Description = "NativeWiFi example test")]
        public void ExampleTest()
        {
            Example.ExampleMain();
        }
    }
}
