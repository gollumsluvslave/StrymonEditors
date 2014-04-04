using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;


namespace RITS.StrymonEditor.Logging
{
    /// <summary>
    /// Static logging class.
    /// </summary>
    public static class StaticLogger
    {

        #region public methods
        /// <summary>
        /// Initialises this instance.
        /// </summary>
        internal static void Listen()
        {
            if (!IsListening)
            {
                Trace.Listeners.Add(StaticLogger.listener);
                
            }
        }
        private static bool IsListening
        {
            get
            {
                return Trace.Listeners.Contains(StaticLogger.listener);
            }
        }

        /// <summary>
        /// A method has been entered.
        /// </summary>
        public static void Entered()
        {
            if (IsValidLogLevel(LogLevel.Debug))
            {
                Listen();
                WriteLog("ENTERED", String.Empty);
            }
        }

        /// <summary>
        /// A method has completed.
        /// </summary>
        public static void Completed()
        {
            if(IsValidLogLevel(LogLevel.Debug))
            {
                Listen();
                WriteLog("COMPLETED", String.Empty);
            }
        }


        /// <summary>
        /// Output a debug message.
        /// </summary>
        /// <param name="message">The message to output.</param>
        public static void Debug(string message)
        {
            if (IsValidLogLevel(LogLevel.Debug))
            {
                Listen();
                WriteLog("DEBUG", message);
            }
        }

        /// <summary>
        /// Output an information message.
        /// </summary>
        /// <param name="message">The message to output.</param>
        public static void Info(string message)
        {
            if (IsValidLogLevel(LogLevel.Info))
            {
                Listen();
                WriteLog("INFO", message);
            }
        }

        /// <summary>
        /// Output a warning message.
        /// </summary>
        /// <param name="message">The message to output.</param>
        public static void Warn(string message)
        {
            if (IsValidLogLevel(LogLevel.Warn))
            {
                Listen();
                WriteLog("WARN", message);
            }
        }

        /// <summary>
        /// Output an error message.
        /// </summary>
        /// <param name="message">The message to output.</param>
        public static void Error(string message)
        {
            if (IsValidLogLevel(LogLevel.Error))
            {
                Listen();
                WriteLog("ERROR", message);
            }
        }

        /// <summary>
        /// Output an exception as an error message.
        /// </summary>
        /// <param name="ex">The exception to output.</param>
        public static void Error(Exception ex)
        {
            if (IsValidLogLevel(LogLevel.Debug))
            {
                Listen();
                WriteLog("ERROR", ex.ToString());
            }
            else
                Error(ex.Message);
        }

        /// <summary>
        /// Output an exception as an error message.
        /// </summary>
        /// <param name="ex">The message to output.</param>
        /// <param name="ex">The exception to output.</param>
        public static void Error(string message, Exception ex)
        {
            if (IsValidLogLevel(LogLevel.Debug))
            {
                Listen();
                WriteLog("ERROR", ex.Message);
            }
            else
                Error(message + " - " + ex.Message);
        } 
        #endregion

        #region internal methods
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        internal static void Dispose()
        {
            if (StaticLogger.listener != null)
            {
                Trace.Listeners.Remove(StaticLogger.listener);
            }
        } 
        #endregion

        #region private methods

        private static TraceListener GetListener()
        {
            // try to use a file-based listener - if anything goes wrong use Console instead
            RollingTextListener fileListener = new RollingTextListener();
            if (fileListener.IsValid())
            {
                return fileListener;
            }
            else
            {
                return new System.Diagnostics.ConsoleTraceListener();
            }
        }
        /// <summary>
        /// Determines if the configured LogLevel is valid
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static bool IsValidLogLevel(LogLevel level)
        {
            return true;
        }

        /// <summary>
        /// Writes the log.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <param name="message">The message.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Logger should not crash program")]
        private static void WriteLog(string severity, string message)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(DateTime.Now.ToString("d/M/yyyy H:mm:ss:fff", CultureInfo.CurrentCulture));
            builder.Append(" ");
            builder.Append(System.Threading.Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture));
            builder.Append(" ");
            builder.Append(GetExecutingMethodName());
            builder.Append(" ");
            builder.Append(severity);
            builder.Append(": ");
            builder.Append(message);

            try
            {
                Trace.WriteLine(builder.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        /// <summary>
        /// Gets the name of the executing method where the log was called from
        /// </summary>
        /// <returns>The name of the executing method.</returns>
        private static string GetExecutingMethodName()
        {
            // init return & stacks
            string result = "Unknown";
            
            // Walk the stack trace to find caller - RW - fix to remove hardcoded stackframe numbers which cause issues with Release builds.
            StackTrace stackTrace = new StackTrace();
            MethodBase method = null;
            foreach (StackFrame stackFrame in stackTrace.GetFrames())
            {
                method = stackFrame.GetMethod();
                if (!method.DeclaringType.FullName.StartsWith("RITS.StrymonEditor.Logging"))
                {
                    break;
                }
            }                        

            if (method != null)
            {
                if (method.DeclaringType != null)
                {
                    result = string.Concat(method.DeclaringType.FullName, ".", method.Name);
                }
                else
                {
                    result = method.Name;
                }
            }

            return result;
        } 
        #endregion

        #region private fields
        /// <summary>
        /// The trace-listener
        /// </summary>
        private static TraceListener listener = GetListener();
        private static TraceSwitch traceSwitch = new TraceSwitch("MC.Core.Logging", "Default Logger Switch");


        #endregion
    }
}
