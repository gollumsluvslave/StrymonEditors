using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RITS.StrymonEditor.AutoUpdate;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.IO;
using RITS.StrymonEditor.MIDI;
using RITS.StrymonEditor.Logging;
namespace RITS.StrymonEditor
{
    /// <summary>
    /// Abstract class to allow native functionality to be injected into the PCL
    /// </summary>
    public abstract class NativeHooks
    {
        private static NativeHooks _hooks;

        public static NativeHooks Current
        {
            get
            {
                if (_hooks == null)
                {
                    throw new InvalidOperationException();
                }
                return _hooks;
            }
            set
            {
                _hooks = value;
            }
        }

        protected NativeHooks()
        {
            Current = this;
        }

        public virtual IList<T> CreateList<T>()
        {
            return new List<T>();
        }

        public abstract IStrymonMidiManager CreateMIDIManager();

        public abstract ILogger CreateLogger();

        public abstract IFileDialog CreateSaveDialog();

        public abstract IFileDialog CreateOpenDialog();

        public abstract IFileIOService CreateFileIOService();

        public abstract IModalDialog CreatePresetStoreDownloadDialog(bool fromMainWindow);

        public abstract IModalDialog CreatePresetStoreUploadDialog(StrymonPreset preset);

        public abstract IMessageDialog CreateMessageDialog();

        public abstract IModalDialog CreateDirectEntryDialog(string fineValue);

        public abstract IModalDialog CreatePresetRenameDialog(string name);

        public abstract IAutoUpdater CreateAutoUpdater();

        public abstract IModalDialog CreatePedalEditorWindow(StrymonPreset preset, IStrymonMidiManager midiManager);

        public abstract void Delay(int delay);

        public abstract void SetBusy();
        public abstract void WorkComplete();

        public abstract void InvalidateRequerySuggested();

        
        public abstract string PedalImage(string pedalName);


        public virtual string VersionInfo { get; set; }
        public virtual bool BPMMode { get; set; }
        public virtual string SyncMode { get; set; }
        public virtual bool DisableBulkFetch { get; set; }
        public virtual int PushChunkSize { get; set; }
        public virtual int PushChunkDelay { get; set; }
        public virtual int BulkFetchDelay { get; set; }
        public virtual string MIDIInDevice { get; set; }
        public virtual string MIDIOutDevice { get; set; }

        public abstract IList<string> MIDIInDevices { get; }
        public abstract IList<string> MIDIOutDevices { get; }

        public virtual int TimelineMIDIChannel { get; set; }
        public virtual int BigsSkyMIDIChannel { get; set; }
        public virtual int MobiusMIDIChannel { get; set; }
    }
}
