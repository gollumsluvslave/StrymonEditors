using System;
using System.IO;
using System.Net;
using System.Data;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.Xml;
using RITS.StrymonEditor.AutoUpdate;
using RITS.StrymonEditor.Serialization;
using RITS.StrymonEditor.Logging;

namespace RITS.StrymonEditor.Updater
{
    public partial class UpdateProgressTracker : Form
    {
        /// <summary>
        /// Default constructor - initialises form.
        /// </summary>
        private string editorExe;
        private string appRoot;
        private string tmpZipPath;
        private string versionConfigFilePath;
        private VersionConfig currentVersionConfig;
        private VersionConfig newVersionConfig;
        public UpdateProgressTracker()
        {
            // initialise form
            InitializeComponent();
        }

        /// <summary>
        /// Form load event - Spawns worker thread to process update.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void UpdateProgressTracker_Load(object sender, EventArgs e)
        {
            // start execution of update in newly spawned worker thread
            ThreadStart workerThreadStart = new ThreadStart(RunUpdate);
            Thread workerThread = new Thread(workerThreadStart);
            workerThread.Start();
        }

        /// <summary>
        /// Runs the update.
        /// </summary>
        private void RunUpdate()
        {
            StaticLogger.Debug("Processing Command Line..."); 
            ProcessCommandLine();
            StaticLogger.Debug("Configuring form...");
            ConfigureForm(1, "Download");
            StaticLogger.Debug("Fetching zip...");
            FetchZipTarget();
            // Backup previous version??
            StaticLogger.Debug("Backing up previous version...");
            BackupCurrentVersion();
            StaticLogger.Debug("Installing new version...");
            // Restart the editor                
            
            ExtractZip();
            Process.Start(editorExe);
            Application.Exit();
        }

        private void ConfigureForm(int fileCount,string stage)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => { UIConfigureForm(fileCount, stage); }));              
            }       
        }
        private void UIConfigureForm(int fileCount, string stage)
        {
            // initialise form progress bar
            prgOverallProgress.Minimum = 0;
            prgOverallProgress.Maximum = fileCount;
            prgOverallProgress.Value = 0;
            lblFileCounters.Text =string.Format("Stage: {0} : Processing {1} files...", stage, fileCount);
        }


        private void ProcessCommandLine()
        {
            // attempt to populate method vars from params (stored in args)
            StaticLogger.Debug(string.Format("Commandline : {0}",String.Join(",",Environment.GetCommandLineArgs())));
            string[] updateParams = Environment.GetCommandLineArgs()[1].Split('|');
            StaticLogger.Debug(string.Format("argCount : {0}", updateParams.Length));
            appRoot = updateParams[0].Replace("\"", "");
            StaticLogger.Debug(string.Format("appRoot : {0}", appRoot));
            versionConfigFilePath = updateParams[1].Replace("\"", "");
            StaticLogger.Debug(string.Format("versionConfigFilePath : {0}", versionConfigFilePath));
            editorExe = Path.Combine(appRoot, @"RITS.StrymonEditor.exe"); // TODO hardcoding
            StaticLogger.Debug(string.Format("editorExe : {0}", editorExe));
            // deserialise new app version config from file path
            using (XmlSerializer<VersionConfig> xs = new XmlSerializer<VersionConfig>())
            {
                newVersionConfig = xs.DeserializeFile(versionConfigFilePath);
            }
            StaticLogger.Debug("Deserialised new config...");
            // deserialise current version config from file path
            using (XmlSerializer<VersionConfig> xs = new XmlSerializer<VersionConfig>())
            {
                currentVersionConfig = xs.DeserializeFile(Path.Combine(appRoot,"VersionConfig.xml"));
            }
            StaticLogger.Debug("Deserialised current config...");
        }

        private void FetchZipTarget()
        {
            UpdateForm(1, tmpZipPath, "Download...");
            tmpZipPath = Path.GetTempFileName();
            HttpUtils.SaveUrl(newVersionConfig.UpdateZipTarget, tmpZipPath);

        }

        private void BackupCurrentVersion()
        {
            string pvFolder= Path.Combine(appRoot,"PreviousVersions");
            Directory.CreateDirectory(pvFolder);
            string zipPath = Path.Combine(pvFolder, currentVersionConfig.Version + ".zip");
            ZipUtils.ZipFolder(zipPath, appRoot, ConfigureForm, UpdateForm);
        }

        private void ExtractZip()
        {
            ZipUtils.ExtractZip(tmpZipPath, appRoot, ConfigureForm, UpdateForm);
        }

        private void UpdateForm(int counter, string file, string stage)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => { UIUpdateForm(counter,file,stage); }));
            }
        }

        private void UIUpdateForm(int counter, string file, string stage)
        {
            lblFileCounters.Text = string.Format("{0} : Processing {1} of {2} files...", stage,counter,prgOverallProgress.Maximum);
            lblFileName.Text = file;
            prgOverallProgress.Value = counter;
            Refresh();
        }

        private void TerminateWithMessage(string message)
        {
            // inform user of error
            MessageBox.Show("There was a problem runing the Auto Updater.\n" + message + "\nPlease contact your system administrator.", "Error occured.", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // terminate application
            Environment.Exit(1);
        }
    }

}