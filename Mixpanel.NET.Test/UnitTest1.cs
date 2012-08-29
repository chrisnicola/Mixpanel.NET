using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixpanel.NET.Engage;

namespace Mixpanel.NET.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var mixpanelEngage = new Engage.MixpanelEngage("fc16ce8dc1910ae4076c51a98756ecc2s", options: new EngageOptions { UseGet = false });
            var b = mixpanelEngage.Set("1", new Dictionary<string, object>()
                {
                    {
                        "Awesome", true.ToString()
                    },
                    {
                        "$email", "Stanley.Goldman+gmail@gmail.com"
                    },
                });
        }
    }
}
