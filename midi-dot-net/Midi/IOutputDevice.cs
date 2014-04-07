using System;
namespace Midi
{
    /// <summary>
    /// Interface for output device to enable testing 
    /// </summary>
    public interface IOutputDevice
    {
        void Close();
        bool IsOpen { get; }
        void Open();
        void SendControlChange(Channel channel, Control control, int value);
        void SendControlChange(Channel channel, int control, int value);
        void SendNoteOff(Channel channel, Pitch pitch, int velocity);
        void SendNoteOn(Channel channel, Pitch pitch, int velocity);
        void SendPercussion(Percussion percussion, int velocity);
        void SendPitchBend(Channel channel, int value);
        void SendProgramChange(Channel channel, Instrument instrument);
        void SendSysEx(byte[] data);
        void SilenceAllNotes();
    }
}
