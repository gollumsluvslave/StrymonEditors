using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace RITS.StrymonEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private NativeHooks _hooks = new WPFNativeHooks();
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Process thisProc = Process.GetCurrentProcess();
            if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1)
            {
                MessageBox.Show("Application running");
                Application.Current.Shutdown();
                return;
            }
            
        }


        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception != null)
            {
                MessageBox.Show("Unhandled Exception: " + e.Exception.Message);
            }
            e.Handled = true;
        }



    }
}
