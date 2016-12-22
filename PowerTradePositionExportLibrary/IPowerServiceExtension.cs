using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Services;
using log4net;

namespace PowerTradePositionExportLibrary
{
    public static class IPowerServiceExtension
    {
        private static ILog logger = LogManager.GetLogger(typeof(IPowerServiceExtension));

        /// <summary>
        /// Extract trades from the PowerService and aggregates them by Period
        /// </summary>
        /// <param name="powerService"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static IEnumerable<PowerPeriod> GetAggregateTradePeriod(this IPowerService powerService, DateTime date)
        {
            var trades = powerService.GetTrades(date);
            logger.DebugFormat("Extracted {0} trades for date {1:yyyyMMdd HH:nn.ss}", trades.Count(), date);

            var distinctPeriods = trades.SelectMany(pt => pt.Periods.Select(p => p));

            return distinctPeriods.GroupBy(pp => pp.Period).Select(p => new PowerPeriod() { Period = p.Key, Volume = p.Sum(v => v.Volume) });            
        }
    }
}
