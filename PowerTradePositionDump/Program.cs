using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Services;
using PowerTradePositionExportLibrary;

namespace PowerTradePositionDump
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var options = new AppOptions();
                if (!CommandLine.Parser.Default.ParseArguments(args, options))
                    throw new CommandLine.ParserException("Invalid arguments");

                if (options.HelpRequired)
                {
                    PrintUsage();
                }
                else
                {
                    var extractor = new PowerPositionExtractor(new PowerService());
                    extractor.FieldSeparator = options.FieldSeparator;

                    extractor.AggregatesPeriodsToStream(options.ExtractDate, Console.Out);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                PrintUsage();
            }
        }

        static void PrintUsage()
        {
            var exeName = Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            Console.WriteLine("DESCRIPTION:\n\tExtracts aggregated trade positions and prints them to the standard output.");
            Console.WriteLine($"USAGE:\n\t{exeName} [-d | --date date] [-s | --separator sep]");
            Console.WriteLine(@"
    -d | --date date                Extraction date. Default is today.
    -s | --separatore separator     Character to use as field separator.

EXAMPLE:
    {0} -d 2016-12-22 -s "";""
",exeName );
        }
    }
}
