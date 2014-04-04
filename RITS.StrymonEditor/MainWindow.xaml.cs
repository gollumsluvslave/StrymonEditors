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
using RITS.StrymonEditor.Views;
using RITS.StrymonEditor.ViewModels;
using RITS.StrymonEditor.Models;
namespace RITS.StrymonEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private object lockObject = new object();
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private int connectedPedalCount=0;
        private StrymonMidiManager midiManager = new StrymonMidiManager();
        public MainWindow()
        {
            Dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(UnhandledException);
            ViewModelBase.mediatorInstance.Register(ViewModelMessages.PedalConnected, PedalConnected);
            ViewModelBase.mediatorInstance.Register(ViewModelMessages.BulkPresetRead, BulkPresetRead);
            ViewModelBase.mediatorInstance.Register(ViewModelMessages.MIDIConnectionComplete, MIDIConnectionComplete);
            if (HandleAutoUpdateUpdate())
            {
                Globals.Init();
                // Go and fetch the preset names for the connected pedals!
                InitializeComponent();
                //
                StatusText = "Initialising Midi...";
                midiManager.InitMidi();
                StatusText = "Ready. No Pedals Connected.";
            }
            
        }

        private bool HandleAutoUpdateUpdate()
        {
            UpdateChecker checker = new AutoUpdate.UpdateChecker();
            if (checker.CheckForUpdate())
            {                
                checker.RunUpdate();
                this.Close();
                return false;   
            }
            return true;
        }

        private void MIDIConnectionComplete(object o)
        {
            // Kick of preset load
            ShowProgressBar = true;
            foreach (var p in midiManager.ConnectedPedals)
            {
                PBMax = p.PresetCount - 1;
                midiManager.BulkFetch(p);
                break;
            }
            
        }

        private void BulkPresetRead(object o)
        {
            var preset = o as StrymonPreset;
            PBValue = preset.SourceIndex;
            PBStatus = string.Format("Fetching : {0}({1})", preset.Pedal.Name, preset.SourceIndex);
            if (preset.SourceIndex == PBMax)
            {
                PBStatus = "Loaded";
                ShowProgressBar = false;
                ViewModelBase.mediatorInstance.NotifyColleagues(ViewModelMessages.BulkLoadComplete, null);
            }
        }

        public string MidiImage
        {
            get
            {
                if (connectedPedalCount>0)
                {
                    return @"pack://application:,,,/Views/Images/MidiConnected.ico";
                }
                else
                {
                    return @"pack://application:,,,/Views/Images/NoMidi.ico";
                }
            }
        }

        private bool showProgressBar;
        public bool ShowProgressBar
        {
            get { return showProgressBar; }
            set { showProgressBar = value; OnPropertyChanged("ShowProgressBar"); }
        }

        private void PedalConnected(object pedal)
        {
            lock (lockObject)
            {
                var p = pedal as StrymonPedal;
                connectedPedalCount++;

                if (connectedPedalCount == 1)
                {
                    ConnectedPedal1 = p.Name;
                    OnPropertyChanged("MidiImage");
                }
                if (connectedPedalCount == 2) ConnectedPedal2 = p.Name;
                if (connectedPedalCount == 3) ConnectedPedal3 = p.Name;
                StatusText = string.Format("Ready. ({0} Pedal Connected)", connectedPedalCount);

                // How to queue these in synchronous sequence?
                //midiManager.BulkFetch(p);
            }

            
        }

        

        private string status;
        public string StatusText 
        {
            get { return status; }
            set { status=value; OnPropertyChanged("StatusText"); }
        }

        private string ped1;
        public string ConnectedPedal1
        {
            get { return ped1; }
            set { ped1 = value; OnPropertyChanged("ConnectedPedal1"); }
        }

        private string ped2;
        public string ConnectedPedal2
        {
            get { return ped2; }
            set { ped2 = value; OnPropertyChanged("ConnectedPedal2"); }
        }

        private string ped3;
        public string ConnectedPedal3
        {
            get { return ped3; }
            set { ped3 = value; OnPropertyChanged("ConnectedPedal3"); }
        }

        private string pbStatus;
        public string PBStatus
        {
            get { return pbStatus; }
            set { pbStatus = value; OnPropertyChanged("PBStatus"); }
        }

        private int pbMax;
        public int PBMax
        {
            get { return pbMax; }
            set { pbMax = value; OnPropertyChanged("PBMax"); }
        }

        private int pbValue;
        public int PBValue
        {
            get { return pbValue; }
            set { pbValue = value; OnPropertyChanged("PBValue"); }
        }


        private BindableCollection<MenuItemViewModel> editorMenu;
        public BindableCollection<MenuItemViewModel> EditorMenu
        {
            get
            {
                if (editorMenu == null) SetupMenu();
                return editorMenu;
            }
        }

        private void SetupMenu()
        {
            editorMenu = new BindableCollection<MenuItemViewModel>();
            var fileMenu = new MenuItemViewModel
            {
                Children = new BindableCollection<MenuItemViewModel>(),
                MenuText = "File"
                
            };
            editorMenu.Add(fileMenu);
            // Not really using the strengths of xml here? Easier to hardwire the menus just now
            fileMenu.Children.Add(new MenuItemViewModel { MenuText = string.Format("New _{0} Preset", StrymonPedal.Timeline_Name), InputGestureText ="CTRL+N", Command = NewTimelinePresetCommand });
            fileMenu.Children.Add(new MenuItemViewModel { MenuText = string.Format("New _{0} Preset", StrymonPedal.Mobius_Name), InputGestureText="SHIFT+N", Command = NewMobiusPresetCommand });
            fileMenu.Children.Add(new MenuItemViewModel { MenuText = string.Format("New _{0} Preset", StrymonPedal.BigSky_Name), InputGestureText = "ALT+N", Command = NewBigSkyPresetCommand });
            fileMenu.Children.Add(new MenuItemViewModel { IsSeparator = true });
            var loadXml = new MenuItemViewModel { MenuText = "Load Xml Preset", Command = LoadXmlCommand, InputGestureText = "SHIFT+L" };
            fileMenu.Children.Add(loadXml);
            var loadSysEx = new MenuItemViewModel { MenuText = "Load .SYX Preset", Command = LoadSyxCommand, InputGestureText = "ALT+L" };
            fileMenu.Children.Add(loadSysEx);

            var exit = new MenuItemViewModel { MenuText = "Exit", Command = ExitCommand, InputGestureText = "CTRL+X" };
            fileMenu.Children.Add(new MenuItemViewModel { IsSeparator = true });
            fileMenu.Children.Add(exit);

            var midiMenu = new MenuItemViewModel
            {
                MenuText = "Midi",
                Command = new RelayCommand(new Action(()=>
                {
                    var v = new MidiSetup(midiManager);
                    v.ShowDialog();
                }))
            };
            editorMenu.Add(midiMenu);
            var aboutMenu = new MenuItemViewModel
            {
                MenuText = "About",
                Command = new RelayCommand(new Action(() =>
                {
                    MessageBox.Show(string.Format("Strymon Editors {0}",System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));
                }))
            };
            editorMenu.Add(aboutMenu);
        }
        #region Commands
        public RelayCommand LoadXmlCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    LoadXml();
                }));
            }
        }

        public RelayCommand LoadSyxCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    var preset = IOUtils.LoadSyxPreset();
                    if (preset != null)
                    {
                        OpenEditor(preset);
                    }
                }));
            }
        }

        public RelayCommand ExitCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    this.Close();
                }));
            }
        }

        public RelayCommand NewTimelinePresetCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                    {
                        OpenEditor(StrymonPedal.GetPedalByName(StrymonPedal.Timeline_Name));
                    }));
            }
        }

        public RelayCommand NewMobiusPresetCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    OpenEditor(StrymonPedal.GetPedalByName(StrymonPedal.Mobius_Name));
                }));
            }
        }

        public RelayCommand NewBigSkyPresetCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    OpenEditor(StrymonPedal.GetPedalByName(StrymonPedal.BigSky_Name));
                }));
            }
        }
        #endregion


        private void OpenEditor(MenuItemViewModel menuItem)
        {
            StrymonPedal pedal = menuItem.Tag as StrymonPedal;
            OpenEditor(new StrymonPreset(pedal,true));
        }

        private void OpenEditor(StrymonPedal pedal)
        {
            OpenEditor(new StrymonPreset(pedal, true));
        }

        private void OpenEditor(StrymonPreset preset)
        {
            PedalEditor editor = new PedalEditor(preset, midiManager);
            editor.ShowDialog();
        }

        private void LoadXml()
        {
            var preset = IOUtils.LoadXmlPreset();
            if (preset != null)
            {
                OpenEditor(preset);
            }
        }

        private void UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {

            if (e.Exception != null)
            {
                MessageBox.Show("Unhandled Exception: " + e.Exception.Message);
            }
            e.Handled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            ViewModelBase.mediatorInstance.UnRegister(ViewModelMessages.PedalConnected, PedalConnected);
            ViewModelBase.mediatorInstance.UnRegister(ViewModelMessages.BulkPresetRead, BulkPresetRead);
            ViewModelBase.mediatorInstance.UnRegister(ViewModelMessages.MIDIConnectionComplete, MIDIConnectionComplete);
        }
    }
}
