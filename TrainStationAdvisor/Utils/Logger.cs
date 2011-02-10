using System;
using System.IO;
using System.Collections.Generic;

namespace TrainStationAdvisor.ClassLibrary
{
    /// <summary>
    /// Create a LogTransaction within a using statement to suppress timestamps.
    /// </summary>
    public class LogTransaction : IDisposable
    {
        public Logger myLogger;
        internal LogTransaction(Logger logger)
        {
            myLogger = logger;
            myLogger.myTransactions.Add(this);
        }

        #region IDisposable Members

        public void Dispose()
        {
            myLogger.myTransactions.Remove(this);
        }

        #endregion
    }

    /// <summary>
    /// This enumeration specifies the log level that the log entry pertains to.
    /// </summary>
    public enum LogLevel
    {
        Error = 0,
        Information = 1,
        Verbose = 2,
        Debug = 3
    }

    /// <summary>
    /// A generic logging class that provides useful features such as log level, log file length notification,
    /// and application wide debug level logging.
    /// </summary>
    public class Logger
    {
        StreamWriter myStream;
        FileStream myFileStream;
        /// <summary>
        /// Set this instance to specify the log to which debug output will be written to.
        /// </summary>
        public static Logger DefaultLogger { get; set; }

        /// <summary>
        /// Log an exception in the debug log.
        /// </summary>
        /// <param name="e">The exception to log.</param>
        /// <param name="entryFormat">The message format.</param>
        /// <param name="entryArgs">The message format arguments</param>
        public static void DebugLogException(Exception e, string entryFormat, params object[] entryArgs)
        {
            if (DefaultLogger != null)
                DefaultLogger.LogException(e, entryFormat, entryArgs);
        }

        /// <summary>
        /// Log a message to the debug log.
        /// </summary>
        /// <param name="entryFormat">The message format.</param>
        /// <param name="entryArgs">The message format arguments</param>
        public static void DebugLog(string entryFormat, params object[] entryArgs)
        {
            if (DefaultLogger != null)
                DefaultLogger.Log(LogLevel.Debug, entryFormat, entryArgs);
            else
            {
                entryFormat = string.Format(entryFormat, entryArgs);
                System.Diagnostics.Debug.WriteLine(entryFormat);
            }
        }

        int myNotifyLength = 100000;
        /// <summary>
        /// Set this property to specify the length at which the the LogFileNotifyLength will fire.
        /// </summary>
        public int NotifyLength
        {
            get { return myNotifyLength; }
            set { myNotifyLength = value; }
        }

        /// <summary>
        /// Log a message at LogLevel.Error
        /// </summary>
        /// <param name="entry">The message format</param>
        /// <param name="args">The message format arguments</param>
        public void Log(string entry, params object[] args)
        {
            Log(LogLevel.Error, entry, args);
        }

        /// <summary>
        /// Log an exception.
        /// </summary>
        /// <param name="logLevel">The LogLevel of this exception</param>
        /// <param name="e">The exception to log</param>
        /// <param name="exceptionFormat">The message format</param>
        /// <param name="exceptionArgs">The message format arguments</param>
        public void LogException(LogLevel logLevel, Exception e, string exceptionFormat, params object[] exceptionArgs)
        {
            using (LogTransaction t = NewTransaction(exceptionFormat, exceptionArgs))
            {
                while (e != null)
                {
                    Log(logLevel, e.GetType().Name);
                    Log(logLevel, e.Message);
                    Log(logLevel, e.StackTrace);
                    e = e.InnerException;
                }
            }
        }

        /// <summary>
        /// Log an exception at LogLevel.Error
        /// </summary>
        /// <param name="e">The exception to log</param>
        /// <param name="exceptionFormat">The message format</param>
        /// <param name="exceptionArgs">The message format arguments</param>
        public void LogException(Exception e, string exceptionFormat, params object[] exceptionArgs)
        {
            LogException(LogLevel.Error, e, exceptionFormat, exceptionArgs);
        }

        /// <summary>
        /// Log a message
        /// </summary>
        /// <param name="logLevel">The LogLevel of this message</param>
        /// <param name="entryFormat">The message format</param>
        /// <param name="entryArgs">The message arguments</param>
        public void Log(LogLevel logLevel, string entryFormat, params object[] entryArgs)
        {
            entryFormat = string.Format(entryFormat, entryArgs);
            System.Diagnostics.Debug.WriteLine(entryFormat);

            if ((int)logLevel > (int)LogLevel)
                return;

            if (myTransactions.Count == 0)
            {
                try
                {
                    FileInfo fi = new FileInfo(LogFilePath);
                    if (fi.Length > myNotifyLength)
                        OnLogfileNotifyLength();
                }
                catch (Exception)
                {
                }
            }
            else if (Timestamp)
            {
                entryFormat = string.Format("\t\t\t{0}", entryFormat).Replace("\n", "\n\t\t\t");
            }

            // we don't actually create a log file until we attempt to log something
            try
            {
                if (myStream == null)
                    OpenLogFile();
            }
            catch (Exception)
            {
                return;
            }

            if (Timestamp && myTransactions.Count == 0)
                entryFormat = string.Format("{0}:\t{1}", DateTime.Now, entryFormat);
            myStream.WriteLine(entryFormat);
            myStream.Flush();
            myFileStream.Flush();
        }

        void OpenLogFile()
        {
            myStream = new StreamWriter(myFileStream = new FileStream(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read));
            myStream.AutoFlush = true;
        }

        string myLogFilePath = Path.Combine(EnvironmentEx.CallingAssemblyDirectory, "Logger.log");
        /// <summary>
        /// Get or Set the Log File Path
        /// </summary>
        public string LogFilePath
        {
            get { return myLogFilePath; }
            set
            {
                if (myLogFilePath != value)
                {
                    myLogFilePath = value;
                    Close();
                }
            }
        }

        void Close()
        {
            if (myStream != null)
            {
                myStream.Close();
                myStream = null;
            }
        }

        LogFileNotifyLengthEventHandler myLogFileNotifyLength;
        /// <summary>
        /// This event fires whenever the Log File reaches the length specified by NotifyLength
        /// </summary>
        public event LogFileNotifyLengthEventHandler LogFileNotifyLength
        {
            add
            {
                myLogFileNotifyLength += value;
            }
            remove
            {
                myLogFileNotifyLength -= value;
            }
        }
        
        void OnLogfileNotifyLength()
        {
            if (myLogFileNotifyLength != null)
                myLogFileNotifyLength();
        }

        LogLevel myLogLevel = LogLevel.Debug;
        /// <summary>
        /// This specifies the maximum LogLevel of the messages that will actually be written to the log.
        /// </summary>
        public LogLevel LogLevel
        {
            get { return myLogLevel; }
            set { myLogLevel = value; }
        }

        bool myTimestamp = true;
        /// <summary>
        /// Get or Set this property to enable/disable time stamps.
        /// </summary>
        public bool Timestamp
        {
            get { return myTimestamp; }
            set { myTimestamp = value; }
        }

        internal List<LogTransaction> myTransactions = new List<LogTransaction>();

        /// <summary>
        /// Create a LogTransaction and use it in the context of a using statement to create a labelled log entry
        /// that encapsulates an entire log event.
        /// While this LogTransaction is active (not disposed), time stamps will be disabled.
        /// </summary>
        /// <param name="transactionNameStringFormat">The log transaction message</param>
        /// <param name="transactionNameArgs">The log transaction message arguments</param>
        /// <returns></returns>
        public LogTransaction NewTransaction(string transactionNameStringFormat, params object[] transactionNameArgs)
        {
            Log(LogLevel.Error, transactionNameStringFormat, transactionNameArgs);
            return new LogTransaction(this);
        }
    }

    /// <summary>
    /// The delegate that will fire when the Logger reaches the length specified in NotifyLength
    /// </summary>
    public delegate void LogFileNotifyLengthEventHandler();
}