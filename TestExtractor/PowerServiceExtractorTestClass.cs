using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Services;
using PowerTradePositionExportLibrary;

using System.IO;

namespace TestExtractor
{
    [TestClass]
    public class PowerServiceExtractorTestClass
    {
        IPowerService theService = new PowerServiceTest(); // use a fake service for unit tests

        [TestMethod]
        public void TestAggregateAndExtractToStream()
        {
            var extractor = new PowerPositionExtractor(theService);

            using (StringWriter writer = new StringWriter())
            {
                extractor.AggregatesPeriodsToStream(DateTime.Now, writer);
                System.Diagnostics.Debug.WriteLine(writer.ToString());
                Assert.IsNotNull(writer.ToString());
            }               
        }

        [TestMethod]
        public void TestAggregateValues()
        {
            var aggregatedValues = this.theService.GetAggregateTradePeriod(DateTime.Now);
            Assert.AreEqual(24, aggregatedValues.Count());
            Assert.AreEqual(150, aggregatedValues.Where(p => p.Period == 1).FirstOrDefault().Volume);
            Assert.AreEqual(150, aggregatedValues.Where(p => p.Period == 11).FirstOrDefault().Volume);
            Assert.AreEqual(80, aggregatedValues.Where(p => p.Period == 12).FirstOrDefault().Volume);
            Assert.AreEqual(80, aggregatedValues.Where(p => p.Period == 23).FirstOrDefault().Volume);
        }
    }
}
