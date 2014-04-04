using System;
using System.Diagnostics;
using System.IO;

namespace RITS.StrymonEditor.Logging
{
    /// <summary>
    /// A <see cref="TraceListener"/> that generates a new output file every day.
    /// </summary>
    public class RollingTextListener : TraceListener
    {
        #region .ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="RollingTextListener"/> class.
        /// </summary>
        public RollingTextListener()
        {
            if (IsValid())
            {
                // Pass in the path of the logfile (ie. C:\Logs\MyAppLog.log)
                // The logfile will actually be created with a yyyymmdd format appended to the filename
                this.traceWriter = new StreamWriter(this.GenerateFilename(), true);
            }
        } 
        #endregion

        #region public methods
        /// <summary>
        /// When overridden in a derived class, writes the specified message to the listener you create in the derived class.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void Write(string message)
        {
            lock (this.lockObject)
            {
                this.CheckRollover();

                if (this.traceWriter != null)
                {
                    this.traceWriter.Write(message);
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, writes a message to the listener you create in the derived class, followed by a line terminator.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void WriteLine(string message)
        {
            lock (this.lockObject)
            {
                this.CheckRollover();

                if (this.traceWriter != null)
                {
                    this.traceWriter.WriteLine(message);
                    this.traceWriter.Flush();
                }
            }
        } 
        #endregion

        #region protected methods
        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Diagnostics.TraceListener"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.traceWriter != null)
                {
                    this.traceWriter.Close();
                    this.traceWriter = null;
                }
            }
        } 
        #endregion

        public bool IsValid()
        {
            if (String.IsNullOrEmpty(this.fileName))
            {
                return false;
            }
            else
            {
                string filePath = Path.GetDirectoryName(this.fileName);
                if (!string.IsNullOrEmpty(filePath))
                {
                    if (!Directory.Exists(filePath))
                        return false;
                }
            }
            return true;
        }
        #region private methods
        /// <summary>
        /// Generates the filename.
        /// </summary>
        /// <returns>A new filename.</returns>
        private string GenerateFilename()
        {
            this.currentDate = DateTime.Today;

            string newFileName = Path.GetFileNameWithoutExtension(this.fileName) + "_" + this.currentDate.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) + Path.GetExtension(this.fileName);

            return Path.Combine(Path.GetDirectoryName(this.fileName), newFileName);
        }

        /// <summary>
        /// Checks the rollover.
        /// </summary>
        private void CheckRollover()
        {
            // If the date has changed, close the current stream and create a new file for today's date
            if (this.currentDate.CompareTo(System.DateTime.Today) != 0)
            {
                this.traceWriter.Close();
                this.traceWriter = new StreamWriter(this.GenerateFilename(), true);
            }
        } 
        #endregion

        #region private fields
        /// <summary>
        /// For thread-safety.
        /// </summary>
        private object lockObject = new object();

        /// <summary>
        /// Name of the logfile.
        /// </summary>
        private string fileName = "UpdaterLog.log";

        /// <summary>
        /// The current date.
        /// </summary>
        private DateTime currentDate;

        /// <summary>
        /// The output writer.
        /// </summary>
        private StreamWriter traceWriter; 
        #endregion
    }
}

