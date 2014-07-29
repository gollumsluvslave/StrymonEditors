using System;

namespace RITS.StrymonEditor.Logging
{
    public enum LogLevel
    {
        Disabled=0,
        Error,
        Warn,
        Info,
        Debug
    }
    /// <summary>
    /// Interface that defines API for logging.
    /// </summary>    
    public interface ILogger : IDisposable
    {
        /// <summary>
        /// A method has been entered.
        /// </summary>
        void Entered();

        /// <summary>
        /// A method has completed.
        /// </summary>
        void Completed();

        /// <summary>
        /// Output a debug message.
        /// </summary>
        /// <param name="message">The message to output.</param>
        void Debug(string message);

        /// <summary>
        /// Output an information message.
        /// </summary>
        /// <param name="message">The message to output.</param>
        void Info(string message);

        /// <summary>
        /// Output a warning message.
        /// </summary>
        /// <param name="message">The message to output.</param>
        void Warn(string message);

        /// <summary>
        /// Output an exception message.
        /// </summary>
        /// <param name="message">The message to output.</param>
        void Error(string message);

        /// <summary>
        /// Output an exception as an error message.
        /// </summary>
        /// <param name="ex">The exception to output.</param>
        void Error(Exception ex);

        /// <summary>
        /// Output an exception as an error message.
        /// </summary>
        /// <param name="ex">The exception to output.</param>
        /// <param name="message">The message to output.</param>
        void Error(string message, Exception ex);
    }
}
