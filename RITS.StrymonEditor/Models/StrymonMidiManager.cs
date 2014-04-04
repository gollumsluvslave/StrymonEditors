using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;

using Midi;
using RITS.StrymonEditor.Logging;
namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Strmon pedal specific wrapper for MIDI events and commands - currently supports
    /// 1. Identification of pedal(s) TODO: waiting from Strymon response regarding 3 pedals in MIDI Thru
    /// 2. Control Change sends and Control Change receipt for all 3 pedals. NB If the received CC is the exact same as the last send it will be ignored.
    /// 3. Various special CC sends with dedicated methods (e.g. Timeline Looper)
    /// 4. Fetch and Push of presets from/to the pedals
    /// </summary>
    public class StrymonMidiManager:IDisposable
    {
        #region private fields
        private List<StrymonPedal> connectedPedals = new List<StrymonPedal>();
        private StrymonPedal bulkPedal;
        private OutputDevice midiOut;
        private InputDevice midiIn;
        private ControlChangeMsg lastSentCC;
        private bool isBulkFetching;
        private ManualResetEvent bulkGate = new ManualResetEvent(true);       
        private int pcBank = 0;
        //private WaitOrTimerCallback 
        private SysExCommand lastSysExCommand;
        //private Queue<SysExCommand> sysExQueue;
        private readonly object lockObject = new object();
        private readonly object sysExLock = new object();
        #endregion
        
        public StrymonMidiManager()
        {
            //sysExQueue = new Queue<SysExCommand>();
            
        }

        private SysExCommand LastSysExCommand
        {
            get
            {
                lock (sysExLock)
                {
                    return lastSysExCommand;
                }
            }
            set
            {
                lock (sysExLock)
                {
                    lastSysExCommand = value; ;
                }
            }
        }

        public bool IsBulkFetching { get {return isBulkFetching;}}
        #region MIDI Connectivity

        private SyncMode syncMode;
        public SyncMode SyncMode 
        {
            get { return syncMode; }
            set
            {
                syncMode=value;

            }
        }
        public List<StrymonPedal> ConnectedPedals
        {
            get 
            {
                return connectedPedals;
            }
        }

        public StrymonPedal ContextPedal { get; set; }

        // Attempt to initialise midi for the context pedal
        public void InitMidi()
        {
            
            if (midiIn != null || midiOut != null)
            {
                Dispose(); // Cleane up if re-init call...
                Thread.Sleep(100);
            }
            using (RITSLogger logger = new RITSLogger())
            {
                midiOut = OutputDevice.InstalledDevices.FirstOrDefault(x => x.Name == Properties.Settings.Default.MidiOutDevice);
                if (midiOut == null) return; // No MIDI out device, return
                midiOut.Open();
                logger.Debug(string.Format("MIDI Out opened on {0}", Properties.Settings.Default.MidiOutDevice));
                midiIn = InputDevice.InstalledDevices.FirstOrDefault(x => x.Name == Properties.Settings.Default.MidiInDevice);
                if (midiIn == null) return; // No MIDI in device, return as can't identify pedal             
                midiIn.Open();
                midiIn.StartReceiving(null, true); // Receive with SysEx support
                midiIn.SysEx += new InputDevice.SysExHandler(ReceiveSysEx);
                midiIn.ControlChange += new InputDevice.ControlChangeHandler(ReceiveCC);
                midiIn.ProgramChange += new InputDevice.ProgramChangeHandler(ReceivePC);
                logger.Debug(string.Format("MIDI In opened on {0} and listening for messages...", Properties.Settings.Default.MidiInDevice));
                SendIdentifyRequest();
            }
        }


        /// <summary>
        /// Flag that instructs the class to NOT send CC messages while True
        /// </summary>
        public bool DisableControlChangeSends { get; set; }
        /// <summary>
        /// Returns whether or not a connection was established with the context pedal
        /// </summary>
        public bool IsConnected
        {

            get
            {
                if (ContextPedal == null) return false;
                return connectedPedals.Exists(x => x.Id == ContextPedal.Id); 
            }

        }
        #endregion

        #region Preset Fetch Support
        /// <summary>
        /// Initiates a request to retrieve the current Edit Buffer preset from the connected pedal
        /// </summary>
        public void FetchCurrent()
        {
            if (!IsConnected) return;
            if (isBulkFetching) return; 
            Thread.Sleep(Properties.Settings.Default.BulkFetchDelay);
            SendFetchPresetRequest(ContextPedal.PresetCount, false);
        }

        public void FetchByIndex(int index)
        {
            if (!IsConnected) return;
            if (isBulkFetching) return; 
            SendProgramChange(index); // Synch pedal 1st
            SendFetchPresetRequest(index,false);
        }

        public void BulkFetch(StrymonPedal pedal)
        {
            // TODO - how do we mark this to avoid the Editor getting it?
            // if we get another request whilst fetching the first, how do we queue it?
            if (isBulkFetching)
            {
                // TODO
            }
            isBulkFetching = true;
            bulkPedal = pedal;
            for (int i = 0; i < pedal.PresetCount; i++)
            {
                Thread.Sleep(Properties.Settings.Default.BulkFetchDelay);
                SendFetchPresetRequest(i, true);
            }
            // Give it a bit of time...
            Thread.Sleep(1000);
            isBulkFetching = false;
        }
        #endregion

        #region Preset Push Support
        /// <summary>
        /// Sends the current preset in the editor to the Edit buffer to allow saving
        /// </summary>
        /// <param name="preset"></param>
        public void PushToEdit(StrymonPreset preset)
        {
            if (!IsConnected) return;
            if (isBulkFetching) return; // TODO make this a bit more UI responsive?
            PushPreset(preset, ContextPedal.PresetCount);
        }

        /// <summary>
        /// Sends the current preset in the editor to the Edit buffer to allow saving
        /// </summary>
        /// <param name="preset"></param>
        public void PushToIndex(StrymonPreset preset, int index)
        {
            if (!IsConnected) return;
            if (isBulkFetching) return; // TODO make this a bit more UI responsive?
            PushPreset(preset, index);
        }
        #endregion

        #region Standard CC Support
        /// <summary>
        /// Sends CC Command to Synch the Type Encoder
        /// </summary>
        /// <param name="machine"></param>
        public void SynchMachine(StrymonMachine machine)
        {
            SendControlChange(StrymonPedal.TypeEncoderCC, machine.CCValue);
        }

        /// <summary>
        /// Send CC for the supplied Parameter
        /// </summary>
        /// <param name="parameter"></param>
        public void SynchParameter(Parameter parameter)
        {
            if (parameter.Definition.ControlChangeNo == 0) return;
            if (parameter.DirectEntryChange)
            {
                SendControlChange(parameter.Definition.ControlChangeNo, parameter.Value);
                // TODO in the vicinity, but need a number of individual changes to get the exact value
                var converter = new DefaultFineCoarseValueConverter(parameter.Definition);
                var approxValue = converter.CoarseToFine(parameter.Value);
                var diff = parameter.FineValue - approxValue;
                int cc = (diff > 0) ? 0 : 1;
                var ubound = Math.Abs(diff);
                for (int i = 0; i < ubound; i++)
                {
                    SendControlChange(StrymonPedal.ValueEncoderCC, cc);
                }
            }
            else if (parameter.FineEncoderLastChange) SendControlChange(StrymonPedal.ValueEncoderCC, parameter.FineEncoderCCValue);
            else
            {
                SendControlChange(parameter.Definition.ControlChangeNo, parameter.Value);
            }
        }
        #endregion

        #region Misc Public Support
        /// <summary>
        /// Expression CC Command
        /// </summary>
        /// <param name="value"></param>
        public void SendVirtualEP(int value)
        {
            SendControlChange(StrymonPedal.ExpressionCC, value);
        }

        /// <summary>
        /// Infinite Hold CC Command
        /// </summary>
        /// <param name="value"></param>
        public void SendInfinite(int value)
        {
            SendControlChange(StrymonPedal.HoldCC, value);
        }

        public void ResetDisplay()
        {
            var request = ResetDisplayRequest.ToArray();
                // update with pedal id
            request[5] = Convert.ToByte(GetPedalId);
            SendSysEx(new SysExCommand("ResetDisplay", request, 100, null, null));
        }
#endregion

        #region Looper CC Support
        /// <summary>
        /// Looper Record CC Command
        /// </summary>
        public void SendLooperRecord()
        {
            if (ContextPedal.Name != StrymonPedal.Timeline_Name) return;
            SendControlChange(StrymonPedal.LooperRecord, 1);
        }

        /// <summary>
        /// Looper Play CC Command
        /// </summary>
        public void SendLooperPlay()
        {
            if (ContextPedal.Name != StrymonPedal.Timeline_Name) return;
            SendControlChange(StrymonPedal.LooperPlay, 1);
        }

        /// <summary>
        /// Looper Stop CC Command
        /// </summary>
        public void SendLooperStop()
        {
            if (ContextPedal.Name != StrymonPedal.Timeline_Name) return;
            SendControlChange(StrymonPedal.LooperStop, 1);
        }

        /// <summary>
        /// Looper Undo CC Command
        /// </summary>
        public void SendLooperUndo()
        {
            if (ContextPedal.Name != StrymonPedal.Timeline_Name) return;
            SendControlChange(StrymonPedal.LooperUndo, 1);
        }

        /// <summary>
        /// Looper Redo CC Command
        /// </summary>
        public void SendLooperRedo()
        {
            if (ContextPedal.Name != StrymonPedal.Timeline_Name) return;
            SendControlChange(StrymonPedal.LooperRedo, 1);
        }

        /// <summary>
        /// Looper Reverse CC Command
        /// </summary>
        public void SendLooperReverse()
        {
            if (ContextPedal.Name != StrymonPedal.Timeline_Name) return;
            SendControlChange(StrymonPedal.LooperReverse, 1);
        }

        /// <summary>
        /// Looper Half/Full speed CC Command
        /// </summary>
        public void SendLooperFullHalf()
        {
            if (ContextPedal.Name != StrymonPedal.Timeline_Name) return;
            SendControlChange(StrymonPedal.LooperFullHalf, 1);
        }

        /// <summary>
        /// Looper Pre/Post CC Command
        /// </summary>
        public void SendLooperPrePost()
        {
            if (ContextPedal.Name != StrymonPedal.Timeline_Name) return;
            SendControlChange(StrymonPedal.LooperPrePost, 1);
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// Dispose unmanaged and managed resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            // no unmanaged resources if MIDI library is doing it's work ok
            // TODO - check midiIn and midiOut and see if they need an IDisposable impl
            if (disposing)
            {
                if (midiOut != null) midiOut.Close();
                if (midiIn != null)
                {
                    midiIn.SysEx -= ReceiveSysEx;
                    if (SyncMode != SyncMode.EditorMaster)
                    {
                        midiIn.ControlChange -= ReceiveCC;
                        midiIn.ProgramChange -= ReceivePC;
                    }
                    midiIn.StopReceiving();
                    midiIn.Close();
                }
            }
        }
        #endregion

        private void CloseBulkGate()
        {
            WaitForCurrentSysExCompletion();
            Thread.Sleep(100);
            bulkGate.Reset();
        }

        private void OpenBulkGate()
        {
            WaitForCurrentSysExCompletion();
            Thread.Sleep(100);
            bulkGate.Set();
        }

        #region Private Receive Callbacks
        private void ReceiveCC(ControlChangeMessage msg)
        {
            if (!IsConnected) return;
            if (SyncMode == SyncMode.EditorMaster) return;
            if (ReceivedCCIsEchoOfLastSent(msg)) return;            
            // Temporarily disable control change sends to avoid infinite loop
            DisableControlChangeSends = true;
            ViewModels.ViewModelBase.mediatorInstance.NotifyColleagues(ViewModelMessages.ReceivedCC, new ControlChangeMsg { ControlChangeNo = msg.Control, Value = msg.Value });
            DisableControlChangeSends = false;
            lastSentCC = null; // Can no longer be an echo
        }
        
        private void ReceivePC(ProgramChangeMessage msg)
        {
            // TODO
            if (!IsConnected) return;
            if (IsBulkFetching) return;
            if (SyncMode ==SyncMode.EditorMaster) return;

            // Need to fetch current buffer
            FetchCurrent();
        }

        private bool ReceivedCCIsEchoOfLastSent(ControlChangeMessage msg)
        {
            return lastSentCC != null && lastSentCC.ControlChangeNo == msg.Control && lastSentCC.Value == msg.Value;
        }

        private void ReceiveSendAck(byte[] data)
        {
            if (data.Reverse().Skip(1).First() != 69)
            {
                System.Windows.MessageBox.Show("Preset Write Failed...");
            }
        }

        private void ReceiveIdentifyResponse(byte[] data)
        {
            lock (lockObject)
            {
                if (lastSysExCommand.TimedOut) return; // ??
                if (data[7] != 85) return;
                if (data[8] != 18) return;
                if (data[9] != 0) return;
                var pedal = StrymonPedal.GetPedalById(data[10]);
                if (pedal == null) return;
                lastSysExCommand.Retrigger();

                connectedPedals.Add(pedal);
                ThreadPool.QueueUserWorkItem(QueuePedalConnected, pedal);
            }
        }

        private void ReceivePresetResponse(byte[] data)
        {
            if (!isBulkFetching && !IsConnected) return;
            lock (lockObject)
            {
                if (data.Length == 8 && data[6] == 71)
                {
                    //logger.Debug("Preset Fetch failed...");
                    System.Windows.MessageBox.Show("Preset Fetch Failed...");
                    return;
                }
                var preset = StrymonSysExUtils.FromSysExData(data);
                // Choice here? Is it a bulk fetch, or a normal fetch?
                if (!lastSysExCommand.IsBulkFetch)
                {
                    // Queue back to editor
                    ThreadPool.QueueUserWorkItem(QueuePresetReceipt, preset);
                }
                else
                {
                    // Assumption is responses will come in order!
                    bulkPedal.UpdatePresetInfo(preset);
                    bulkPedal.UpdatePresetRawData(preset.SourceIndex, data);
                    ThreadPool.QueueUserWorkItem(QueueBulkPresetUpdate, preset);
                }
                
                lastSysExCommand.Completed();
                
            }
        }

        // Actual callback that receives all SysEx messages from the MIDI library
        // Checks for echo then passes on to the callback delegate in the active SysExCommand
        private void ReceiveSysEx(SysExMessage msg)
        {
            byte[] data = msg.Data;
            // Echo of request? If same as request then ignore...from Strymon github code, John has encountered this as well
            // thinks it might be a midi-merge default??
            lock (lockObject)
            {
                if (data.SequenceEqual(lastSysExCommand.Data)) return;
                if (lastSysExCommand.ResponseCallback == null) return;
                lastSysExCommand.ResponseCallback(data);                
            }
        }
        #endregion

        #region Private Send Push
        private void SendIdentifyRequest()
        {
            SendSysEx(new SysExCommand("IdentifyRequest", 
                                            IdentifyRequest, 
                                            1000, 
                                            ReceiveIdentifyResponse, 
                                            IdentityProcessComplete));
        }

        private void PushPreset(StrymonPreset preset, int index)
        {
            if (SyncMode == SyncMode.PedalMaster) return;
            using (RITSLogger logger = new RITSLogger())
            {
                var data = StrymonSysExUtils.ToSysExData(preset);
                UpdatePresetReadrequestWithPresetNo(data, index);
                var sysEx = new SysExCommand("Push", data, 2000, ReceiveSendAck, null);
                logger.Debug(string.Format("Push Command: {0}", BitConverter.ToString(sysEx.Data)));
                SendSysEx(sysEx);
            }
        }


        private void SendFetchPresetRequest(int presetIndex,bool isBulk)
        {
            using (RITSLogger logger = new RITSLogger())
            {
                var request = PresetReadRequest.ToArray();
                // update with pedal id
                request[5] = Convert.ToByte(GetPedalId);
                UpdatePresetReadrequestWithPresetNo(request, presetIndex);
                var sysEx = new SysExCommand("Fetch", request, 2000, ReceivePresetResponse, null, isBulk);
                logger.Debug(string.Format("FetchPreset Command: {0}", BitConverter.ToString(sysEx.Data)));
                SendSysEx(sysEx);
            }
            
        }

        private void SendSysEx(SysExCommand cmd)
        {
            lock (lockObject)
            {
                WaitForCurrentSysExCompletion();
                midiOut.SendSysEx(cmd.Data);
                lastSysExCommand = cmd;
                lastSysExCommand.Dispatched();
            }
        }

        private void WaitForCurrentSysExCompletion()
        {
            if (lastSysExCommand != null)
            {
                while (lastSysExCommand.IsProcessing)
                {
                    // Don't send new SysEx until timedout or complete
                }
            }
        }

        private void SendControlChange(int ccNo, int value)
        {
            using (RITSLogger logger = new RITSLogger())
            {
                if (!IsConnected) return;
                if (SyncMode == SyncMode.PedalMaster) return;
                if (DisableControlChangeSends) return;
                logger.Debug(string.Format("Sending CC{0}, Value{1}, Channel{2}",ccNo,value,ContextPedal.MidiChannel));
                Channel chan = FromInt(ContextPedal.MidiChannel - 1);
                lastSentCC = new ControlChangeMsg { ControlChangeNo = ccNo, Value = value };
                midiOut.SendControlChange(chan, ccNo, value);
            }
        }

        private void SendProgramChange(int pcNo)
        {
            using (RITSLogger logger = new RITSLogger())
            {
                if (!IsConnected) return;
                if (SyncMode == SyncMode.PedalMaster) return;
                if (DisableControlChangeSends) return;
                logger.Debug(string.Format("Sending PC{0}, Channel{1}", pcNo, ContextPedal.MidiChannel));
                Channel chan = FromInt(ContextPedal.MidiChannel - 1);
                // TODO for prsets above 127 - need bank change cc
                int newPC = pcNo;
                if (pcNo >= 128 && pcBank == 0)
                {
                    pcBank = 1;
                    newPC = pcNo % 128;
                }
                else if (pcNo >= 256 && pcBank == 1)
                {
                    pcBank = 2;
                    newPC = pcNo % 256;
                }
                else
                {
                    pcBank = 0;
                }
                midiOut.SendControlChange(chan, 0, pcBank);
                Instrument inst = FromIntPC(newPC);
                midiOut.SendProgramChange(chan, inst);
            }
        }
        #endregion

        #region Private ThreadPool Methods
        private void QueueBulkPresetUpdate(object preset)
        {
            ViewModels.ViewModelBase.mediatorInstance.NotifyColleagues(ViewModelMessages.BulkPresetRead, preset);
        }

        private void QueuePresetReceipt(object preset)
        {
            ViewModels.ViewModelBase.mediatorInstance.NotifyColleagues(ViewModelMessages.ReceivedPresetFromPedal, preset);
        }

        private void QueuePedalConnected(object pedal)
        {
            ViewModels.ViewModelBase.mediatorInstance.NotifyColleagues(ViewModelMessages.PedalConnected, pedal);
        }
        private void QueueMIDIConnectionComplete(object pedal)
        {
            ViewModels.ViewModelBase.mediatorInstance.NotifyColleagues(ViewModelMessages.MIDIConnectionComplete, null);
        }

        #endregion

        #region Misc Private
        private void IdentityProcessComplete()
        {
            lastSysExCommand.Complete = true;
            QueueMIDIConnectionComplete(null);
        }

        // Helper to update a byte array with the correct preset number at the correct offset
        private void UpdatePresetReadrequestWithPresetNo(byte[] request, int preset)
        {
            // TODO Constant the offset numbers
            int byte1 = preset / 128;
            int byte2 = preset % 128;
            request[7] = Convert.ToByte(byte1);
            request[8] = Convert.ToByte(byte2);
        }

        // Helper to get the pedal id from the relevant pedal
        private byte GetPedalId
        {
            get
            {
                if (isBulkFetching) { return Convert.ToByte(bulkPedal.Id); }
                else
                {
                    return Convert.ToByte(ContextPedal.Id);
                }
            }
        }
        
        // Helper to concert int into annoying MIDI library enums - ideally fix MIDI library
        private Channel FromInt(int channel)
        {
            return (Channel)Enum.Parse(typeof(Channel), channel.ToString());
        }

        // Helper to concert int into annoying MIDI library enums - ideally fix MIDI library
        private Instrument FromIntPC(int pcNo)
        {
            return (Instrument)Enum.Parse(typeof(Instrument), pcNo.ToString());
        }

        #endregion

        /// <summary>
        /// Internal SysEx class to assist - deals with timeouts and callbacks
        /// </summary>
        internal class SysExCommand
        {

            public SysExCommand(string name, byte[] data, int timeout,
                                 Action<byte[]> responseCallback, Action timeoutCallback, bool isBulk = false)
            {
                Name = name;
                Data = data;
                IsBulkFetch = isBulk;
                ResponseCallback = responseCallback;
                TimeoutCallback = timeoutCallback;
                if (ResponseCallback != null)
                {
                    timer = new System.Timers.Timer(timeout);
                }
            }
            private System.Timers.Timer timer;
            public Action<byte[]> ResponseCallback { get; private set; }
            public Action TimeoutCallback { get; private set; }
            public string Name { get; private set; }
            public bool TimedOut { get; private set; }
            public byte[] Data { get; private set; }
            public bool Complete { get; set; }
            public bool IsBulkFetch { get; set; }
            public bool IsProcessing
            {
                get 
                {
                    if (timer == null) return false;
                    return timer.Enabled; 
                }
            }

            private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                StopTimer();
                TimedOut = true;
                if (TimeoutCallback != null) TimeoutCallback();
            }

            public void Dispatched()
            {
                if (ResponseCallback == null) return;
                StartTimer();
            }

            public void Retrigger()
            {
                if (ResponseCallback == null) return;
                StopTimer();
                StartTimer();
            }

            public void Completed()
            {
                Complete = true;
                StopTimer();
            }

            private void StartTimer()
            {
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
            }
            private void StopTimer()
            {
                timer.Stop();
                timer.Elapsed -= Timer_Elapsed;
            }
        }

        #region Static SysEx Data
        private static byte[] identifyRequest = new byte[] { 0xF0, 0x7E, 0x7F, 0x06, 0x01, 0xF7 };
        public static byte[] IdentifyRequest
        {
            get
            {
                return identifyRequest;
            }
        }

        private static byte[] presetReadRequest = new byte[] { 0xF0, 0x00, 0x01, 0x55, 0x12, 0x00, 0x63, 0x00, 0x00, 0xF7 };
        public static byte[] PresetReadRequest
        {
            get
            {
                return presetReadRequest;
            }
        }
        private static byte[] resetDisplayRequest = new byte[] { 0xF0, 0x00, 0x01, 0x55, 0x12, 0x00, 0x26, 0xF7 };
        public static byte[] ResetDisplayRequest
        {
            get
            {
                return resetDisplayRequest;
            }
        }
        #endregion

    }
}
