# PowerTradePositionExtractor
Development challenge for AXPO

# PowerTradePositionReportService

This project can be used as a Console application or as a Windows Service.

## Install and run the Windows Service

To install the Windows Service, compile the solution and then run the command

```BATCH
InstallUtil PowerTradePositionReportService.exe
```

Check configuration (see below) before starting the Windows Service.

## Run in Console Mode

To run the application in console mode, Start the project with F5 in Visual Studio (it must be the default project) or type the exe name in a command prompt, once changed directory into the build output folder.
The application starts logging main events on the screen. To quit the application press the ENTER key.

## Configuration

The application config file contains settings for the extraction, as per requirements.
Extract interval is currently set every 10 seconds. Change the interval according the TimeSpan format.

```xml
    <PowerTradePositionReportService.Properties.Settings>
      <setting name="ExtractionInterval" serializeAs="String">
        <value>00:00:10</value>
      </setting>
      <setting name="CSVFilePath" serializeAs="String">
        <value>.\</value>
      </setting>
      <setting name="CSVFileNamePattern" serializeAs="String">
        <value>PowerPosition_{0:yyyyMMdd_HHmm}.csv</value>
      </setting>
      <setting name="MaxServiceCallAttempts" serializeAs="String">
        <value>3</value>
      </setting>
      <setting name="ThreadSleepMillisecondsUntilNewServiceCallAttempt"
        serializeAs="String">
        <value>1000</value>
      </setting>
    </PowerTradePositionReportService.Properties.Settings>
 ```

 The application uses log4net library to log main events and support in throubleshooting.
 Log traces are appended to different destinations. When running in Console mode, DEBUG level traces are available on the screen.
 The application also writes INFO level messages in a text file, using the log4net RollingFileAppender.

 To change log settings review the section `<log4net>` in the application config file.

 ## Project PowerTradePositionDump

 It's a Console application that dumps the power trade positions into the standard output.
 The extraction is one shot, no timer. Type `PowerTradePositionDump --help` for usage.

