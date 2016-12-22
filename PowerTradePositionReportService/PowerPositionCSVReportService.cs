using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.IO;

using Services;
using PowerTradePositionExportLibrary;
using log4net;

namespace PowerTradePositionReportService
{
    public partial class PowerPositionCSVReportService : ServiceBase
    {
        private readonly IPowerService powerService;    
        private readonly PowerPositionExtractor extractor;
        private readonly ILog logger = LogManager.GetLogger(typeof(PowerPositionCSVReportService));
        private Timer timer;
        private Task worker;

        private readonly string CSV_PATH = Properties.Settings.Default.CSVFilePath;
        private readonly string CSV_FILENAME_PATTERN = Properties.Settings.Default.CSVFileNamePattern;
        private readonly int MAX_SERVICE_CALL_RETRY = Properties.Settings.Default.MaxServiceCallAttempts;
        private readonly int THREAD_SLEEP_MILLISECONDS = Properties.Settings.Default.ThreadSleepMillisecondsUntilNewServiceCallAttempt;

        public PowerPositionCSVReportService()
        {
            InitializeComponent();

            this.powerService = new PowerService();
            this.extractor = new PowerPositionExtractor(powerService);                        

        }

        internal void StartInteractiveMode(string[] args)
        {
            this.OnStart(args);
        }

        internal void StopInteractiveMode()
        {
            this.OnStop();
        }

        private void startWorker(object state)
        {
            
            if (this.worker != null && this.worker.Status == TaskStatus.Running)
            {
                logger.Warn("Another thread is still running. Waiting...");
                this.worker.Wait();
                logger.Info("Previous thread completed");
            }
                
            logger.Debug("Starting extraction task");
            this.worker = Task.Factory.StartNew(ExtractWorker);
        }

        protected override void OnStart(string[] args)
        {
            logger.InfoFormat("Starting service with following parameters:");
            logger.InfoFormat("\tCSV Location: {0}", CSV_PATH);
            logger.InfoFormat("\tCSV File name pattern: {0}", CSV_FILENAME_PATTERN);
            logger.InfoFormat("\tMax attempts to call PowerService {0}", MAX_SERVICE_CALL_RETRY);

            logger.InfoFormat("\tField separator is currently \"{0}\". Change service configuration in case is wrong.", this.extractor.FieldSeparator);


            logger.InfoFormat("\tSetting up the timer with interval {0}", Properties.Settings.Default.ExtractionInterval);                        
            timer = new Timer(startWorker, null, TimeSpan.FromSeconds(0), Properties.Settings.Default.ExtractionInterval);
            logger.Info("Service started");
        }

        protected override void OnStop()
        {
            timer.Dispose();            
            logger.Info("Service timer disposed.");

            if (this.worker.Status == TaskStatus.Running)
            {
                logger.Warn("Extraction in progress, waiting till completes");
                this.worker.Wait(); // wait for the running thread to complete
            }

            this.logger.Info("Service stopped");
        }

        void ExtractWorker()
        {
            DateTime extractionDate = DateTime.Now;
            string fileName = Path.Combine(CSV_PATH, string.Format(CSV_FILENAME_PATTERN, extractionDate));

            logger.InfoFormat("Extraction started into the file {0}", fileName);

            try
            {
                var csvFile = File.CreateText(fileName);
                logger.Debug("File created");

                try
                { 

                    PowerServiceException serviceException = null;
                    int triesCounter = MAX_SERVICE_CALL_RETRY;

                    do // loops to handle service failures
                    {
                        try
                        {
                            if (serviceException != null) // another attempt to call the service
                            {
                                logger.InfoFormat("New attempt to call PowerService.GetTrades. Sleep for {0}.", TimeSpan.FromMilliseconds(THREAD_SLEEP_MILLISECONDS));
                                System.Threading.Thread.Sleep(THREAD_SLEEP_MILLISECONDS);
                            }

                            this.extractor.AggregatesPeriodsToStream(extractionDate, csvFile);
                            serviceException = null;
                        }
                        catch (PowerServiceException ex)
                        {
                            // Exception calling PowerService
                            logger.WarnFormat("PowerService reported an error.{0}", MAX_SERVICE_CALL_RETRY > 0 ? $" The service will be called again other {MAX_SERVICE_CALL_RETRY - triesCounter} times" : "The service will be called again until a valid response returns.");
                            serviceException = ex;
                        }

                    } while (serviceException != null && --triesCounter != 0); // continue until PowerService raises exception 

                    if (serviceException != null)
                    {
                        throw serviceException;
                    }
                        
                    logger.Debug("Extractor wrote into the file stream.");
                    csvFile.Flush();
                    logger.Debug("Flushed content");

                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }
                finally
                {                    
                    csvFile.Close();
                    this.logger.Debug("File closed");
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }
    }
}
