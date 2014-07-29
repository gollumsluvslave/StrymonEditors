using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Net;
using RITS.StrymonEditor.Serialization;
using RITS.StrymonEditor.Logging;
using RITS.StrymonEditor.IO;
namespace RITS.StrymonEditor.AutoUpdate
{
    /// <summary>
    /// Provides functionality to determine whether or not there is an update available to the application
    /// </summary>
    public class UpdateChecker
    {
        // private vars
        private VersionConfig _currentVersionConfig;
        private VersionConfig _newVersionConfig;
        private string _newVersionConfigLocalPath;
        private IMessageDialog messageDialog;
        
        /// <summary>
        /// Default .ctor
        /// </summary>
        public UpdateChecker(IMessageDialog dialog)
        {
            this.messageDialog = dialog;
            using (XmlSerializer<VersionConfig> xs = new XmlSerializer<VersionConfig>())
            {
                _currentVersionConfig = xs.DeserializeFile("VersionConfig.xml");
            }
        }


        // public methods
        /// <summary>
        /// Checks to see if an update is required.
        /// </summary>
        /// <returns>Boolean determining if an update is required.</returns>
        public bool CheckForUpdate()
        {
            // First handle properties if this is already an upgrade
            using (RITSLogger logger = new RITSLogger())
            {
                if (Properties.Settings.Default.UpgradeRequired)
                {
                    HandlePostUpdateTasks(logger);
                }
                // Try and get current version info
                _newVersionConfigLocalPath = Path.GetTempFileName();
                if (!HttpUtils.SaveUrl(_currentVersionConfig.UpdateCheckTarget, _newVersionConfigLocalPath)) return false;
                try
                {
                    using (XmlSerializer<VersionConfig> xs = new XmlSerializer<VersionConfig>())
                    {
                        _newVersionConfig = xs.DeserializeFile(_newVersionConfigLocalPath);
                    }
                }
                catch { } // TODO tighten up
                if (_newVersionConfig == null) return false;
                // if version in update file is greater than current application version
                if (_newVersionConfig.VersionNo > _currentVersionConfig.VersionNo) return true;
                return false;
            }
        }

        /// <summary>
        /// Runs an update.
        /// </summary>
        public void RunUpdate()
        {
            try
            {
                //// Get zip, now have config, and zip locally, leave zip download for the actual updater?
                //string zipPath = Path.GetTempFileName();
                //HttpUtils.SaveUrl(_newVersionConfig.UpdateZipTarget, zipPath);
                using (RITSLogger logger = new RITSLogger())
                {
                    // compile update args into delimited string
                    string arg = AppDomain.CurrentDomain.BaseDirectory + "|" + _newVersionConfigLocalPath;
                    // Get update full path
                    string updaterExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RITS.StrymonEditor.Updater.exe");
                    // start the update process
                    logger.Debug(string.Format("Starting update : {0}. {1}",updaterExe,arg));
                    Process.Start(updaterExe, string.Format(@"""{0}""",arg));
                }
            }
            catch (Exception e)
            {
                // inform user of error
                messageDialog.ShowError("There was a problem runing the Auto Update.\n" + e.Message + "\nPlease contact your system administrator.", "Error occured.");
            }
        }


        private void HandlePostUpdateTasks(RITSLogger logger)
        {
            EnsureUpdaterProcessCompleted();
            logger.Debug("Upgrade required...");
            var autoUpdateLockedFiles = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutoUpdateFiles");            
            // Handle previous upgrade files
            if (Directory.Exists(autoUpdateLockedFiles))
            {
                foreach (var s in Directory.EnumerateFiles(autoUpdateLockedFiles))
                {
                    var realPath = s.Replace(@"AutoUpdateFiles\", "");
                    logger.Debug(string.Format("Checking locked AutoUpdateFile : {0}", realPath));
                    if (File.Exists(realPath))
                    {
                        logger.Debug(string.Format("Deleting AutoUpdateFile : {0}", realPath));
                        File.Delete(realPath);
                    }
                    logger.Debug(string.Format("Moving {0} to {1}", s, realPath));
                    File.Move(s, realPath);
                }
            }
            Properties.Settings.Default.Upgrade();
            Properties.Settings.Default.UpgradeRequired = false;
            Properties.Settings.Default.Save();
        }

        private void EnsureUpdaterProcessCompleted()
        {
            int timeoutCount = 0;
            int sleepPeriod = 500;
            while (true)
            {
                if (Process.GetProcessesByName("RITS.StrymonEditor.Updater").Length > 0)
                {
                    Thread.Sleep(sleepPeriod);
                }
                else
                {
                    return;
                }
                timeoutCount += sleepPeriod;
                if (timeoutCount > 20000)
                {
                    StaticLogger.Debug("Timeout waiting for Update process to exit. Abandoning AutoUpdate.");
                    return;
                }
            }
        }
    }

}
