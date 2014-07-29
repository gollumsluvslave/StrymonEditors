using System;
using System.Collections.Generic;
namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Interface that defines the MIDI operations supported in the editor
    /// </summary>
    public interface IStrymonMidiManager :IDisposable
    {
        #region Connectivity

        /// <summary>
        /// Operation that initialises / reinitilaises the MIDI setup
        /// </summary>
        void InitMidi();

        /// <summary>
        /// Returns the list of <see cref="StrymonPedal"/> that have been detected after midi init
        /// </summary>
        List<StrymonPedal> ConnectedPedals { get; }

        /// <summary>
        /// Specifies / returns the <see cref="StrymonPedal"/> relevant for BulkFetch oepration
        /// </summary>
        StrymonPedal BulkPedal { get; set; }

        /// <summary>
        /// Specifies / returns the <see cref="StrymonPedal"/> relevant for normal operations (non-bulk)
        /// </summary>
        StrymonPedal ContextPedal { get; set; }

        /// <summary>
        /// Returns whether or not a connection was established with the context pedal.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Returns whether or not the system is currently involved in a 'bulk' fetch operation
        /// </summary>
        bool IsBulkFetching { get; }

        #endregion

        #region Control Flags
        /// <summary>
        /// Specifies / returns the current <see cref="SyncMode"/> 
        /// </summary>
        SyncMode SyncMode { get; set; }

        /// <summary>
        /// Specifies whether or not to prohibit the sending of CC messages
        /// </summary>
        bool DisableControlChangeSends { get; set; }
        #endregion

        #region Control Change
        
        /// <summary>
        /// This method will synchronise connected pedal with the supplied <see cref="StrymonMachine"/>
        /// </summary>
        /// <param name="machine"></param>
        void SynchMachine(StrymonMachine machine);

        /// <summary>
        /// This method will synchronise connected pedal with the supplied <see cref="Parameter"/>
        /// </summary>
        /// <param name="parameter"></param>
        void SynchParameter(Parameter parameter);
        
        /// <summary>
        /// This method will send/toggle the Hold/Infinite CC message
        /// </summary>
        /// <param name="value"></param>
        void SendInfinite(int value);

        /// <summary>
        /// This method will send an updated value for the Expression Pedal CC message
        /// </summary>
        /// <param name="value"></param>
        void SendVirtualEP(int value);

        /// <summary>
        /// This method will send the Timeline looper Record CC message
        /// </summary>
        void SendLooperRecord();

        /// <summary>
        /// This method will send the Timeline looper Play CC message
        /// </summary>
        void SendLooperPlay();

        /// <summary>
        /// This method will send the Timeline looper Stop CC message
        /// </summary>
        void SendLooperStop();

        /// <summary>
        /// This method will send the Timeline looper Undo CC message
        /// </summary>
        void SendLooperUndo();

        /// <summary>
        /// This method will send the Timeline looper Redo CC message
        /// </summary>
        void SendLooperRedo();

        /// <summary>
        /// This method will send/toggle the Timeline looper Reverse CC message
        /// </summary>
        void SendLooperReverse();

        /// <summary>
        /// This method will send the Timeline looper full/half speed CC message
        /// </summary>
        void SendLooperFullHalf();

        /// <summary>
        /// This method will send the Timeline looper pre/post CC message
        /// </summary>
        void SendLooperPrePost();


        
        #endregion

        #region Fetch SysEx Requests
        
        /// <summary>
        /// Instruct a fetch request to be sent that retreives the current pedal Edit Buffer
        /// </summary>
        void FetchCurrent();

        /// <summary>
        /// Instruct a fetch request to be sent that retreives the pedal preset index
        /// </summary>
        void FetchByIndex(int index);
        #endregion

        #region Push SysEx Requests
        /// <summary>
        /// Push the supplied <see cref="StrymonPreset"/> to the edit buffer of the context <see cref="StrymonPedal"/>
        /// </summary>
        /// <param name="preset"></param>
        void PushToEdit(StrymonPreset preset);

        /// <summary>
        /// Push the supplied <see cref="StrymonPreset"/> to the specified preset index of the context <see cref="StrymonPedal"/>
        /// </summary>
        void PushToIndex(StrymonPreset preset, int index);

        void UpdateDisplay();
        #endregion
    }
}
