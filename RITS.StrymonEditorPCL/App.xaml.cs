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
            
            BuildNativeHooks();
        }

        public void BuildNativeHooks()
        {
            
            NativeHooks.Current.BigsSkyMIDIChannel = RITS.StrymonEditor.Properties.Settings.Default.BigSkyMidiChannel;
            NativeHooks.Current.MobiusMIDIChannel = RITS.StrymonEditor.Properties.Settings.Default.MobiusMidiChannel;
            NativeHooks.Current.TimelineMIDIChannel = RITS.StrymonEditor.Properties.Settings.Default.TimelineMidiChannel;
            NativeHooks.Current.BulkFetchDelay = RITS.StrymonEditor.Properties.Settings.Default.BulkFetchDelay;
            NativeHooks.Current.MIDIInDevice = RITS.StrymonEditor.Properties.Settings.Default.MidiInDevice;
            NativeHooks.Current.MIDIOutDevice = RITS.StrymonEditor.Properties.Settings.Default.MidiOutDevice;
            NativeHooks.Current.PushChunkDelay = RITS.StrymonEditor.Properties.Settings.Default.PushChunkDelay;
            NativeHooks.Current.PushChunkSize = RITS.StrymonEditor.Properties.Settings.Default.PushChunkSize;
            NativeHooks.Current.SyncMode = RITS.StrymonEditor.Properties.Settings.Default.SyncMode;
            NativeHooks.Current.DisableBulkFetch = RITS.StrymonEditor.Properties.Settings.Default.DisableBulkFetch;

            NativeHooks.Current.VersionInfo = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            
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
