using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace RITS.StrymonEditor.Updater
{
    /// <summary>
    /// Auto updater class - used to apply auto updates.
    /// </summary>
    static class AutoUpdater
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            // initialise 

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // initialise thread exception handlers
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(OnGuiUnhandedException);

            // set shadow copy files to true
            AppDomain.CurrentDomain.SetupInformation.ShadowCopyFiles="true";

            // run updater progress form
            Application.Run(new UpdateProgressTracker());
        }

        /// <summary>
        /// Handles unhandled exception.
        /// </summary>
        /// <param name="sender">Sending object.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnUnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            // handle exception
            HandleUnhandledException(e.ExceptionObject);
        }

        /// <summary>
        /// Handles unhandled GUI exception.
        /// </summary>
        /// <param name="sender">Sending object.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnGuiUnhandedException(Object sender, ThreadExceptionEventArgs e)
        {
            // handle exception
            HandleUnhandledException(e.Exception);
        }

        static void HandleUnhandledException(Object o)
        {
            // force cast to exception
            Exception e = o as Exception;

            // if exception exists
            if (e != null)
            {
                // create thread dialog - set specifics & show
                ThreadExceptionDialog dialog = new ThreadExceptionDialog(e);
                dialog.Text = "Strymon Editor Auto Updater.";
                dialog.ShowDialog();
            }
            else
            { 
                // swallow error
            }

            // terminate application
            Environment.Exit(1);
        }
    }
}