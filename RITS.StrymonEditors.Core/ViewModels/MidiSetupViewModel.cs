using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RITS.StrymonEditor.MIDI;
using RITS.StrymonEditor.Commands;
using RITS.StrymonEditor.Models;

namespace RITS.StrymonEditor.ViewModels
{
    /// <summary>
    /// ViewModel responsible for the <see cref="Views.MidiSetup"/> view
    /// </summary>
    public class MidiSetupViewModel:ViewModelBase
    {
        private IStrymonMidiManager midiManager;
        private bool midiInitRequired;
        private Action closeAction;

        /// <summary>
        /// Default .ctor
        /// </summary>
        /// <param name="midiManager"></param>
        /// <param name="close"></param>
        public MidiSetupViewModel(IStrymonMidiManager midiManager, Action close)
        {
            this.midiManager = midiManager;
            closeAction = close;
            ConfiguredInputDevice = NativeHooks.Current.MIDIInDevice;
            ConfiguredOutputDevice = NativeHooks.Current.MIDIOutDevice;
            TimelineMidiChannel = NativeHooks.Current.TimelineMIDIChannel;
            MobiusMidiChannel = NativeHooks.Current.MobiusMIDIChannel;
            BigSkyMidiChannel = NativeHooks.Current.BigsSkyMIDIChannel;
        }

        /// <summary>
        /// The currently configured MIDI input device
        /// </summary>
        public string ConfiguredInputDevice
        {
            get
            {
                return NativeHooks.Current.MIDIInDevice;
            }
            set
            {
                if (NativeHooks.Current.MIDIInDevice != value)
                {
                    midiInitRequired = true;
                    NativeHooks.Current.MIDIInDevice = value;
                    OnPropertyChanged("ConfiguredInputDevice");
                }
            }

        }

        /// <summary>
        /// The currently configured MIDI input device
        /// </summary>
        public string ConfiguredOutputDevice
        {
            get
            {
                return NativeHooks.Current.MIDIOutDevice;
            }
            set
            {
                if (NativeHooks.Current.MIDIOutDevice != value)
                {
                    midiInitRequired = true;
                    NativeHooks.Current.MIDIOutDevice = value;
                    OnPropertyChanged("ConfiguredOutputDevice");
                }
            }

        }

        /// <summary>
        /// The currently configured MIDI channel for the Timeline
        /// </summary>
        public int TimelineMidiChannel
        {
            get
            {
                return NativeHooks.Current.TimelineMIDIChannel;
            }
            set
            {
                NativeHooks.Current.TimelineMIDIChannel = value;
                OnPropertyChanged("TimelineMidiChannel");
            }
        }

        /// <summary>
        /// The currently configured MIDI channel for the Mobius
        /// </summary>
        public int MobiusMidiChannel
        {
            get
            {
                return NativeHooks.Current.MobiusMIDIChannel;
            }
            set
            {
                NativeHooks.Current.MobiusMIDIChannel = value;
                OnPropertyChanged("MobiusMidiChannel");
            }
        }

        /// <summary>
        /// The currently configured MIDI channel for the BigSky
        /// </summary>
        public int BigSkyMidiChannel
        {
            get
            {
                return NativeHooks.Current.BigsSkyMIDIChannel;
            }
            set
            {
                NativeHooks.Current.BigsSkyMIDIChannel = value;
                OnPropertyChanged("BigSkyMidiChannel");
            }
        }

        /// <summary>
        /// The configurable delay between bulk fetch operations
        /// </summary>
        public int BulkFetchDelay
        {
            get
            {
                return NativeHooks.Current.BulkFetchDelay;
            }
            set
            {
                NativeHooks.Current.BulkFetchDelay = value;
                OnPropertyChanged("BulkFetchDelay");
            }
        }

        /// <summary>
        /// The configurable chunk size for push operations
        /// </summary>
        public int PushChunkSize
        {
            get
            {
                return NativeHooks.Current.PushChunkSize;
            }
            set
            {
                NativeHooks.Current.PushChunkSize = value;
                OnPropertyChanged("PushChunkSize");
            }
        }


        /// <summary>
        /// The configurable delay between chunked push operations
        /// </summary>
        public int PushChunkDelay
        {
            get
            {
                return NativeHooks.Current.PushChunkDelay;
            }
            set
            {
                NativeHooks.Current.PushChunkDelay = value;
                OnPropertyChanged("PushChunkDelay");
            }
        }



        /// <summary>
        /// Returns the available MIDI input devices
        /// </summary>
        public IList<string> InputDevices
        {
            get
            {
                return NativeHooks.Current.MIDIInDevices;
            }
        }

        /// <summary>
        /// Returns the available MIDI output devices
        /// </summary>
        public IList<string> OutputDevices
        {
            get
            {
                return NativeHooks.Current.MIDIOutDevices;
            }
        }

        /// <summary>
        /// Returns  the list of MIDI channels
        /// </summary>
        public IList<int> MidiChannels
        {
            get
            {
                var bc = NativeHooks.Current.CreateList<int>();
                for (int c = 1; c < 17; c++)
                {
                    bc.Add(c);
                }
                return bc;
            }
        }

        /// <summary>
        /// Returns the list of supported chunk sizes
        /// </summary>
        public IList<int> PushChunkSizes
        {
            get
            {
                var bc = NativeHooks.Current.CreateList<int>();
                bc.Add(0);
                bc.Add(1);
                bc.Add(5);
                bc.Add(10);
                bc.Add(50);
                bc.Add(65);
                bc.Add(130);
                bc.Add(325);

                return bc;
            }
        }

        /// <summary>
        /// Returns the list of supported chunk delays
        /// </summary>
        public IList<int> PushChunkDelays
        {
            get
            {
                var bc = new List<int>();
                for (int i = 0; i < 50; i++)
                {
                    bc.Add(i);
                }
                return bc;
            }
        }

        /// <summary>
        /// Returns the list of supported fetch delays
        /// </summary>
        public IList<int> BulkFetchDelays
        {
            get
            {
                var bc = new List<int>();
                bc.Add(0);
                bc.Add(10);
                bc.Add(50);
                bc.Add(100);
                bc.Add(200);
                bc.Add(300);
                bc.Add(400);
                bc.Add(500);
                return bc;
            }
        }


        /// <summary>
        /// Command to be executed when setup is complete
        /// </summary>
        public RelayCommand OKCommand
        {
            get
            {
                return new RelayCommand(Done);
            }
        }

        private void Done()
        {
            if (midiInitRequired)
            {
                midiManager.InitMidi();
            }
            closeAction();
        }
    }
}
