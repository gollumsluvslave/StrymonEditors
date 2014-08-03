using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;

using RITS.StrymonEditor.AutoUpdate;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.IO;
using RITS.StrymonEditor.MIDI;
using RITS.StrymonEditor.Logging;
using RITS.StrymonEditor.ViewModels;

namespace RITS.StrymonEditor
{
    public class WPFNativeHooks: NativeHooks
    {
        public override IList<T> CreateList<T>()
        {
            return new ViewModels.BindableCollection<T>();
        }

        public override IStrymonMidiManager CreateMIDIManager()
        {
            return new StrymonMidiManager(MidiDevices.ConfiguredInputDevice, MidiDevices.ConfiguredOutputDevice) as IStrymonMidiManager;
        }

        public override ILogger CreateLogger()
        {
            return new RITSLogger() as ILogger;
        }

        public override IFileDialog CreateSaveDialog()
        {
            return new FileDialogSave() as IFileDialog;
        }

        public override IFileDialog CreateOpenDialog()
        {
            return new FileDialogOpen() as IFileDialog;
        }

        public override IFileIOService CreateFileIOService()
        {
            return new FileIOService(CreateOpenDialog(), CreateSaveDialog(), CreateMessageDialog()) as IFileIOService;
        }

        public override IModalDialog CreatePresetStoreDownloadDialog(bool fromMainWindow)
        {
            return new Views.PresetStoreDialog(null, fromMainWindow) as IModalDialog;
        }

        public override IModalDialog CreatePresetStoreUploadDialog(StrymonPreset preset)
        {
            return new Views.PresetStoreDialog(preset, false) as IModalDialog;
        }

        public override IMessageDialog CreateMessageDialog()
        {
            return new MessageDialog(); 
        }

        public override IModalDialog CreateDirectEntryDialog(string fineValue)
        {
            return new Views.DirectEntryDialog(fineValue) as IModalDialog;
        }

        public override IModalDialog CreatePresetRenameDialog(string name)
        {
            return new Views.PresetRenameDialog(name) as IModalDialog;
        }

        public override IAutoUpdater CreateAutoUpdater()
        {
            return new UpdateChecker(CreateMessageDialog());
        }

        public override IModalDialog CreatePedalEditorWindow(StrymonPreset preset, IStrymonMidiManager midiManager)
        {
            return (IModalDialog)new Views.PedalEditorWindow(preset, midiManager);
        }

        public override IModalDialog CreateProgressBarDialog(ModalProgressDialogViewModel progressVM)
        {
            return (IModalDialog)new Views.ModalProgressBar(progressVM);
        }

        public override void Delay(int delay)
        {
            Thread.Sleep(delay);
        }

        public override void SetBusy()
        {
            Mouse.OverrideCursor = Cursors.Wait;
        }
        public override void WorkComplete()
        {
            Mouse.OverrideCursor = null;
        }

        public override void AddCanExecuteRequerySuggested(EventHandler canExecuteChanged)
        {
            CommandManager.RequerySuggested += canExecuteChanged;
        }

        public override void RemoveCanExecuteRequerySuggested(EventHandler canExecuteChanged)
        {
            CommandManager.RequerySuggested -= canExecuteChanged;
        }

        public override void InvalidateRequerySuggested()
        {
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

        public override string PedalImage(string pedalName)
        {
            return string.Format(@"pack://application:,,,/Views/Images/{0}.png", pedalName);
        }


        public override IList<string> MIDIInDevices
        {
            get
            {
                return MidiDevices.GetInputDevices().ToList();
            }
        }
        public override IList<string> MIDIOutDevices 
        {
            get
            {
                return MidiDevices.GetOutputDevices().ToList();
            }
        }




        public override void DoWork(Action<object> work, object arg, Action onComplete)
        {
            if (!Thread.CurrentThread.IsBackground)
            {
                var worker = new BackgroundWorker();
                worker.DoWork += (sender, e) => work(arg);
                worker.RunWorkerCompleted += (sender, e) => onComplete();
                worker.RunWorkerAsync();
            }
            else
            {
                work(arg);
            }

        }

        public override int TimelineMIDIChannel 
        { 
            get
            { 
                return Properties.Settings.Default.TimelineMidiChannel;
            }
            set
            {
                Properties.Settings.Default.TimelineMidiChannel=value;
            }
        }
        public override int BigsSkyMIDIChannel 
        { 
            get
            { 
                return Properties.Settings.Default.BigSkyMidiChannel;
            }
            set
            {
                Properties.Settings.Default.BigSkyMidiChannel=value;
            }
        }
        public override int MobiusMIDIChannel 
        { 
            get
            { 
                return Properties.Settings.Default.MobiusMidiChannel;
            }
            set
            {
                Properties.Settings.Default.MobiusMidiChannel=value;
            }
        }
        
        public override string VersionInfo 
        { 
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();}
        }
        public override bool BPMMode
        {
            get
            {
                return Properties.Settings.Default.BPMMode;
            }
            set
            {
                Properties.Settings.Default.BPMMode = value;
            }
        }
        public override string SyncMode
        {
            get
            {
                return Properties.Settings.Default.SyncMode;
            }
            set
            {
                Properties.Settings.Default.SyncMode = value;
            }
        }
        public override bool DisableBulkFetch
        {
            get
            {
                return Properties.Settings.Default.DisableBulkFetch;
            }
            set
            {
                Properties.Settings.Default.DisableBulkFetch = value;
            }
        }
        public override int PushChunkSize
        {
            get
            {
                return Properties.Settings.Default.PushChunkSize;
            }
            set
            {
                Properties.Settings.Default.PushChunkSize = value;
            }
        }
        public override int PushChunkDelay
        {
            get
            {
                return Properties.Settings.Default.PushChunkDelay;
            }
            set
            {
                Properties.Settings.Default.PushChunkDelay = value;
            }
        }
        public override int BulkFetchDelay
        {
            get
            {
                return Properties.Settings.Default.BulkFetchDelay;
            }
            set
            {
                Properties.Settings.Default.BulkFetchDelay = value;
            }
        }
        public override string MIDIInDevice
        {
            get
            {
                return Properties.Settings.Default.MidiInDevice;
            }
            set
            {
                Properties.Settings.Default.MidiInDevice = value;
            }
        }
        public override string MIDIOutDevice
        {
            get
            {
                return Properties.Settings.Default.MidiOutDevice;
            }
            set
            {
                Properties.Settings.Default.MidiOutDevice = value;
            }
        }

    }
}
