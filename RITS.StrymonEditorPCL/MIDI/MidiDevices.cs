using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Midi;
namespace RITS.StrymonEditor.MIDI
{
    /// <summary>
    /// Static helper class that returns various MIDI Device related information
    /// </summary>
    public static class MidiDevices
    {
        /// <summary>
        /// Enumerates the list of <see cref="InputDevice"/> supported by the current system, and returns the names
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetInputDevices()
        {
            foreach (var id in InputDevice.InstalledDevices)
            {
                yield return id.Name;
            }
        }

        /// <summary>
        /// Enumerates the list of <see cref="OutputDevice"/> supported by the current system, and returns the names
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetOutputDevices()
        {
            foreach (var id in OutputDevice.InstalledDevices)
            {
                yield return id.Name;
            }
        }

        /// <summary>
        /// Returns the currently configured <see cref="InputDevice"/> based on the users settings
        /// </summary>
        public static InputDevice ConfiguredInputDevice
        {
            get
            {
                return InputDevice.InstalledDevices.FirstOrDefault(x => x.Name == Properties.Settings.Default.MidiInDevice);
            }
        }

        /// <summary>
        /// Returns the currently configured <see cref="OutputDevice"/> based on the users settings
        /// </summary>
        public static OutputDevice ConfiguredOutputDevice
        {
            get
            {
                return OutputDevice.InstalledDevices.FirstOrDefault(x => x.Name == Properties.Settings.Default.MidiOutDevice);
            }
        }
    }
}
