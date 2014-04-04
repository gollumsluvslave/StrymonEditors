using System;

namespace RITS.StrymonEditor.Logging
{
    /// <summary>
    /// Facade class to wrapper concrete implementations of <see cref="ILogger"/>.
    /// </summary>
    public sealed class MCLogger : ILogger
    {
        #region .ctor
        public MCLogger() : this(true) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="MCLogger" /> class.
        /// </summary>
        /// <param name="logMethodEntered">Whether to log that the calling method was entered.</param>
        public MCLogger(bool logMethodEntered)
        {
            try
            {                
                this.configuredLogger = new DefaultLogger();                
                if (logMethodEntered) Entered();
            }
            catch (Exception ex)
            {
                throw new LoggingConfigurationException(string.Format("Unexpected Exception trying to create an ILogger implementation : {0}. Check the MC.Core.Logging applicationSettings to ensure the configuration is valid.",ex.Message),ex);
            }
            
        } 
        #endregion

        #region ILogger implementation
        /// <summary>
        /// A method has been entered.
        /// </summary>
        public void Entered()
        {
            if (this.configuredLogger != null)
            {
                this.configuredLogger.Entered();
            }
        }

        /// <summary>
        /// A method has completed.
        /// </summary>
        public void Completed()
        {
            if (this.configuredLogger != null)
            {
                this.configuredLogger.Completed();
            }
        }

        /// <summary>
        /// Output a debug message.
        /// </summary>
        /// <param name="message">The message to output.</param>
        public void Debug(string message)
        {
            if (this.configuredLogger != null)
            {
                this.configuredLogger.Debug(message);
            }
        }

        /// <summary>
        /// Output an information message.
        /// </summary>
        /// <param name="message">The message to output.</param>
        public void Info(string message)
        {
            if (this.configuredLogger != null)
            {
                this.configuredLogger.Info(message);
            }
        }

        /// <summary>
        /// Output a warning message.
        /// </summary>
        /// <param name="message">The message to output.</param>
        public void Warn(string message)
        {
            if (this.configuredLogger != null)
            {
                this.configuredLogger.Warn(message);
            }
        }

        /// <summary>
        /// Output an error message.
        /// </summary>
        /// <param name="message">The message to output.</param>
        public void Error(string message)
        {
            if (this.configuredLogger != null)
            {
                this.configuredLogger.Error(message);
            }
        }

        /// <summary>
        /// Output an exception as an error message.
        /// </summary>
        /// <param name="message">The message to output.</param>
        /// <param name="ex">The exception to output.</param>
        public void Error(string message, Exception ex)
        {
            if (this.configuredLogger != null)
            {
                this.configuredLogger.Error(ex);
            }
        }

        /// <summary>
        /// Output an exception as an error message.
        /// </summary>
        /// <param name="ex">The exception to output.</param>
        public void Error(Exception ex)
        {
            if (this.configuredLogger != null)
            {
                this.configuredLogger.Error(ex);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Completed();
            if (this.configuredLogger != null)
            {
                this.configuredLogger.Dispose();
                this.configuredLogger = null;
            }
        } 
        #endregion

        #region private fields
        /// <summary>
        /// The configured logger.
        /// </summary>
        private ILogger configuredLogger; 
        #endregion
    }
}
