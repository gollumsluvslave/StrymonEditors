using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Midi;
namespace RITS.StrymonEditor.Models
{
    public static class MidiDevices
    {

        public static IEnumerable<string> GetInputDevices()
        {
            foreach (var id in InputDevice.InstalledDevices)
            {
                yield return id.Name;
            }
        }
        public static IEnumerable<string> GetOutputDevices()
        {
            foreach (var id in OutputDevice.InstalledDevices)
            {
                yield return id.Name;
            }
        }

        public static InputDevice ConfiguredInputDevice
        {
            get
            {
                return InputDevice.InstalledDevices.FirstOrDefault(x => x.Name == Properties.Settings.Default.MidiInDevice);
            }
        }

        public static OutputDevice ConfiguredOutputDevice
        {
            get
            {
                return OutputDevice.InstalledDevices.FirstOrDefault(x => x.Name == Properties.Settings.Default.MidiOutDevice);
            }
        }
    }
}
