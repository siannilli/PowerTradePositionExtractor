using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Services;

namespace TestExtractor
{
    public class PowerServiceTest : IPowerService
    {
        public IEnumerable<PowerTrade> GetTrades(DateTime date)
        {
            // Build the same result as Requirements
            PowerTrade trade1 = PowerTrade.Create(date, 24);
            PowerTrade trade2 = PowerTrade.Create(date, 24);

            for (int i = 0; i < trade1.Periods.Length; i++)
            {
                trade1.Periods[i].Volume = 100;
                trade2.Periods[i].Volume = 50;
            }
            
            for (int i= 11; i< trade2.Periods.Length; i++)
            {
                trade2.Periods[i].Volume = -20;
            }

            return new List<PowerTrade> { trade1, trade2 };
        }

        public Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date)
        {
            return new Task<IEnumerable<PowerTrade>>(() => this.GetTrades(date));
        }
    }
}
