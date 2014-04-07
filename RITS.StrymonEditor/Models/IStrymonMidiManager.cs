using System;
namespace RITS.StrymonEditor.Models
{
    public interface IStrymonMidiManager
    {
        StrymonPedal BulkPedal { get; set; }
        System.Collections.Generic.List<StrymonPedal> ConnectedPedals { get; }
        StrymonPedal ContextPedal { get; set; }
        bool DisableControlChangeSends { get; set; }
        void Dispose();
        void FetchByIndex(int index);
        void FetchCurrent();
        void InitMidi();
        bool IsBulkFetching { get; }
        bool IsConnected { get; }
        void PushToEdit(StrymonPreset preset);
        void PushToIndex(StrymonPreset preset, int index);
        void SendInfinite(int value);
        void SendLooperFullHalf();
        void SendLooperPlay();
        void SendLooperPrePost();
        void SendLooperRecord();
        void SendLooperRedo();
        void SendLooperReverse();
        void SendLooperStop();
        void SendLooperUndo();
        void SendVirtualEP(int value);
        void SynchMachine(StrymonMachine machine);
        void SynchParameter(Parameter parameter);
        RITS.StrymonEditor.SyncMode SyncMode { get; set; }
    }
}
