using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace PowerTradePositionReportService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            // set current directory to the base dir, in order to use relative paths in app.config
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
            ILog logger = LogManager.GetLogger(typeof(PowerPositionCSVReportService));

            PowerPositionCSVReportService extractorService = new PowerPositionCSVReportService();            

            if (!Environment.UserInteractive)
            {

                logger.Info("Starting service in Windows Service mode");
                ServiceBase.Run(extractorService); // run as windows service
            }
            else // console mode
            {
                logger.Info("Starting service in Interactive mode");                
                extractorService.StartInteractiveMode(args);
                Console.WriteLine("Hit ENTER to stop the program");
                Console.ReadLine();
                logger.Info("Stopping service due to user request");
                extractorService.StopInteractiveMode();

            }
        }
    }
}
