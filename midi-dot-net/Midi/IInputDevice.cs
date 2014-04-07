using System;
namespace Midi
{
    /// <summary>
    /// Interface for output device to enable testing 
    /// </summary>
    public interface IInputDevice
    {
        void Close();
        event InputDevice.ControlChangeHandler ControlChange;
        bool IsOpen { get; }
        bool IsReceiving { get; }
        event InputDevice.NoteOffHandler NoteOff;
        event InputDevice.NoteOnHandler NoteOn;
        void Open();
        event InputDevice.PitchBendHandler PitchBend;
        event InputDevice.ProgramChangeHandler ProgramChange;
        void RemoveAllEventHandlers();
        void StartReceiving(Clock clock);
        void StartReceiving(Clock clock, bool handleSysEx);
        void StopReceiving();
        event InputDevice.SysExHandler SysEx;
    }
}
