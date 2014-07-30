using System;
using System.Collections.Generic;
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

    }
}
