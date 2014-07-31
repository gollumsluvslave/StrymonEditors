using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using RITS.StrymonEditor.MIDI;

using RITS.StrymonEditor.Logging;
using RITS.StrymonEditor.Commands;
using RITS.StrymonEditor.Conversion;
using RITS.StrymonEditor.IO;
using RITS.StrymonEditor.Messaging;
using RITS.StrymonEditor.Models;

namespace RITS.StrymonEditor.ViewModels
{
    /// <summary>
    /// 'Main' Editor view model that abstracts both the <see cref="StrymonPedal"/> 
    /// and the currently active <see cref="StrymonPreset"/> from the UI / View
    /// </summary>
    public class StrymonPedalViewModel : ViewModelBase, IDisposable
    {
        private IStrymonMidiManager midiManager;
        private bool presetFromPedal;

        /// <summary>
        /// Default .ctor
        /// </summary>
        /// <param name="preset"></param>
        /// <param name="midiManager"></param>
        public StrymonPedalViewModel(StrymonPreset preset, IStrymonMidiManager midiManager)
        {
            this.midiManager = midiManager;
            if (preset == null)
            {
                if (midiManager.IsConnected)
                {                    
                    midiManager.FetchCurrent();
                    ActivePedal = midiManager.ContextPedal;                    
                    return;
                }
                preset = new StrymonPreset(midiManager.ContextPedal,true);
            }
            
            midiManager.ContextPedal=preset.Pedal;

            using (ILogger logger = NativeHooks.Current.CreateLogger())
            {
                logger.Debug(string.Format("Creating ViewModel for Preset: {0}", preset.Name));
                SetSyncMode();
                this.ActivePreset = preset;
            }
            // Preserve original state
            originalState = preset.ToXmlPreset();

        }



        /// <inheritdoc/>
        public override void RegisterWithMediator()
        {
            Mediator.Register(ViewModelMessages.MachineSelected, MachineChanged);
            Mediator.Register(ViewModelMessages.ParameterChanged, HandleParameterChanged);
            Mediator.Register(ViewModelMessages.LCDUpdate, LCDUpdate);
            Mediator.Register(ViewModelMessages.ReceivedPresetFromPedal, PresetReceived);
            Mediator.Register(ViewModelMessages.ReceivedCC, ReceiveCCChange);
            Mediator.Register(ViewModelMessages.FetchPresetRequested, FetchPresetRequested);
            Mediator.Register(ViewModelMessages.PushPresetRequested, PushPresetRequested);
            Mediator.Register(ViewModelMessages.BulkLoadPedalComplete, BulkLoadComplete);
            Mediator.Register(ViewModelMessages.PresetRenamed, PresetRenamed);
            Mediator.Register(ViewModelMessages.DirectEntryValueEntered, DirectEntryValueEntered);
            Mediator.Register(ViewModelMessages.PushPresetFailed, PushPresetFailed);
        }

        /// <inheritdoc/>
        public override void DeRegisterFromMediator()
        {
            Mediator.UnRegister(ViewModelMessages.MachineSelected, MachineChanged);
            Mediator.UnRegister(ViewModelMessages.ParameterChanged, HandleParameterChanged);
            Mediator.UnRegister(ViewModelMessages.LCDUpdate, LCDUpdate);
            Mediator.UnRegister(ViewModelMessages.ReceivedPresetFromPedal, PresetReceived);
            Mediator.UnRegister(ViewModelMessages.ReceivedCC, ReceiveCCChange);
            Mediator.UnRegister(ViewModelMessages.FetchPresetRequested, FetchPresetRequested);
            Mediator.UnRegister(ViewModelMessages.PushPresetRequested, PushPresetRequested);
            Mediator.UnRegister(ViewModelMessages.BulkLoadPedalComplete, BulkLoadComplete);
            Mediator.UnRegister(ViewModelMessages.PresetRenamed, PresetRenamed);
            Mediator.UnRegister(ViewModelMessages.DirectEntryValueEntered, DirectEntryValueEntered);
            Mediator.UnRegister(ViewModelMessages.PushPresetFailed, PushPresetFailed);
        }

        #region MIDIConnectivity

        /// <summary>
        /// Whether the preset is connected to a pedal in 'real-time' midi mode
        /// </summary>
        public bool IsConnected
        {
            get { return midiManager.IsConnected; }
        }

        /// <summary>
        /// Whether the preset is connected to a pedal in 'real-time' midi mode, obvious inverse of IsConnected
        /// </summary>
        public bool IsNotConnected
        {
            get { return !IsConnected; }
        }

        public SyncMode SyncMode { get; set; }

        private PresetControlViewModel fetchPreset;
        public PresetControlViewModel FetchPreset
        {
            get
            {
                if (fetchPreset == null)
                {
                    fetchPreset = new PresetControlViewModel("Fetch", ActivePedal,midiManager);
                }
                return fetchPreset;
            }
        }

        private PresetControlViewModel pushPreset;
        public PresetControlViewModel PushPreset
        {
            get
            {
                if (pushPreset == null)
                {
                    pushPreset = new PresetControlViewModel("Push", ActivePedal, midiManager);
                }
                return pushPreset;
            }
        }
        #endregion

        #region IDisposable

        /// <summary>
        /// Dispose, base handles dereg, call to other VMs
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            FetchPreset.Dispose();
            PushPreset.Dispose();
            foreach (var p in HiddenParameters)
            {
                p.Dispose();
            }
            midiManager.ContextPedal = null;
        }
        #endregion

        #region Dynamic UI Properties
        /// <summary>
        /// Returns the image for the UI to use for the pedal
        /// </summary>
        public string Image
        {
            get
            {
                return NativeHooks.Current.PedalImage(ActivePedal.Name);
            }
        }

        /// <summary>
        /// Determines the UI colour for the 'hidden' preset area
        /// </summary>
        public string PedalColour
        {
            get
            {
                return ActivePedal.Colour;
            }
        }

        #endregion

        #region Simple Properties

        /// <summary>
        /// Callback to close the asociated view 
        /// 
        /// TODO - push this to ViewModelBase?
        /// 
        /// </summary>
        public Action CloseWindow { get; set; }


        private string _lcdValue;
        /// <summary>
        /// Property that determines what is displayed in the LCD display
        /// </summary>
        public string LCDValue
        {
            get
            {
                return _lcdValue;
            }
            set
            {
                if (!Globals.IsPedalViewLoading)
                {
                    _lcdValue = value;
                }
                else
                {
                    _lcdValue = ActivePreset.Name;
                }
                OnPropertyChanged("LCDValue");
            }
        }

        private string _epSetMode;
        /// <summary>
        /// Property that shows what is displayed to indicate EPSetMode
        /// </summary>
        public string EPSetMode
        {
            get
            {
                return _epSetMode;
            }
            set
            {
                _epSetMode = value;
                OnPropertyChanged("EPSetMode");
            }
        }

        private int _virtualEPValue;
        public int VirtualExpressionPedalValue
        {
            get { return _virtualEPValue; }
            set
            {
                _virtualEPValue = value;
                OnPropertyChanged("VirtualExpressionPedalValue");
                midiManager.SendVirtualEP(_virtualEPValue);
            }
        }
        /// <summary>
        /// Returns a title for this view
        /// </summary>
        public string Title
        {
            get
            {
                var dirtyFlag = IsDirty ? " *EDITED*" : "";
                return string.Format("{0} - {1}{2}", ActivePedal.Name, ActivePreset.Name, dirtyFlag);
            }
            set
            {
            }
        }

        /// <summary>
        /// Flag that indictates whether the activePreset has been edited
        /// Should delegate to a flag on the <see cref="StrymonPreset"/> instance!
        /// </summary>
        public bool IsDirty
        {
            get { return IsEdited; }
            set
            {
                // Update state to current
                if (value == false) originalState = ActivePreset.ToXmlPreset();
                OnPropertyChanged("IsDirty");
                OnPropertyChanged("Title");
            }
        }

        #endregion

        #region Pots
        private PotViewModel _encoder;
        /// <summary>
        /// Exposes a <see cref="PotViewModel"/> that abstracts the endless encoder used 
        /// for fine control of Time/Decay etc
        /// </summary>
        public PotViewModel Encoder
        {
            get
            {
                if (_encoder == null)
                {
                    var finepot = ActivePedal.Pots.FirstOrDefault(x => x.Id == 0);
                    var fineCoarseParam = ActivePreset.ControlParameters.FirstOrDefault(x => x.Definition.PotId == 1);
                    _encoder = PotViewModelFactory.Create(finepot, fineCoarseParam);
                    _encoder.ContextPedal = ActivePedal;
                }
                return _encoder;
            }
        }


        private List<PotViewModel> _potControls;
        /// <summary>
        /// List of pot controls for the pedal.
        /// NB this 
        /// </summary>
        public List<PotViewModel> PotControls
        {
            get
            {
                if (_potControls == null) LoadPotControls();
                return _potControls;
            }
        }

        #endregion

        #region Preset / State Properties
        private StrymonPreset preset;
        private StrymonXmlPreset originalState;
        private StrymonXmlPreset epSetCachedState;
        /// <summary>
        /// The currently active <see cref="StrymonPreset"/>
        /// </summary>
        public StrymonPreset ActivePreset
        {
            get { return preset; }
            set
            {
                if (Globals.MachineLocked && value.Machine.Value != Globals.LockedMachine)
                {
                    if (MessageDialog.ShowYesNo("The loaded preset has a different machine to the current (locked) machine. Do you wish to unlock the machine and load the preset?", "Machine Different"))
                    {
                        LockMachine(null);
                    }
                    else
                    {
                        return;
                    }
                }
                preset = value;
                ActivePedal = preset.Pedal;
                if (preset.Name == null) preset.Name = "NEW";
                // Rather than doing a ton of CC messages if the preset was not sourced from the pedal, then send to edit buffer?
                Globals.PotValueMap = preset.PotValueMap;
                OnPropertyChanged("ActivePedal");
                OnPropertyChanged("PedalColour");
                OnPropertyChanged("Image");
                OnPropertyChanged("ActivePreset");
                this.ActiveMachine = Machines.First(x => x.Value == ActivePreset.Machine.Value);
                if (preset.EPSetValues == null)
                {
                    preset.EPSetValues = new List<HeelToeSetting>();
                    foreach (var p in PotControls.Where(x=>!x.Hide))
                    {
                        preset.EPSetValues.Add(new HeelToeSetting { PotId = p.Id });
                    }
                }
                
            }
        }


        /// <summary>
        /// The currently active <see cref="StrymonPedal"/>
        /// </summary>
        private StrymonPedal activePedal;
        public StrymonPedal ActivePedal
        {
            get { return activePedal; }
            set { activePedal = value; }
        }

        private StrymonMachineViewModel activeMachine;

        /// <summary>
        /// The currently selected <see cref="StrymonMachine"/> in the active <see cref="StrymonPreset"/>
        /// </summary>
        public StrymonMachineViewModel ActiveMachine
        {
            get
            {
                if (activeMachine == null) activeMachine = Machines.First(x => x.Value == ActivePreset.Machine.Value);
                return activeMachine;
            }
            set
            {
                if (Globals.MachineLocked)
                {
                    OnPropertyChanged("ActiveMachine"); //  Reset combo
                    return;
                }
                using (ILogger logger = NativeHooks.Current.CreateLogger())
                {
                    activeMachine = value;
                    ActivePreset.Machine = activeMachine._machine;
                    activeMachine.IsActive = true;
                    // Deactivate any other machines that areactive
                    foreach (var m in Machines.Where(m => m.Value != activeMachine.Value && m.IsActive))
                    {
                        m.IsActive = false;
                    }
                    midiManager.SynchMachine(activeMachine._machine);
                    OnPropertyChanged("ActiveMachine");
                    RefreshView();
                    LCDValue = ActivePreset.Name;
                }
            }
        }


        #endregion

        #region Bindable Collections
        private IList<StrymonMachineViewModel> _machines;
        /// <summary>
        /// Returns a 'bindable' collection of view models that allow the View to interpret 
        /// the available <see cref="StrymonMachine"/>
        /// </summary>
        public IList<StrymonMachineViewModel> Machines
        {
            get
            {
                if (_machines == null)
                {
                    _machines = NativeHooks.Current.CreateList<StrymonMachineViewModel>();
                    foreach (var m in ActivePedal.Machines.OrderBy(p => p.Value))
                    {
                        _machines.Add(new StrymonMachineViewModel(m));
                    }
                }
                return _machines;
            }
            set
            {
                _machines = value;
                OnPropertyChanged("Machines");
            }
        }


        private IList<ParameterViewModel> _hiddenParameters;
        /// <summary>
        /// Returns a 'bindable' collection of view models that allow the View to interpret 
        /// the available 'hidden' <see cref="Parameter"/>
        /// </summary>
        public IList<ParameterViewModel> HiddenParameters
        {
            get
            {
                if (_hiddenParameters == null)
                {
                    _hiddenParameters = NativeHooks.Current.CreateList<ParameterViewModel>();
                    if (preset != null)
                    {
                        foreach (var p in ActivePreset.HiddenParameters)
                        {
                            var pvm = new ParameterViewModel(p);
                            _hiddenParameters.Add(pvm);
                        }
                    }
                    else { _hiddenParameters = null; }
                }
                return _hiddenParameters;
            }
            set
            {
                _hiddenParameters = value;
                OnPropertyChanged("HiddenParameters");
            }
        }

        private IList<MenuItemViewModel> editorMenu;
        /// <summary>
        /// Menu collection for the Main pedal screen menu
        /// </summary>
        public IList<MenuItemViewModel> EditorMenu
        {
            get
            {
                if (editorMenu == null) SetupMenu();
                return editorMenu;
            }
        }

        private IList<MenuItemViewModel> lcdMenu;
        /// <summary>
        /// Menu collection for the LCD display - only rename currently
        /// </summary>
        public IList<MenuItemViewModel> LCDMenu
        {
            get
            {
                if (lcdMenu == null) SetupLCDMenu();
                return lcdMenu;
            }
        }
        #endregion

        #region Private Methods

        // sets the sync mode
        private void SetSyncMode()
        {
            SyncMode test;
            if (Enum.TryParse<SyncMode>(NativeHooks.Current.SyncMode, out test))
            {
                SyncMode = test;
            }
            else
            {
                SyncMode = SyncMode.TwoWay;
            }
            foreach (var v in OptionsMenu[0].Children)
            {
                v.IsChecked = ((SyncMode)v.Tag == SyncMode);
                v.IsEnabled = !v.IsChecked;
            }
            midiManager.SyncMode = SyncMode;
        }


        // Method that does a number of things
        // 1 - builds the backing _potControls collection
        // 2 - Associates the relevant parameter to the pot control
        // 3 - Handles 'dynamic' pots in Mobius & BigSky
        // 4 - Handles fine/coarse specific controls, including overrides
        private void LoadPotControls()
        {
            using (ILogger logger = NativeHooks.Current.CreateLogger())
            {
                Globals.IsPedalViewLoading = true;
                _potControls = new List<PotViewModel>();
                foreach (var pot in ActivePedal.Pots.Where(x => x.Id != 0).OrderBy(x => x.Id))
                {
                    LoadPotControl(pot);
                }
                //Only need to do this when not come from Pedal
                if (!presetFromPedal)
                {
                    foreach (var hp in HiddenParameters)
                    {
                        midiManager.SynchParameter(hp._parameter);
                    }
                }
                logger.Debug(string.Format("{0} Pot Controls Created.", _potControls.Count));
                PresetRefreshComplete();
            }
        }

        private void PresetRefreshComplete()
        {
            if (!presetFromPedal)midiManager.PushToEdit(ActivePreset);
            midiManager.DisableControlChangeSends = false;
            midiManager.UpdateDisplay();
            IsDirty = false;
            presetFromPedal = false;
            Globals.IsPedalViewLoading = false;
            NativeHooks.Current.WorkComplete();
        }

        private void LoadPotControl(Pot pot)
        {
            var potParam = ActivePreset.ControlParameters.FirstOrDefault(x => x.Definition.PotId == pot.Id);
            //potParam.Value = GetValueFromPreviousParameter(potParam.Name);
            // Get default value?
            var vm = PotViewModelFactory.Create(pot, potParam);
            vm.ContextPedal = ActivePedal;
            _potControls.Add(vm);
            if (!pot.Hide)
            {
                if (pot.IsDynamic) LoadDynamicPot(pot, vm);
                else LoadNormalPot(potParam, vm);                
            }
        }

        private void LoadNormalPot(Parameter potParam, PotViewModel vm)
        {
            vm.LinkedParameter = potParam;
            var cvm = vm as CoarsePotViewModel;
            var machineOverridePot = ActiveMachine._machine.Pots.FirstOrDefault(x => x.Id == vm.Id);
            if (cvm != null)
            {
                cvm.LinkedParameter.Definition.ResetRange();
                // Only apply increment map after range has been overriden, reset synchroniser to pickup override                
                if (!cvm.HandleFineRangeOverrides(machineOverridePot, ActivePreset))
                {
                    // reset default incement map
                    Globals.FineCoarseSynchroniser = new FineCoarseSynchroniser(cvm, Encoder);
                    Globals.PotValueMap.ApplyFineValueIncrementMap(ActivePreset.Pedal.IncrementMap, potParam.Definition);
                }
                else Globals.FineCoarseSynchroniser = new FineCoarseSynchroniser(cvm, Encoder);                
            }
            else if (machineOverridePot != null) vm.Label = machineOverridePot.Label;
        }

        private void LoadDynamicPot(Pot pot, PotViewModel vm)
        {
            // Need to get the potParam from preset?? Defalt to 1st 2 Machine parameters
            if (pot.Id == 5)
            {
                HiddenParameters[preset.Param1ParameterIndex].AssignToDynamicPot(vm);
            }
            else
            {
                HiddenParameters[preset.Param2ParameterIndex].AssignToDynamicPot(vm);
            }
        }

        // Sets up the LCD ContextMenu
        private void SetupLCDMenu()
        {
            lcdMenu = NativeHooks.Current.CreateList<MenuItemViewModel>();
            var rename = new MenuItemViewModel { MenuText = "Rename", Command = RenameCommand, InputGestureText = "CTRL+R" };
            lcdMenu.Add(rename);
            string menuText = string.Format("Enter Exact {0} for {1} parameter.", Globals.IsBPMModeActive ? "BPM" : "MS", ActivePreset.ControlParameters[0].Name);
            var directEntry = new MenuItemViewModel { MenuText = menuText, Command = DirectFineEntryCommand, InputGestureText = "CTRL+D" };
            lcdMenu.Add(directEntry);
        }

        // Sets up the main menu
        private void SetupMenu()
        {
            editorMenu = NativeHooks.Current.CreateList<MenuItemViewModel>();
            // File Menu
            var fileMenu = new MenuItemViewModel { Children = FileMenu, MenuText = "File" };
            editorMenu.Add(fileMenu);

            // View menu 
            var viewMenu = new MenuItemViewModel { MenuText = "View", Children = ViewMenu };
            editorMenu.Add(viewMenu);

            // EPSet Menu
            var epSetMenu = new MenuItemViewModel { MenuText = "EP Set", Children = EPSetMenu };
            editorMenu.Add(epSetMenu);

            // EPSet Menu
            var optionsMenu = new MenuItemViewModel { MenuText = "Options", Children = OptionsMenu };
            editorMenu.Add(optionsMenu);

            // Tools Menu
            var toolsMenu = new MenuItemViewModel { MenuText = "Tools", Children = ToolsMenu }; 
            editorMenu.Add(toolsMenu);

        }

        private IList<MenuItemViewModel> fileMenu;
        private IList<MenuItemViewModel> FileMenu
        {
            get
            {
                if (fileMenu == null)
                {
                    fileMenu = NativeHooks.Current.CreateList<MenuItemViewModel>();
                    fileMenu.Add(new MenuItemViewModel { IsSeparator = true });
                    var loadXml = new MenuItemViewModel { MenuText = "Load Xml Preset", Command = LoadXmlCommand, InputGestureText = "SHIFT+L" };
                    fileMenu.Add(loadXml);
                    var loadSysEx = new MenuItemViewModel { MenuText = "Load .SYX Preset", Command = LoadSyxCommand, InputGestureText = "ALT+L" };
                    fileMenu.Add(loadSysEx);
                    var download = new MenuItemViewModel { MenuText = "Download", Command = DownloadCommand };
                    fileMenu.Add(download);
                    fileMenu.Add(new MenuItemViewModel { IsSeparator = true });
                    var save = new MenuItemViewModel { MenuText = "Save", Command = SaveCommand, InputGestureText = "CTRL+S" };
                    fileMenu.Add(save);
                    var saveXml = new MenuItemViewModel { MenuText = "Save As Xml", Command = SaveXmlCommand, InputGestureText = "SHIFT+S" };
                    fileMenu.Add(saveXml);
                    var saveSyx = new MenuItemViewModel { MenuText = "Save As .SYX", Command = SaveSyxCommand, InputGestureText = "ALT+N" };
                    fileMenu.Add(saveSyx);
                    var upload = new MenuItemViewModel { MenuText = "Upload", Command = UploadCommand};
                    fileMenu.Add(upload);
                    fileMenu.Add(new MenuItemViewModel { IsSeparator = true });
                    var exit = new MenuItemViewModel { MenuText = "Close", Command = CloseCommand, InputGestureText = "CTRL+X" };
                    fileMenu.Add(exit);
                }
                return fileMenu;
            }
        }

        private IList<MenuItemViewModel> viewMenu;
        private IList<MenuItemViewModel> ViewMenu
        {
            get
            {
                if (viewMenu == null)
                {
                    viewMenu = NativeHooks.Current.CreateList<MenuItemViewModel>();
                    var bpm = new MenuItemViewModel { MenuText = "BPM Mode", IsCheckable = true, Command = BPMModeCommand, InputGestureText = "CTRL+B" };
                    viewMenu.Add(bpm);

                }
                return viewMenu;
            }
        }

        private IList<MenuItemViewModel> toolsMenu;
        private IList<MenuItemViewModel> ToolsMenu
        {
            get
            {
                if (toolsMenu == null)
                {
                    toolsMenu = NativeHooks.Current.CreateList<MenuItemViewModel>();
                    var backup = new MenuItemViewModel { MenuText = "Pedal Backup", Command = PedalBackup };
                    toolsMenu.Add(backup);
                    var restore = new MenuItemViewModel { MenuText = "Restore Pedal Backup", Command = RestorePedalBackup };
                    toolsMenu.Add(restore);

                }
                return toolsMenu;
            }
        }

        private IList<MenuItemViewModel> optionsMenu;
        private IList<MenuItemViewModel> OptionsMenu
        {
            get
            {
                if (optionsMenu == null)
                {
                    optionsMenu = NativeHooks.Current.CreateList<MenuItemViewModel>();
                    var syncMode = new MenuItemViewModel { MenuText = "Synch Mode", Children = NativeHooks.Current.CreateList<MenuItemViewModel>() };
                    syncMode.Children.Add(new MenuItemViewModel { MenuText = "Two Way", IsCheckable = true, Command = SyncModeChanged, Tag=SyncMode.TwoWay });
                    syncMode.Children.Add(new MenuItemViewModel { MenuText = "Editor Master", IsCheckable = true, Command = SyncModeChanged, Tag = SyncMode.EditorMaster });
                    syncMode.Children.Add(new MenuItemViewModel { MenuText = "Pedal Master", IsCheckable = true, Command = SyncModeChanged, Tag = SyncMode.PedalMaster });
                    optionsMenu.Add(syncMode);

                    var lockMachine = new MenuItemViewModel { MenuText = "Lock Machine", IsCheckable = true, Command = LockMachineCommand};
                    optionsMenu.Add(lockMachine);
                }
                return optionsMenu;
            }
        }

        


        private IList<MenuItemViewModel> epSetMenu;
        private IList<MenuItemViewModel> EPSetMenu
        {
            get
            {
                if (epSetMenu == null)
                {
                    epSetMenu = NativeHooks.Current.CreateList<MenuItemViewModel>();
                    var epSet = new MenuItemViewModel { MenuText = "Start EP Set - Set Heel", Command = EPSetCommand, InputGestureText = "CTRL+E" };
                    epSetMenu.Add(epSet);
                    var epPreviewHeel = new MenuItemViewModel { MenuText = "Preview Heel Settings", IsCheckable = true, Command = PreviewEPSetHeelCommand, InputGestureText = "CTRL+H" };
                    epSetMenu.Add(epPreviewHeel);
                    var epPreviewToe = new MenuItemViewModel { MenuText = "Preview Toe Settings", IsCheckable = true, Command = PreviewEPSetToeCommand, InputGestureText = "CTRL+T" };
                    epSetMenu.Add(epPreviewToe);

                }
                return epSetMenu;
            }
        }

        // Helper that forces a full refresh of the view
        private void RefreshView()
        {
            midiManager.DisableControlChangeSends = true;
            _encoder = null;
            _hiddenParameters = null;
            _potControls = null;
            OnPropertyChanged("Encoder");
            OnPropertyChanged("HiddenParameters");
            OnPropertyChanged("PotControls");

        }




        // Delegate to determin if the parameter change is a 'trigger' and refresh the view if so
        private void HandleParameterChanged(object p)
        {
            IsDirty = true;
            Parameter param = p as Parameter;
            if (ActiveMachine._machine.Pots.Any(x => x.RangeOverrides.Any(r => r.TriggerParameter == param.Name)))
            {
                // Trigger parameter - refresh view to handle range overrides
                LoadNormalPot(ActivePreset.ControlParameters.First(), PotControls.First(x => x.IsCoarseControlPot));
            }
            // Avoid double sends
            if (!Globals.FineCoarseSynchroniser.synchInProgress) midiManager.SynchParameter(param);

        }

        private void ReceiveCCChange(object msg)
        {
            ControlChangeMsg ccMsg = msg as ControlChangeMsg;
            var affectedPotVM = PotControls.Where(x=>!x.Hide).FirstOrDefault(x => x.LinkedParameter.Definition.ControlChangeNo == ccMsg.ControlChangeNo);
            if (affectedPotVM != null)
            {
                affectedPotVM.Value = ccMsg.Value;
            }
            else
            {
                var affectedParamVM = HiddenParameters.FirstOrDefault(x => x.Definition.ControlChangeNo == ccMsg.ControlChangeNo);
                if (affectedParamVM != null) affectedParamVM.Value = ccMsg.Value;
            }

        }

        private void FetchPresetRequested(object index)
        {
            if (IsDirty)
            {
                if (!MessageDialog.ShowYesNo("There are unsaved edits, do you wish to fetch?", "Confirmation"))
                {
                    return;
                }
            }
            presetFromPedal = true;
            midiManager.FetchByIndex((int)index);
        }

        private void BulkLoadComplete(object index)
        {
            NativeHooks.Current.InvalidateRequerySuggested();
        }

        private void PushPresetRequested(object index)
        {
            if (!MessageDialog.ShowYesNo("This action will OVERWRITE the selected preset in the pedal, do you wish to proceed?", "Confirmation"))
            {
                return;
            }
            midiManager.PushToIndex(ActivePreset, (int)index);
        }

        private void PresetRenamed(object newName)
        {
            ActivePreset.Name = newName.ToString().ToUpperInvariant(); // Fix for caps
            LCDValue = ActivePreset.Name;
        }

        private void DirectEntryValueEntered(object fineValue)
        {
            var fe = Encoder as FinePotViewModel;
            fe.HandleDirectEntry(Convert.ToDouble(fineValue));
        }

        private void PushPresetFailed(object arg)
        {
            MessageDialog.ShowError("Preset Push Failed","Push Rejected");
        }

        

        // Delegate to update the LCD display from messaging
        private void LCDUpdate(object s)
        {
            IsDirty = true;
            string update = s.ToString();
            LCDValue = update;
        }

        // Delegate that sets the active machine
        private void MachineChanged(object m)
        {
            IsDirty = true;
            ActiveMachine = m as StrymonMachineViewModel;
        }

        private void PresetReceived(object p)
        {
            var preset = p as StrymonPreset;
            midiManager.DisableControlChangeSends=true;
            ActivePreset = preset;
        }
        private bool IsEdited
        {
            get
            {
                if (originalState.Name != ActivePreset.Name) return true;
                if (originalState.Machine != ActivePreset.Machine.Value) return true;
                if (originalState.Pedal != ActivePreset.Pedal.Id) return true;
                foreach (var p in originalState.Parameters)
                {
                    var ep = ActivePreset.AllParameters.FirstOrDefault(x => x.Name == p.Name);
                    if (p.Value != ep.Value) return true;
                }
                foreach (var ht in originalState.EPSet)
                {
                    var curHt = ActivePreset.EPSetValues.FirstOrDefault(x => x.PotId == ht.PotId);
                    if (curHt.HeelValue != ht.HeelValue) return true;
                    if (curHt.ToeValue != ht.ToeValue) return true;
                }
                return false;
            }
        }


        #endregion

        #region Commands
        /// <summary>
        /// ICommand to save the active preset
        /// </summary>
        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    FileIOService.SavePreset(ActivePreset);
                    IsDirty = false;
                }));
            }
        }

        /// <summary>
        /// ICommand to save the active preset to xml
        /// </summary>
        public RelayCommand SaveXmlCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    if (FileIOService.SavePresetToXml(ActivePreset))
                    {
                        IsDirty = false;
                    }
                }));
            }
        }

        /// <summary>
        /// ICommand to save the active preset to .Syx
        /// </summary>
        public RelayCommand SaveSyxCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    if (FileIOService.SavePresetToSyx(ActivePreset))
                    {
                        IsDirty = false;
                    }
                }));
            }
        }

        /// <summary>
        /// ICommand to load an Xml Preset
        /// </summary>
        public RelayCommand LoadXmlCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    if (IsDirty)
                    {
                        if (!MessageDialog.ShowYesNo("There are unsaved edits, do you wish to fetch?", "Confirmation"))
                        {
                            return;
                        }
                    }

                    var presetToLoad = FileIOService.LoadXmlPreset();
                    if (presetToLoad != null)
                    {
                        ActivePreset = presetToLoad;
                    }
                }));
            }
        }

        /// <summary>
        /// ICommand to load a .Syx preset
        /// </summary>
        public RelayCommand LoadSyxCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    if (IsDirty)
                    {
                        if (!MessageDialog.ShowYesNo("There are unsaved edits, do you wish to fetch?", "Confirmation"))
                        {
                            return;
                        }
                    }

                    var presetToLoad = FileIOService.LoadSyxPreset();
                    if (presetToLoad != null)
                    {
                        ActivePreset = presetToLoad;
                    }
                }));
            }
        }

        /// <summary>
        /// ICommand to close the editor
        /// </summary>
        public RelayCommand CloseCommand
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
        /// ICommand to change the BPM / Millisecond mode
        /// </summary>
        public RelayCommand<MenuItemViewModel> BPMModeCommand
        {
            get
            {
                return new RelayCommand<MenuItemViewModel>(new Action<MenuItemViewModel>(x =>
                {
                    ChangeViewMode(x);
                }));
            }
        }
        private void ChangeViewMode(MenuItemViewModel vm)
        {
            if (vm == null)
            {
                //Manual hack to deal with key command / menu binding issues
                vm = ViewMenu[0];
                vm.IsChecked = !vm.IsChecked;
            }
            Globals.IsBPMModeActive = vm.IsChecked;
            LCDUpdate(Encoder.ValueLabel);

        }

        private int epSetStage = 0;
        public RelayCommand<MenuItemViewModel> EPSetCommand
        {
            get
            {
                switch (epSetStage)
                {
                    case 1:
                        return StartEPSetToeCommand;
                    case 2:
                        return EPSetCompleteCommand;
                    default:
                        return StartEPSetHeelCommand;
                }
            }
        }

        /// <summary>
        /// ICommand that starts the EP Set 'wizard' - triggers the 'Heel' set
        /// </summary>
        public RelayCommand<MenuItemViewModel> StartEPSetHeelCommand
        {
            get
            {
                return new RelayCommand<MenuItemViewModel>(new Action<MenuItemViewModel>(x =>
                {
                    StartEPSet(x);
                }));
            }
        }
        private void StartEPSet(MenuItemViewModel vm)
        {
            //Manual hack to deal with key command / menu binding issues
            if (vm == null) { vm = EPSetMenu[0]; }
            epSetCachedState = ActivePreset.ToXmlPreset();
            EPSetMode = "EP SET : Setting Heel...";
            vm.MenuText = "EP Set - Step 2 - Set Toe";
            epSetStage = 1;
            OnPropertyChanged("EPSetCommand");
            vm.Command = EPSetCommand;
        }

        /// <summary>
        /// ICommand that starts the 'Toe' set of the EP Set parameter
        /// </summary>
        public RelayCommand<MenuItemViewModel> StartEPSetToeCommand
        {
            get
            {
                return new RelayCommand<MenuItemViewModel>(new Action<MenuItemViewModel>(x =>
                {
                    StartEPToe(x);
                }));
            }
        }
        private void StartEPToe(MenuItemViewModel vm)
        {
            if (vm == null) { vm = EPSetMenu[0]; }
            foreach (var ht in ActivePreset.EPSetValues)
            {
                ht.HeelValue = PotControls.FirstOrDefault(x => x.Id == ht.PotId).Value;
            }
            vm.MenuText = "Complete EP Set.";
            EPSetMode = "EP SET : Setting Toe...";
            epSetStage = 2;
            OnPropertyChanged("EPSetCommand");
            vm.Command = EPSetCommand;
        }

        /// <summary>
        /// ICommand that signals the EPSet process / wizard is complete
        /// </summary>
        public RelayCommand<MenuItemViewModel> EPSetCompleteCommand
        {
            get
            {
                return new RelayCommand<MenuItemViewModel>(new Action<MenuItemViewModel>(x =>
                {
                    EPSetComplete(x);
                }));
            }
        }
        private void EPSetComplete(MenuItemViewModel vm)
        {
            if (vm == null) { vm = EPSetMenu[0]; }
            foreach (var ht in ActivePreset.EPSetValues)
            {
                ht.ToeValue = PotControls.Single(x => x.Id == ht.PotId).Value;
            }
            EPSetMode = null;
            vm.MenuText = "Start EP Set (Set Heel)";
            foreach (var cp in PotControls.Where(x => !x.Hide))
            {
                var oldState = epSetCachedState.Parameters.Single(x => x.Name == cp.LinkedParameter.Name);
                cp.Value = oldState.Value;
            }
            epSetStage = 0;
            OnPropertyChanged("EPSetCommand");
            vm.Command = EPSetCommand;
        }

        /// <summary>
        /// ICommand that previews the Heel pot positions that are currently assigned 
        /// </summary>
        public RelayCommand<MenuItemViewModel> PreviewEPSetHeelCommand
        {
            get
            {
                return new RelayCommand<MenuItemViewModel>(new Action<MenuItemViewModel>(x =>
                {
                    PreviewHeel(x);
                }));
            }
        }
        private void PreviewHeel(MenuItemViewModel vm)
        {
            if (vm == null)
            {
                vm = EPSetMenu[1];
                vm.IsChecked = !vm.IsChecked;
            }
            if (vm.IsChecked)
            {
                var t = EPSetMenu[2];
                if (t.IsChecked)
                {
                    t.IsChecked = false;
                    RevertFromEPSetPreview(t);
                }
                epSetCachedState = ActivePreset.ToXmlPreset();
                EPSetMode = "EP SET : Previewing Heel...";
                foreach (var cp in PotControls.Where(x => !x.Hide))
                {
                    var heelState = ActivePreset.EPSetValues.FirstOrDefault(x => x.PotId == cp.Id);
                    cp.Value = heelState.HeelValue;
                }

            }
            else
            {
                RevertFromEPSetPreview(vm);
            }
        }

        /// <summary>
        /// ICommand that previews the Toe pot positions that are currently assigned 
        /// </summary>
        public RelayCommand<MenuItemViewModel> PreviewEPSetToeCommand
        {
            get
            {
                return new RelayCommand<MenuItemViewModel>(new Action<MenuItemViewModel>(x =>
                {
                    PreviewToe(x);

                }));
            }
        }
        private void PreviewToe(MenuItemViewModel vm)
        {
            if (vm == null)
            {
                vm = EPSetMenu[2];
                vm.IsChecked = !vm.IsChecked;
            }
            if (vm.IsChecked)
            {
                var h = EPSetMenu[1];
                if (h.IsChecked)
                {
                    h.IsChecked = false;
                    RevertFromEPSetPreview(h);
                }
                epSetCachedState = ActivePreset.ToXmlPreset();
                EPSetMode = "EP SET : Previewing Toe...";
                foreach (var cp in PotControls.Where(x => !x.Hide))
                {
                    var toeState = ActivePreset.EPSetValues.FirstOrDefault(x => x.PotId == cp.Id);
                    cp.Value = toeState.ToeValue;
                }

            }
            else
            {
                RevertFromEPSetPreview(vm);
            }
        }

        private void RevertFromEPSetPreview(MenuItemViewModel vm)
        {
            EPSetMode = null;
            foreach (var cp in PotControls.Where(x=>!x.Hide))
            {
                var oldState = epSetCachedState.Parameters.FirstOrDefault(x => x.Name == cp.LinkedParameter.Name);
                cp.Value = oldState.Value;
            }
        }

        public RelayCommand RenameCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    Rename();
                }));
            }
        }
        // Rename the currently active preset
        private void Rename()
        {
            RenameDialog.ShowModal();
            // Refresh name
            OnPropertyChanged("Title");
            LCDValue = ActivePreset.Name;
        }

        /// <summary>
        /// Trigger the fine entry function
        /// </summary>
        public RelayCommand DirectFineEntryCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    DirectEntry();
                }));
            }
        }
        // Rename the currently active preset
        private void DirectEntry()
        {
            DirectEntryDialog.ShowModal();
        }

        private IModalDialog directEntryDialog;
        
        /// <summary>
        /// Return the IModalDialog implementation for Direct Entry function
        /// </summary>
        public IModalDialog DirectEntryDialog
        {
            get 
            {
                if (directEntryDialog == null) return NativeHooks.Current.CreateDirectEntryDialog(ActivePreset.FineValue);
                return directEntryDialog;
            }
            set { directEntryDialog = value; }
        }

        private IModalDialog renameDialog;
        /// <summary>
        /// Return the IModalDialog implementation for rename function
        /// </summary>
        public IModalDialog RenameDialog
        {
            get
            {
                if (renameDialog == null) return NativeHooks.Current.CreatePresetRenameDialog(ActivePreset.Name);
                return renameDialog;
            }
            set { renameDialog = value; }
        }

        private IModalDialog progressDialog;
        /// <summary>
        /// Return the IModalDialog implementation for progress function
        /// </summary>
        public IModalDialog ProgressDialog
        {
            get
            {
                if (progressDialog == null)  progressDialog = NativeHooks.Current.CreateProgressBarDialog(ModalProgressVM);
                return progressDialog;
            }
            set
            {
                progressDialog = value;
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

        private IModalDialog uploadWindow;
        /// <summary>
        /// Seam to allow testing of functions that open the <see cref="PedalEditor"/>
        /// </summary>
        public IModalDialog UploadWindow
        {
            get
            {
                return uploadWindow;
            }
            set
            {
                uploadWindow = value;
            }
        }

        private ModalProgressDialogViewModel modalProgressVM;
        /// <summary>
        /// Return the Modal dialog Vm
        /// </summary>
        public ModalProgressDialogViewModel ModalProgressVM
        {
            get
            {
                if (modalProgressVM == null) modalProgressVM=new ModalProgressDialogViewModel(RestorePresets);
                return modalProgressVM;
            }
        }

        /// <summary>
        /// Looper Record command
        /// </summary>
        public RelayCommand LoopRecord
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    midiManager.SendLooperRecord();
                }));
            }
        }

        /// <summary>
        /// Looper Play command
        /// </summary>
        public RelayCommand LoopPlay
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    midiManager.SendLooperPlay();
                }));
            }
        }

        /// <summary>
        /// Looper Stop command
        /// </summary>
        public RelayCommand LoopStop
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    midiManager.SendLooperStop();
                }));
            }
        }

        /// <summary>
        /// Looper Undo command
        /// </summary>
        public RelayCommand LoopUndo
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    midiManager.SendLooperUndo();
                }));
            }
        }

        /// <summary>
        /// Looper Redo command
        /// </summary>
        public RelayCommand LoopRedo
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    midiManager.SendLooperRedo();
                }));
            }
        }

        /// <summary>
        /// Looper Reverse command
        /// </summary>
        public RelayCommand LoopReverse
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    midiManager.SendLooperReverse();
                }));
            }
        }

        /// <summary>
        /// Looper FullHalf command
        /// </summary>
        public RelayCommand LoopFullHalf
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    midiManager.SendLooperFullHalf();
                }));
            }
        }

        /// <summary>
        /// Looper PrePost command
        /// </summary>
        public RelayCommand LoopPrePost
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    midiManager.SendLooperPrePost();
                }));
            }
        }

        private int infValue = 127;
        /// <summary>
        /// Looper Toggle Infinite command
        /// </summary>
        public RelayCommand ToggleInfinite
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    midiManager.SendInfinite(infValue);
                    infValue = infValue == 127 ? 0 : 127;
                }));
            }
        }

        /// <summary>
        /// Command that requests a fetch of the current pedal edit buffer
        /// </summary>
        public RelayCommand FetchCurrent
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    if (IsDirty)
                    {
                        if (!MessageDialog.ShowYesNo("There are unsaved edits, do you wish to fetch?", "Confirmation"))
                        {
                            return;
                        }
                    }
                    NativeHooks.Current.SetBusy();
                    presetFromPedal = true;
                    midiManager.FetchCurrent();
                }));
            }
        }

        /// <summary>
        /// Command that sends the activs <see cref="StrymonPreset"/> to the context pedal edit buffer
        /// </summary>
        public RelayCommand SendToEdit
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    midiManager.PushToEdit(ActivePreset);
                }));
            }
        }

        private RelayCommand pedalBackup;
        /// <summary>
        /// Command to create .syx backup of all presets
        /// </summary>
        public RelayCommand PedalBackup
        {
            get
            {
                if (pedalBackup == null)
                {
                    pedalBackup = new RelayCommand
                    (
                        new Action(() =>
                        {
                            FileIOService.PedalBackupToSyx(ActivePedal);
                        }),
                        new Func<bool>(BackupOk));
                }
                return pedalBackup;
            }
        }
        private bool BackupOk()
        {
            return ActivePedal.RawPresetData.Count == ActivePedal.PresetCount;
        }

        private RelayCommand restorePedalBackup;
        /// <summary>
        /// Command to restore all presets on the pedal from .syx backup
        /// </summary>
        public RelayCommand RestorePedalBackup
        {
            get
            {
                if (restorePedalBackup == null)
                {
                    restorePedalBackup = new RelayCommand
                    (
                        new Action(() =>
                        {

                            ProgressDialog.ShowModal();
                            
                        }));
                }
                return restorePedalBackup;
            }
        }

        private void RestorePresets(object vm)
        {
            int index = 0;
            var pvm = vm as ModalProgressDialogViewModel;
            foreach (var presetData in FileIOService.GetPresetBackupDataFromSyx(ActivePedal))
            {
                midiManager.PushToIndex(presetData, index);
                NativeHooks.Current.Delay(NativeHooks.Current.BulkFetchDelay);
                index++;
                pvm.PBValue = index;
                pvm.PBStatus = string.Format("Restored : {0}", index);
            }
         
        }


        /// <summary>
        /// ICommand to change the BPM / Millisecond mode
        /// </summary>
        public RelayCommand<MenuItemViewModel> SyncModeChanged
        {
            get
            {
                return new RelayCommand<MenuItemViewModel>(new Action<MenuItemViewModel>(x =>
                {
                    ChangeSyncMode(x);
                }));
            }
        }
        private void ChangeSyncMode(MenuItemViewModel vm)
        {
            SyncMode = (SyncMode)vm.Tag;
            midiManager.SyncMode = SyncMode;
    
            foreach (var v in OptionsMenu[0].Children)
            {
                if (v.MenuText != vm.MenuText) v.IsChecked = !vm.IsChecked;
                v.IsEnabled = !v.IsChecked;
            }            
        }

        /// <summary>
        /// ICommand to change the BPM / Millisecond mode
        /// </summary>
        public RelayCommand<MenuItemViewModel> LockMachineCommand
        {
            get
            {
                return new RelayCommand<MenuItemViewModel>(new Action<MenuItemViewModel>(x =>
                {
                    LockMachine(x);
                }));
            }
        }
        private void LockMachine(MenuItemViewModel vm)
        {
            if (vm == null)
            {
                vm = OptionsMenu[1];
                vm.IsChecked = !vm.IsChecked;
            }
            Globals.MachineLocked = vm.IsChecked;
            Globals.LockedMachine = ActiveMachine.Value;
            // Refresh Fetch Preset
            FetchPreset.PresetIndex = FetchPreset.PresetIndex;
        }


        /// <summary>
        /// ICommand to save the active preset to .Syx
        /// </summary>
        public RelayCommand UploadCommand
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    UploadWindow = NativeHooks.Current.CreatePresetStoreUploadDialog(ActivePreset);
                    UploadWindow.ShowModal();
                }));
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
                    DownloadWindow = NativeHooks.Current.CreatePresetStoreDownloadDialog(false);
                    DownloadWindow.ShowModal();
                }));
            }
        }

        #endregion
    }

}
