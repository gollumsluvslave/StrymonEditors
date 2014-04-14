using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;

using Midi;
using RITS.StrymonEditor.Conversion;
using RITS.StrymonEditor.ViewModels;
using RITS.StrymonEditor.Logging;
using RITS.StrymonEditor.Messaging;

namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Strmon pedal specific wrapper for MIDI events and commands - currently supports
    /// 1. Identification of pedal(s) TODO: waiting from Strymon response regarding 3 pedals in MIDI Thru
    /// 2. Control Change sends and Control Change receipt for all 3 pedals. NB If the received CC is the exact same as the last send it will be ignored.
    /// 3. Various special CC sends with dedicated methods (e.g. Timeline Looper)
    /// 4. Fetch and Push of presets from/to the pedals
    /// </summary>
    public class StrymonMidiManager: ViewModelBase, IDisposable, IStrymonMidiManager
    {
        #region private fields
        private IOutputDevice midiOut;
        private IInputDevice midiIn;
        private ControlChangeMsg lastSentCC;
        private int pcBank = 0;
        //private WaitOrTimerCallback 
        //private Queue<SysExCommand> sysExQueue;
        private readonly object lockObject = new object();
        private readonly object sysExLock = new object();
        #endregion
        
        public StrymonMidiManager(IInputDevice inputDevice, IOutputDevice outputDevice)
        {
            using (RITSLogger logger = new RITSLogger())
            {
                logger.Debug(string.Format("Setting midiIn device *{0}*", midiIn));
                midiIn = inputDevice;
                logger.Debug(string.Format("Setting midiOut device *{0}*", midiOut));
                midiOut = outputDevice;
            }
        }

        #region Connectivity

        /// <summary>
        /// Operation that initialises / reinitilaises the MIDI setup
        /// </summary>
        public void InitMidi()
        {
            using (RITSLogger logger = new RITSLogger())
            {
                if (midiIn == null || midiOut == null)
                {
                    logger.Debug("No midi devices supplied. Midi cannot initialise.");
                    return;
                }
                midiOut.Open();
                logger.Debug(string.Format("MIDI Out opened on {0}", Properties.Settings.Default.MidiOutDevice));
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
        /// Returns the list of <see cref="StrymonPedal"/> that have been detected after midi init
        /// </summary>
        private List<StrymonPedal> connectedPedals = new List<StrymonPedal>();
        public List<StrymonPedal> ConnectedPedals
        {
            get
            {
                lock (lockObject)
                {
                    return connectedPedals;
                }
            }
        }

        /// <summary>
        /// Specifies / returns the <see cref="StrymonPedal"/> relevant for BulkFetch oepration
        /// NB setting this value will also set the IsBulkFetching flag to true
        /// </summary>
        private StrymonPedal bulkPedal;
        public StrymonPedal BulkPedal
        {
            get { return bulkPedal; }
            set
            {
                bulkPedal = value;
            }
        }

        /// <summary>
        /// Specifies / returns the <see cref="StrymonPedal"/> relevant for normal operations (non-bulk)
        /// </summary>
        private StrymonPedal contextPedal;
        public StrymonPedal ContextPedal 
        {
            get { if (IsBulkFetching)return BulkPedal; return contextPedal; }
            set
            {
                contextPedal = value;
            }
        }
        
        /// <summary>
        /// Returns whether or not a connection was established with the context pedal
        /// </summary>
        public bool IsConnected
        {

            get
            {
                if (ContextPedal == null) return false;
                return ConnectedPedals.Exists(x => x.Id == ContextPedal.Id); 
            }

        }

        /// <summary>
        /// Returns whether or not the system is currently involved in a 'bulk' fetch operation
        /// </summary>
        public bool IsBulkFetching
        { get { return bulkPedal != null; } }

        #endregion

        #region Control Flags

        /// <summary>
        /// Specifies / returns the current <see cref="SyncMode"/> 
        /// </summary>
        private SyncMode syncMode;
        public SyncMode SyncMode
        {
            get { return syncMode; }
            set
            {
                syncMode = value;

            }
        }

        /// <summary>
        /// Specifies whether or not to prohibit the sending of CC messages
        /// </summary>
        public bool DisableControlChangeSends { get; set; }
        #endregion

        #region Control Change

        /// <summary>
        /// This method will synchronise connected pedal with the supplied <see cref="StrymonMachine"/>
        /// </summary>
        /// <param name="machine"></param>
        public void SynchMachine(StrymonMachine machine)
        {
            SendControlChange(StrymonPedal.TypeEncoderCC, machine.CCValue);
        }

        /// <summary>
        /// This method will synchronise connected pedal with the supplied <see cref="Parameter"/>
        /// </summary>
        /// <param name="parameter"></param>
        public void SynchParameter(Parameter parameter)
        {
            if (parameter.Definition.ControlChangeNo == 0) return;
            if (parameter.DirectEntryChange)
            {
                SendControlChange(parameter.Definition.ControlChangeNo, parameter.Value);
                // TODO in the vicinity, but need a number of individual changes to get the exact value
                var converter = FineCoarseValueConverterFactory.Create(parameter.Definition);
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

        #region Misc CC Support
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

        #region Preset Fetch Support
        /// <summary>
        /// Initiates a request to retrieve the current Edit Buffer preset from the connected pedal
        /// </summary>
        public void FetchCurrent()
        {
            if (!IsConnected) return;
            if (IsBulkFetching) return;
            Thread.Sleep(Properties.Settings.Default.BulkFetchDelay);
            SendFetchPresetRequest(ContextPedal.PresetCount);
        }

        public void FetchByIndex(int index)
        {
            if (!IsConnected && !IsBulkFetching) return;
            if (!IsBulkFetching)
            {
                SendProgramChange(index);
            }
            SendFetchPresetRequest(index);
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
            if (IsBulkFetching) return; // TODO make this a bit more UI responsive?
            PushPreset(preset, ContextPedal.PresetCount);
        }

        /// <summary>
        /// Sends the current preset in the editor to the Edit buffer to allow saving
        /// </summary>
        public void PushToIndex(StrymonPreset preset, int index)
        {
            if (!IsConnected) return;
            if (IsBulkFetching) return; // TODO make this a bit more UI responsive?
            PushPreset(preset, index);
        }
        #endregion


        #region IDisposable
        /// <summary>
        /// Dispose unmanaged and managed resources
        /// </summary>
        public override void Dispose()
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


        #region Private Receive Callbacks
        private void ReceiveCC(ControlChangeMessage msg)
        {
            if (!IsConnected) return;
            if (SyncMode == SyncMode.EditorMaster) return;
            if (ReceivedCCIsEchoOfLastSent(msg)) return;            
            // Temporarily disable control change sends to avoid infinite loop
            DisableControlChangeSends = true;
            Mediator.NotifyColleagues(ViewModelMessages.ReceivedCC, new ControlChangeMsg { ControlChangeNo = msg.Control, Value = msg.Value });
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
            using (RITSLogger logger = new RITSLogger())
            {
                logger.Debug(string.Format("Push Response: {0}", BitConverter.ToString(data)));
                try
                {
                    if (data.Reverse().Skip(1).First() != 69)
                    {
                        logger.Debug("Push NACK received - device rejected write.");
                        ThreadPool.QueueUserWorkItem(QueuePushPresetFailed, null);
                    }
                    else
                    {
                        logger.Debug("Push ACK received - device write success.");
                    }

                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    ThreadPool.QueueUserWorkItem(QueuePushPresetFailed, null);
                }
                finally
                {
                    LastSysExCommand.Completed();
                }
            }
        }

        private void ReceiveIdentifyResponse(byte[] data)
        {
            using (RITSLogger logger = new RITSLogger())
            {
                try
                {
                    if (LastSysExCommand.TimedOut) return; // ??
                    var testData = data.Skip(7).Take(3);
                    if (!testData.SequenceEqual(new byte[] { 0x55, 0x12, 0x00 })) return;
                    var pedal = StrymonPedal.GetPedalById(data[10]);
                    if (pedal == null) return;
                    LastSysExCommand.Retrigger();
                    ConnectedPedals.Add(pedal);
                    ThreadPool.QueueUserWorkItem(QueuePedalConnected, pedal);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
        }

        private void ReceivePresetResponse(byte[] data)
        {
            using (RITSLogger logger = new RITSLogger())
            {
                try
                {
                    if (!IsBulkFetching && !IsConnected) return;
                    if (data.Length != 650)
                    {
                        //logger.Debug("Preset Fetch failed...");
                        throw new ArgumentOutOfRangeException("Preset not recieved - invalid length.");
                    }
                    var preset = StrymonSysExUtils.FromSysExData(data);
                    // Choice here? Is it a bulk fetch, or a normal fetch?
                    if (!LastSysExCommand.IsBulkFetch)
                    {
                        // Queue back to editor
                        ThreadPool.QueueUserWorkItem(QueuePresetReceipt, preset);
                    }
                    else
                    {
                        // Assumption is responses will come in order!
                        BulkPedal.UpdatePresetRawData(preset, data);
                        ThreadPool.QueueUserWorkItem(QueueBulkPresetUpdate, preset);
                    }                    
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    if (LastSysExCommand.IsBulkFetch)
                    {
                        ThreadPool.QueueUserWorkItem(QueueBulkPresetUpdate, null);
                    }
                }
                finally
                {
                    LastSysExCommand.Completed();
                }
            }    
        }


        // Deal with a 'chunked' send mecahnism to split the preset into smaller chunks with a delay between
        private void HandleChunkedSend(byte[] presetData)
        {            
            // TODO - unclear whether a straight byte split is sufficient here or whether a OxF7 start byte is needed??
            var chunkCount = presetData.Length / Properties.Settings.Default.PushChunkSize;
            var chunks = presetData.Chunkify(Properties.Settings.Default.PushChunkSize);
            // Create the chunked queue
            int chunkIndex = 0;
            // Populate chunked queue
            foreach (var chunk in chunks)
            {
                // Add SysEx continuation marker
                //var chunkData = new byte[] { 0xF7 }.Union(chunk).ToArray();
                var chunkData = chunk.ToArray();
                chunkIndex++;
                if (chunkIndex == chunkCount)
                {
                    // terminating chunk, specify 'final nack' callbacks
                    SendSysEx(new SysExCommand("PushChunk", chunkData, 2000, ReceiveSendAck, PushPresetTimeout));
                }
                else
                {
                    midiOut.SendSysEx(chunkData);
                }
                Thread.Sleep(Properties.Settings.Default.PushChunkDelay);
            }
        }

        private SysExCommand lastSysExCommand;
        private SysExCommand LastSysExCommand
        {
            get
            {
                lock (lockObject)
                {
                    return lastSysExCommand;
                }
            }
            set
            {
                lock (lockObject)
                {
                    lastSysExCommand = value;
                }
            }
        }

        // Actual callback that receives all SysEx messages from the MIDI library
        // Checks for echo then passes on to the callback delegate in the active SysExCommand
        private void ReceiveSysEx(SysExMessage msg)
        {
            byte[] data = msg.Data;
            // Echo of request? If same as request then ignore...from Strymon github code, John has encountered this as well
            // thinks it might be a midi-merge default??
            if (data.SequenceEqual(LastSysExCommand.Data)) return;
            if (LastSysExCommand.ResponseCallback == null) return;
            LastSysExCommand.ResponseCallback(data);                
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
                var sysEx = new SysExCommand("Push", data, 2000, ReceiveSendAck, PushPresetTimeout);
                logger.Debug(string.Format("Push Command: {0}", BitConverter.ToString(sysEx.Data)));

                if (SendChunked)
                {
                    HandleChunkedSend(data);
                }
                else
                {
                    SendSysEx(sysEx);
                }
            }
        }

        private bool SendChunked
        {
            get
            {
                return ((Properties.Settings.Default.PushChunkDelay > 0) && (Properties.Settings.Default.PushChunkSize > 0));
            }
        }

        private void PushPresetTimeout()
        {
            using (RITSLogger logger = new RITSLogger())
            {
                ThreadPool.QueueUserWorkItem(QueuePushPresetFailed, null);
            }
        }

        private void SendFetchPresetRequest(int presetIndex)
        {
            using (RITSLogger logger = new RITSLogger())
            {
                var request = PresetReadRequest.ToArray();
                // update with pedal id
                request[5] = Convert.ToByte(GetPedalId);
                UpdatePresetReadrequestWithPresetNo(request, presetIndex);
                var sysEx = new SysExCommand("Fetch", request, 2000, ReceivePresetResponse, null, IsBulkFetching);
                logger.Debug(string.Format("FetchPreset Command: {0}", BitConverter.ToString(sysEx.Data)));
                SendSysEx(sysEx);
            }
            
        }

        private void SendSysEx(SysExCommand cmd)
        {
            WaitForCurrentSysExCompletion();
            LastSysExCommand = cmd;
            midiOut.SendSysEx(LastSysExCommand.Data);
            LastSysExCommand.Dispatched();
        }

        private void WaitForCurrentSysExCompletion()
        {
            if (LastSysExCommand != null)
            {
                while (LastSysExCommand.IsProcessing)
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
                if (SyncMode == SyncMode.PedalMaster) return;
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

        #region Private ThreadPool Mediator Methods
        // Queue notification that a BulkPreset has been read
        private void QueueBulkPresetUpdate(object preset)
        {
            Mediator.NotifyColleagues(ViewModelMessages.BulkPresetRead, preset);
        }

        // Queue notification that a normal (non-bulk) Preset has been received
        private void QueuePresetReceipt(object preset)
        {
            Mediator.NotifyColleagues(ViewModelMessages.ReceivedPresetFromPedal, preset);
        }

        // Queue notification that a pedal has been successfully connected
        private void QueuePedalConnected(object pedal)
        {
            Mediator.NotifyColleagues(ViewModelMessages.PedalConnected, pedal);
        }

        // Queue notification that the midi conection process is complete
        private void QueueMIDIConnectionComplete(object pedal)
        {
            Mediator.NotifyColleagues(ViewModelMessages.MIDIConnectionComplete, null);
        }

        // Queue notification that the push of a preset failed
        private void QueuePushPresetFailed(object pedal)
        {
            Mediator.NotifyColleagues(ViewModelMessages.PushPresetFailed, null);
        }

        #endregion

        #region Misc Private
        private void IdentityProcessComplete()
        {
            LastSysExCommand.Complete = true;
            QueueMIDIConnectionComplete(null);
        }

        // Helper to update a byte array with the correct preset number at the correct offset
        private void UpdatePresetReadrequestWithPresetNo(byte[] request, int preset)
        {
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
                if (IsBulkFetching) { return Convert.ToByte(BulkPedal.Id); }
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
        /// SysEx class to assist - deals with timeouts and callbacks
        /// and sysex message payload
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

            /// <summary>
            /// Callback to invoke when the SysEx response is received
            /// </summary>
            public Action<byte[]> ResponseCallback { get; private set; }

            /// <summary>
            /// Callback to invoke when the SysEx response has timed out
            /// </summary>
            public Action TimeoutCallback { get; private set; }

            /// <summary>
            /// The naem of this command
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            ///  A flag that indicates the command has timed out
            /// </summary>
            public bool TimedOut { get; private set; }

            /// <summary>
            /// The sysex payload / data
            /// </summary>
            public byte[] Data { get; private set; }

            /// <summary>
            /// Flag that indicates whether this command is complete
            /// </summary>
            public bool Complete { get; set; }
            public bool IsBulkFetch { get; set; }

            /// <summary>
            /// Returns whether or not the command is currently processing
            /// </summary>
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

            /// <summary>
            /// Indicates that this command has been sent and 
            /// to start the callback timer if applicable
            /// </summary>
            public void Dispatched()
            {
                if (ResponseCallback == null) return;
                StartTimer();
            }

            /// <summary>
            /// For multiple messages (e.g. identity) this will 
            /// stop and start the timer
            /// </summary>
            public void Retrigger()
            {
                if (ResponseCallback == null) return;
                StopTimer();
                StartTimer();
            }

            /// <summary>
            /// Indicates the response was recieved successfully 
            /// and the command is complete
            /// </summary>
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
        #endregion

    }
}
