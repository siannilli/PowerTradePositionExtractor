using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;
using CommandLine.Text;

namespace PowerTradePositionDump
{
    public class AppOptions
    {
        public AppOptions()
        {
            this.ExtractDate = DateTime.Now;
        }
        [Option('d', "date") ]
        public DateTime ExtractDate { get; set; }

        [Option('s', "separator", DefaultValue = "\t", HelpText = "Field separator (default is the tab char)")]
        public string FieldSeparator { get; set; }

        [Option('?', "help")]
        public bool HelpRequired { get; set; }
    }
}
