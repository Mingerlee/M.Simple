using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M.Simple.Extensions.Core;

namespace M.Simple.Tests
{
    [TestClass]
    public class ExtensionsGuid
    {
        [TestMethod]
        public void TestGuid()
        {
            Console.WriteLine(new Guid().GetNextGuid());
            Console.WriteLine(new Guid().GetEncryptSecret());
            Console.WriteLine(new Guid().GetNoHyphenString());
        }
    }
}
