using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Services;
using log4net;

namespace PowerTradePositionExportLibrary
{
    public class PowerPositionExtractor
    {
        private readonly IPowerService powerService;
        private string fieldSeparator = !string.IsNullOrEmpty(Properties.Settings.Default.FieldSeparator) ? Properties.Settings.Default.FieldSeparator : System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        private readonly ILog logger = log4net.LogManager.GetLogger(typeof(PowerPositionExtractor));
        public string FieldSeparator
        {
            get { return fieldSeparator; }
            set { fieldSeparator = value; }
        }

        public PowerPositionExtractor(IPowerService powerService)
        {
            this.powerService = powerService;
        }

        public void AggregatesPeriodsToStream(DateTime date, TextWriter output)
        {
            this.logger.DebugFormat("Extraction started for date {0:yyyyMMdd HH:mm:ss}", date);
            var periods = this.powerService.GetAggregateTradePeriod(date);
            this.writeToStream(output, periods);
            this.logger.InfoFormat("Flushed stream with aggregated PowerPeriod for the date {0:yyyyMMdd HH:mm.ss}", date);
        }

        private void writeToStream(TextWriter output, IEnumerable<PowerPeriod> periods)
        {
            if (Properties.Settings.Default.WriteHeader)
            {
                this.logger.Debug("Writing header");
                output.WriteLine($"Local Time{this.fieldSeparator}Volume");
            }

            foreach (var item in periods)
            {                
                output.WriteLine(this.writePowerPeriodLine(item));
            }

            output.Flush();
            this.logger.Info("PowerPeriod array flushed into the stream.");
        }

        /// <summary>
        /// Writes a single line for a csv file according the current field separator
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string writePowerPeriodLine(PowerPeriod line)
        {
            // lambda expression to convert period number to hour (1 = 23, 2 = 0, 3 = 1)
            Func<PowerPeriod, int> periodToHourConverter = (item) => item.Period == 1 ? 23 : item.Period - 2;

            bool encloseFigures = this.fieldSeparator == System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator; // if field separator and decimal separator match, figures are enclosed in double quotas
            string volumeField = encloseFigures ? $"\"{line.Volume}\"" : line.Volume.ToString(); // writes the volume field according field and decimal separator

            var retString = $"{periodToHourConverter(line):00}:00{this.fieldSeparator}{volumeField}";

            this.logger.Debug(retString);

            return retString;
        }


    }
}
