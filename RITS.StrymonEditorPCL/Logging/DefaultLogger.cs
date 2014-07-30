using System;
using System.Diagnostics;
using System.IO;
using System.Security;
namespace RITS.StrymonEditor.Logging
{
    /// <summary>
    /// Instance wrapper around <see cref="StaticLogger"/>.
    /// </summary>
    public sealed class DefaultLogger : ILogger
    {
        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultLogger"/> class.
        /// NB - This is simply a facade implementation for the <see cref="StaticLogger"/> class and should not be used by external libraries
        /// Either use StaticLogger directly for RnD work, or ideally use the <see cref="RITSLogger"/> which allows different implementations of <see cref="ILogger"/> to be switched in and out
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Logger should not crash program")]
        public DefaultLogger()
        {
            try
            {
                StaticLogger.Listen();
            }
            catch (Exception ex)
            {
                this.isloggingConfigured = false;
                string errorMessage = "Error configuring logging framework" + Environment.NewLine + ex;

                try
                {

                    if (!EventLog.SourceExists("RITS.StrymonEditor.Logging"))
                        EventLog.CreateEventSource("RITS.StrymonEditor.Logging", "Application");

                    EventLog.WriteEntry("RITS.StrymonEditor.Logging", errorMessage);

                    string warningMessage = "Error configuring logging framework - check message logged to machine EventLog";
                    Console.WriteLine(warningMessage);
                    Trace.WriteLine(warningMessage);
                }
                catch (SecurityException)
                {
                    Console.WriteLine(errorMessage);
                    Trace.WriteLine(errorMessage);
                }
            }
        } 
        #endregion

        #region ILogger implementation
        /// <summary>
        /// A method has been entered.
        /// </summary>
        public void Entered()
        {
            if (this.isloggingConfigured)
            {
                StaticLogger.Entered();
            }
        }

        /// <summary>
        /// A method has completed.
        /// </summary>
        public void Completed()
        {
            if (this.isloggingConfigured)
            {
                StaticLogger.Completed();
            }
        }

        /// <summary>
        /// Output a debug message.
        /// </summary>
        /// <param name="message">The message to output.</param>
        public void Debug(string message)
        {
            if (this.isloggingConfigured)
            {
                StaticLogger.Debug(message);
            }
        }

        /// <summary>
        /// Output an information message.
        /// </summary>
        /// <param name="message">The message to output.</param>
        public void Info(string message)
        {
            if (this.isloggingConfigured)
            {
                StaticLogger.Info(message);
            }
        }

        /// <summary>
        /// Output a warning message.
        /// </summary>
        /// <param name="message">The message to output.</param>
        public void Warn(string message)
        {
            if (this.isloggingConfigured)
            {
                StaticLogger.Warn(message);
            }
        }

        /// <summary>
        /// Output an error message.
        /// </summary>
        /// <param name="message">The message to output.</param>
        public void Error(string message)
        {
            if (this.isloggingConfigured)
            {
                StaticLogger.Error(message);
            }
        }

        /// <summary>
        /// Output an exception as an error message.
        /// </summary>
        /// <param name="ex">The exception to output.</param>
        public void Error(Exception ex)
        {
            if (this.isloggingConfigured)
            {
                StaticLogger.Error(ex);
            }
        }

        /// <summary>
        /// Output an exception as an error message.
        /// </summary>
        /// <param name="message">The message to output.</param>
        /// <param name="ex">The exception to output.</param>
        public void Error(string message, Exception ex)
        {
            if (this.isloggingConfigured)
            {
                StaticLogger.Error(ex);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.isloggingConfigured)
            {
                StaticLogger.Dispose();
            }
        }
        #endregion

        #region private fields
        /// <summary>
        /// Flag to indicate if logging is configured.
        /// </summary>
        private bool isloggingConfigured = true; 
        #endregion
    }
}
