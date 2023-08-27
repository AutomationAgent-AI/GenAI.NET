using System;
using System.IO;
using System.Text;
using System.Timers;

namespace Automation.GenerativeAI.Utilities
{
    /// <summary>
    /// Level of logging
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Informative Log
        /// </summary>
        Info,

        /// <summary>
        /// Warning Log
        /// </summary>
        Warning,

        /// <summary>
        /// Error Log
        /// </summary>
        Error,

        /// <summary>
        /// Stack Trace Log
        /// </summary>
        StackTrace,

        /// <summary>
        /// Audit Log
        /// </summary>
        Audit
    }

    /// <summary>
    /// Different operations type for which log information can be written
    /// </summary>
    public enum LogOps
    {
        /// <summary>
        /// Log when result not found
        /// </summary>
        NotFound,

        /// <summary>
        /// Log when result is found
        /// </summary>
        Found,

        /// <summary>
        /// Log when result is classified to a type/class.
        /// </summary>
        Classified,

        /// <summary>
        /// Log when text is extracted
        /// </summary>
        TextExtracted,

        /// <summary>
        /// Log when exception happend
        /// </summary>
        Exception,

        /// <summary>
        /// Log while running a command
        /// </summary>
        Command,

        /// <summary>
        /// Log while updating settings
        /// </summary>
        Settings,

        /// <summary>
        /// Log information related to result
        /// </summary>
        Result,

        /// <summary>
        /// Log information related to a request
        /// </summary>
        Request,

        /// <summary>
        /// Log information related to a response
        /// </summary>
        Response,

        /// <summary>
        /// Log information while running a test
        /// </summary>
        Test
    }

    interface ILogger
    {
        LogLevel LogLevel { get; set; }
        void WriteLine(LogLevel mode, LogOps ops, string log);
    }

    abstract class BaseLogger : ILogger
    {
        public BaseLogger() { LogLevel = LogLevel.Error; }

        public LogLevel LogLevel { get; set; }

        public void WriteLine(LogLevel level, LogOps ops, string log)
        {
            var timestamp = DateTime.Now;
            if (level >= LogLevel)
                WriteLog(string.Format("{0}, [{1} {2}], {3}, {4}", level.ToString(), timestamp.ToShortTimeString(), timestamp.ToShortDateString(), ops.ToString(), log));
        }

        protected abstract void WriteLog(string log);
    }

    class FileLogger : BaseLogger
    {
        StringBuilder InMemoryLog = new StringBuilder();
        Timer Timer;
        object syncroot = new object();

        public FileLogger(string logFile) 
        { 
            LogFile = logFile;
            Timer = new Timer(2000);
            Timer.Elapsed += OnTimer;
            Timer.Enabled = true;
            Timer.AutoReset = true;
        }

        public string LogFile { get; private set; }

        protected override void WriteLog(string log)
        {
            lock(syncroot)
            {
                InMemoryLog.AppendLine(log);
            }
        }

        void OnTimer(Object source, ElapsedEventArgs e)
        {
            lock (syncroot)
            {
                File.AppendAllText(LogFile, InMemoryLog.ToString());
                InMemoryLog.Clear();
            }
        }
    }

    class ConsoleLogger : BaseLogger
    {
        public ConsoleLogger()
        {
        }

        protected override void WriteLog(string log)
        {
            System.Console.WriteLine(log);
        }
    }

    class TraceLogger : BaseLogger
    {
        public TraceLogger() { }

        protected override void WriteLog(string log)
        {
            System.Diagnostics.Trace.WriteLine(log);
        }
    }

    /// <summary>
    /// A simple logger class that helps write log information
    /// </summary>
    public class Logger
    {
        private static ILogger logger = new TraceLogger();
        private static string LogFile { get; set; }

        /// <summary>
        /// Gets the log file for specific operation
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static string GetLogFile(string operation)
        {
            var folder = string.IsNullOrEmpty(LogFile) ? "" : Path.GetDirectoryName(LogFile);
            var name = DateTime.Now.ToString("yyyyMMddHHmmss");
            return Path.Combine(folder, string.Format("{0}.{1}.log", operation, name));
        }

        /// <summary>
        /// Sets the log file and the log level
        /// </summary>
        /// <param name="logfile">Full path of the log file</param>
        /// <param name="logLevel">Log information level</param>
        public static void SetLogFile(string logfile, LogLevel logLevel = LogLevel.Info)
        {
            LogFile = logfile;
            if (!string.IsNullOrEmpty(logfile))
                logger = new FileLogger(logfile) { LogLevel = logLevel };
            else
                logger = new TraceLogger();
        }

        /// <summary>
        /// Write the log information
        /// </summary>
        /// <param name="level">Log level</param>
        /// <param name="ops">Log operation</param>
        /// <param name="log">Log information</param>
        public static void WriteLog(LogLevel level, LogOps ops, string log)
        {
            logger.WriteLine(level, ops, log);
        }
    }
}
