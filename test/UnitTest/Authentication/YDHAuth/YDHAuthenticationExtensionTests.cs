using Microsoft.VisualStudio.TestTools.UnitTesting;
using YDH.Finace.Web.Authentication.YDHAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YDH.Finace.Web.Authentication.YDHAuth.Tests
{
    [TestClass()]
    public class YDHAuthenticationExtensionTests
    {
        [TestMethod()]
        public void TokenEncryptionTest()
        {
            string ss = "YDH.Port.FivePlaneSweep-Test-2021".TokenEncryption();
            Assert.Fail();
        }
    }
}