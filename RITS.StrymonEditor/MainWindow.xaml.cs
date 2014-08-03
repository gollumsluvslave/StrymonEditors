using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using RITS.StrymonEditor.AutoUpdate;
using RITS.StrymonEditor.MIDI;
using RITS.StrymonEditor.Logging;
using RITS.StrymonEditor.Views;
using RITS.StrymonEditor.ViewModels;
using RITS.StrymonEditor.Models;
namespace RITS.StrymonEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(UnhandledException);
            ViewModel.CloseWindow = CloseWindow;
            InitializeComponent();
            
        }

        /// <summary>
        /// Returns the ViewModel instance for databinding
        /// </summary>
        private MainWindowViewModel viewModel = new MainWindowViewModel(new StrymonMidiManager(MidiDevices.ConfiguredInputDevice, MidiDevices.ConfiguredOutputDevice));
        public MainWindowViewModel ViewModel
        {
            get
            {
                return viewModel;
            }
        }

        // Delegate to allow the viewmodel to close the window
        private void CloseWindow()
        {
            this.Close();
        }

        // Unhandled exception handler
        private void UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {

            if (e.Exception != null)
            {
                using (RITSLogger logger = new RITSLogger())
                {
                    logger.Error(e.Exception);
                }
                MessageBox.Show("Unhandled Exception: " + e.Exception.Message);
            }
            e.Handled = true;
        }

        // Closing - dispose of ViewMNodel and save properties
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            ViewModel.Dispose();
        }
    }
}
