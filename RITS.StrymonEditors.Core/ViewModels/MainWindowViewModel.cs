﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using RITS.StrymonEditor.MIDI;
using RITS.StrymonEditor.Logging;
using RITS.StrymonEditor.AutoUpdate;
using RITS.StrymonEditor.Commands;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.IO;
using RITS.StrymonEditor.Messaging;

namespace RITS.StrymonEditor.ViewModels
{
    /// <summary>
    /// View Model responsible for the <see cref="MainWindow"/>
    /// </summary>
    public class MainWindowViewModel:ViewModelBase, IDisposable
    {
        private IStrymonMidiManager midiManager;
        private object lockObject = new object();
        private int connectedPedalCount = 0;
        private int currentBulkFetch =0;
        private int failedFetchCount = 0;
        private bool isPedalChange;

        public MainWindowViewModel(IStrymonMidiManager midiManager)
        {
            
            using (ILogger logger = NativeHooks.Current.CreateLogger())
            {
                logger.Debug("Setting midi manager...");
                this.midiManager = midiManager;
                if (HandleAutoUpdateUpdate())
                {
                    Globals.Init();
                    StatusText = "Initialising Midi...";
                    logger.Debug(string.Format("Initialising midi : configured InputDevice={0}, configured OutputDevice={1}",
                                    NativeHooks.Current.MIDIInDevice, NativeHooks.Current.MIDIOutDevice));
                    this.midiManager.InitMidi();
                    StatusText = "Ready. No Pedals Connected.";
                }
            }
        }

        #region Public Properties

        /// <summary>
        /// Delegate for closing the associated view / window
        /// </summary>
        public Action CloseWindow { get; set; }

        /// <summary>
        /// Returns the image that indicates MIDI connectivity
        /// </summary>
        public string MidiImage
        {
            get
            {
                if (connectedPedalCount > 0)
                {
                    return @"pack://application:,,,/Views/Images/MidiConnected.ico";
                }
                else
                {
                    return @"pack://application:,,,/Views/Images/NoMidi.ico";
                }
            }
        }

        /// <summary>
        /// Flag for the visibility of the bulk fetch progress bar
        /// </summary>
        private bool showProgressBar;
        public bool ShowProgressBar
        {
            get { return showProgressBar; }
            set { showProgressBar = value; OnPropertyChanged("ShowProgressBar"); }
        }

        /// <summary>
        /// The current status text
        /// </summary>
        private string status;
        public string StatusText
        {
            get { return status; }
            set { status = value; OnPropertyChanged("StatusText"); }
        }

        /// <summary>
        /// Returns the name of the 1st connected pedal
        /// </summary>
        private string ped1;
        public string ConnectedPedal1
        {
            get { return ped1; }
            set { ped1 = value; OnPropertyChanged("ConnectedPedal1"); }
        }

        /// <summary>
        /// Returns the name of the 2nd connected pedal
        /// NB Not supported
        /// </summary>
        private string ped2;
        public string ConnectedPedal2
        {
            get { return ped2; }
            set { ped2 = value; OnPropertyChanged("ConnectedPedal2"); }
        }

        /// <summary>
        /// Returns the name of the 3rd connected pedal
        /// NB Not supported
        /// </summary>
        private string ped3;
        public string ConnectedPedal3
        {
            get { return ped3; }
            set { ped3 = value; OnPropertyChanged("ConnectedPedal3"); }
        }

        /// <summary>
        /// Status text for the bulk fetch progress operation
        /// </summary>
        private string pbStatus;
        public string PBStatus
        {
            get { return pbStatus; }
            set { pbStatus = value; OnPropertyChanged("PBStatus"); }
        }

        /// <summary>
        /// Max for the bulk fetch operation
        /// </summary>
        private int pbMax;
        public int PBMax
        {
            get { return pbMax; }
            set { pbMax = value; OnPropertyChanged("PBMax"); }
        }

        /// <summary>
        /// Current value for the bulk fetch operation
        /// </summary>
        private int pbValue;
        public int PBValue
        {
            get { return pbValue; }
            set { pbValue = value; OnPropertyChanged("PBValue"); }
        }
        #endregion

        #region Menu Related

        /// <summary>
        /// Returns a bindable collection of <see cref="MenuItemViewModel"/> that drives the menu
        /// </summary>
        private IList<MenuItemViewModel> editorMenu;
        public IList<MenuItemViewModel> EditorMenu
        {
            get
            {
                if (editorMenu == null) SetupMenu();
                return editorMenu;
            }
        }


        private IModalDialog downloadWindow;
        /// <summary>
        /// Seam to allow testing of functions that open the <see cref="PedalEditor"/>
        /// </summary>
        public IModalDialog DownloadWindow
        {
            get
            {
                return downloadWindow;
            }
            set
            {
                downloadWindow = value;
            }
        }

        private IModalDialog editorWindow;
        /// <summary>
        /// Seam to allow testing of functions that open the <see cref="PedalEditor"/>
        /// </summary>
        public IModalDialog EditorWindow
        {
            get
            {
                return editorWindow;
            }
            set
            {
                editorWindow = value;
            }
        }

        // Helper that setups the menu
        private void SetupMenu()
        {
            editorMenu = NativeHooks.Current.CreateList<MenuItemViewModel>();
            var fileMenu = new MenuItemViewModel
            {
                Children = NativeHooks.Current.CreateList<MenuItemViewModel>(),
                MenuText = "File"

            };
            editorMenu.Add(fileMenu);
            // Not really using the strengths of xml here? Easier to hardwire the menus just now
            fileMenu.Children.Add(new MenuItemViewModel { MenuText = string.Format("New _{0} Preset", StrymonPedal.Timeline_Name), InputGestureText = "CTRL+N", Command = NewTimelinePresetCommand });
            fileMenu.Children.Add(new MenuItemViewModel { MenuText = string.Format("New _{0} Preset", StrymonPedal.Mobius_Name), InputGestureText = "SHIFT+N", Command = NewMobiusPresetCommand });
            fileMenu.Children.Add(new MenuItemViewModel { MenuText = string.Format("New _{0} Preset", StrymonPedal.BigSky_Name), InputGestureText = "ALT+N", Command = NewBigSkyPresetCommand });
            fileMenu.Children.Add(new MenuItemViewModel { IsSeparator = true });
            var loadXml = new MenuItemViewModel { MenuText = "Load Xml Preset", Command = LoadXmlCommand, InputGestureText = "SHIFT+L" };
            fileMenu.Children.Add(loadXml);
            var loadSysEx = new MenuItemViewModel { MenuText = "Load .SYX Preset", Command = LoadSyxCommand, InputGestureText = "ALT+L" };
            fileMenu.Children.Add(loadSysEx);
            var download = new MenuItemViewModel { MenuText = "Search Online", Command = DownloadCommand};
            fileMenu.Children.Add(download);

            var exit = new MenuItemViewModel { MenuText = "Exit", Command = ExitCommand, InputGestureText = "CTRL+X" };
            fileMenu.Children.Add(new MenuItemViewModel { IsSeparator = true });
            fileMenu.Children.Add(exit);

            var midiMenu = new MenuItemViewModel
            {
                MenuText = "Midi",
                Command = new RelayCommand(new Action(() =>
                {
                    var v = NativeHooks.Current.CreateMIDISetupDialog(midiManager);
                    v.ShowModal();
                }))
            };
            editorMenu.Add(midiMenu);
            var aboutMenu = new MenuItemViewModel
            {
                MenuText = "About",
                Command = new RelayCommand(new Action(() =>
                {
                    MessageDialog.ShowInfo(NativeHooks.Current.VersionInfo,"Strymon Editors");
                }))
            };
            editorMenu.Add(aboutMenu);
        }

        // Helper that opens the editor window based on the supplied StrymonPedal
        private void OpenEditor(StrymonPedal pedal)
        {
            midiManager.ContextPedal = pedal;
            OpenEditor(null as StrymonPreset);
        }

        // Helper that opens the editor window using the supplied StrymonPreset
        private void OpenEditor(StrymonPreset preset)
        {
            NativeHooks.Current.SetBusy();
            var pew = NativeHooks.Current.CreatePedalEditorWindow(preset, midiManager);
            EditorWindow = pew;
            EditorWindow.ShowModal();
        }

        // Helper that loads an xml file preset
        private void LoadXml()
        {
            var preset = FileIOService.LoadXmlPreset();
            if (preset != null)
            {
                OpenEditor(preset);
            }
        }
        #endregion

        #region Commands

        /// <summary>
        /// ICommand that handles loading an xml file preset
        /// </summary>
        public RelayCommand LoadXmlCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    LoadXml();
                }), new Func<bool>(BulkFetchDone));
            }
        }

        /// <summary>
        /// ICommand that handles loading a .syx file preset
        /// </summary>
        public RelayCommand LoadSyxCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    var preset = FileIOService.LoadSyxPreset();
                    if (preset != null)
                    {
                        OpenEditor(preset);
                    }
                }), new Func<bool>(BulkFetchDone));
            }
        }

        /// <summary>
        /// ICommand that handles loading a .syx file preset
        /// </summary>
        public RelayCommand DownloadCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    DownloadWindow = NativeHooks.Current.CreatePresetStoreDownloadDialog(true);
                    DownloadWindow.ShowModal();
                }), new Func<bool>(BulkFetchDone));
            }
        }

        /// <summary>
        /// ICommand that handles exiting the application
        /// </summary>
        public RelayCommand ExitCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    CloseWindow();
                }));
            }
        }

        /// <summary>
        /// ICommand that handles creating a new Timeline preset
        /// </summary>
        public RelayCommand NewTimelinePresetCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    OpenEditor(StrymonPedal.GetPedalByName(StrymonPedal.Timeline_Name));
                }), new Func<bool>(BulkFetchDone));
            }
        }
        private bool BulkFetchDone()
        {
            return !midiManager.IsBulkFetching;
        }
        /// <summary>
        /// ICommand that handles creating a new Mobius preset
        /// </summary>
        public RelayCommand NewMobiusPresetCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    OpenEditor(StrymonPedal.GetPedalByName(StrymonPedal.Mobius_Name));
                }), new Func<bool>(BulkFetchDone));
            }
        }

        /// <summary>
        /// ICommand that handles creating a new BigSky preset
        /// </summary>
        public RelayCommand NewBigSkyPresetCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    OpenEditor(StrymonPedal.GetPedalByName(StrymonPedal.BigSky_Name));
                }), new Func<bool>(BulkFetchDone));
            }
        }
        #endregion

        #region IColleague
        /// <inheritdoc/>
        public override void RegisterWithMediator()
        {
            Mediator.Register(ViewModelMessages.PedalConnected, PedalConnected);
            Mediator.Register(ViewModelMessages.BulkPresetRead, BulkPresetRead);
            Mediator.Register(ViewModelMessages.MIDIReset, MIDIReset);
            Mediator.Register(ViewModelMessages.MIDIConnectionComplete, MIDIConnectionComplete);
            Mediator.Register(ViewModelMessages.ReceivedPresetFromOnlineMainWindow, PedalDownloaded);
        }

        /// <inheritdoc/>
        public override void DeRegisterFromMediator()
        {
            Mediator.UnRegister(ViewModelMessages.PedalConnected, PedalConnected);
            Mediator.UnRegister(ViewModelMessages.BulkPresetRead, BulkPresetRead);
            Mediator.UnRegister(ViewModelMessages.MIDIReset, MIDIReset);
            Mediator.UnRegister(ViewModelMessages.MIDIConnectionComplete, MIDIConnectionComplete);
            Mediator.UnRegister(ViewModelMessages.ReceivedPresetFromOnlineMainWindow, PedalDownloaded);
        }
        #endregion

        #region Mediator Callbacks

        private void PedalDownloaded(object o)
        {
            var preset = o as StrymonPreset;
            OpenEditor(preset);
        }
        private void MIDIReset(object o)
        {
            this.midiManager = o as IStrymonMidiManager;
            connectedPedalCount = 0;
            ConnectedPedal1 = null;
            ConnectedPedal2 = null;
            ConnectedPedal3 = null;
            StatusText = "Ready. No Pedals Connected.";
            OnPropertyChanged("MIDIImage");
        }
        // Handle the MIDIConnection complete notification
        private void MIDIConnectionComplete(object o)
        {
            // Kick of preset load
            if (NativeHooks.Current.DisableBulkFetch) return;
            if (midiManager.ConnectedPedals.Count == 0) return;
            PBMax = midiManager.ConnectedPedals.Sum(x => x.PresetCount);
            ShowProgressBar = true;
            PBValue = 0;
            //if (midiManager.ConnectedPedals.Count > 0)
            foreach(var p in midiManager.ConnectedPedals)
            {
                BulkFetch(p);
                
            }

        }
        
        // Initiate a bulk fetch for the supplied pedal
        private void BulkFetch(StrymonPedal p)
        {
            
            for (int i = 0; i < p.PresetCount; i++)
            {
                currentBulkFetch++;
                NativeHooks.Current.Delay(NativeHooks.Current.BulkFetchDelay);
                midiManager.BulkPedal = p;
                midiManager.FetchByIndex(i);
            }
            NativeHooks.Current.Delay(1000);
            midiManager.BulkPedal = null;
        }

        // handle the bulkpresetread notification
        private void BulkPresetRead(object o)
        {
            var preset = o as StrymonPreset;
            PBValue = currentBulkFetch;
            if (preset == null)
            {
                failedFetchCount++;
                PBStatus = string.Format("Fetch Failed : {0}", currentBulkFetch);
            }
            else
            {
                PBStatus = string.Format("Fetched : {0}({1})", preset.Pedal.Name, preset.SourceIndex);
                if (preset.SourceIndex == preset.Pedal.PresetCount - 1)
                {
                    Mediator.NotifyColleagues(ViewModelMessages.BulkLoadPedalComplete, null);
                }
            }
            if (currentBulkFetch == PBMax)
            {
                PBStatus = (failedFetchCount > 0) ? string.Format("Loaded ({0} failed)", failedFetchCount) : "Loaded";
                ShowProgressBar = false;
                //Mediator.NotifyColleagues(ViewModelMessages.BulkLoadComplete, null);
            }
        }

        // Handles the pedalconnected notification
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

        #endregion
     
        #region IDisposable
        /// <inheritdoc/>
        public override void Dispose()
        {
            base.Dispose();
        }
        #endregion

        #region AutoUpdate Check
        private bool HandleAutoUpdateUpdate()
        {
            IAutoUpdater checker = NativeHooks.Current.CreateAutoUpdater();
            if (checker.CheckForUpdate())
            {
                MessageDialog.ShowInfo("A new version of Strymon Editors is available. The Editor will now exit and install the new version.", "New Version Available");
                checker.RunUpdate();
                CloseWindow();
                return false;
            }
            return true;
        }
        #endregion

    }
}
