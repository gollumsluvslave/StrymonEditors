using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            ConfiguredInputDevice = Properties.Settings.Default.MidiInDevice;
            ConfiguredOutputDevice = Properties.Settings.Default.MidiOutDevice;
            TimelineMidiChannel = Properties.Settings.Default.TimelineMidiChannel;
            MobiusMidiChannel = Properties.Settings.Default.MobiusMidiChannel;
            BigSkyMidiChannel = Properties.Settings.Default.BigSkyMidiChannel;
        }

        /// <summary>
        /// The currently configured MIDI input device
        /// </summary>
        public string ConfiguredInputDevice
        {
            get
            {
                return Properties.Settings.Default.MidiInDevice;
            }
            set
            {
                if (Properties.Settings.Default.MidiInDevice != value)
                {
                    midiInitRequired = true;
                    Properties.Settings.Default.MidiInDevice = value;
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
                return Properties.Settings.Default.MidiOutDevice;
            }
            set
            {
                if (Properties.Settings.Default.MidiOutDevice != value)
                {
                    midiInitRequired = true;
                    Properties.Settings.Default.MidiOutDevice = value;
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
                return Properties.Settings.Default.TimelineMidiChannel;
            }
            set
            {
                Properties.Settings.Default.TimelineMidiChannel = value;
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
                return Properties.Settings.Default.MobiusMidiChannel;
            }
            set
            {
                Properties.Settings.Default.MobiusMidiChannel = value;
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
                return Properties.Settings.Default.BigSkyMidiChannel;
            }
            set
            {
                Properties.Settings.Default.BigSkyMidiChannel = value;
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
                return Properties.Settings.Default.BulkFetchDelay;
            }
            set
            {
                Properties.Settings.Default.BulkFetchDelay = value;
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
                return Properties.Settings.Default.PushChunkSize;
            }
            set
            {
                Properties.Settings.Default.PushChunkSize = value;
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
                return Properties.Settings.Default.PushChunkDelay;
            }
            set
            {
                Properties.Settings.Default.PushChunkDelay = value;
                OnPropertyChanged("PushChunkDelay");
            }
        }



        /// <summary>
        /// Returns the available MIDI input devices
        /// </summary>
        public BindableCollection<string> InputDevices
        {
            get
            {
                var bc = new BindableCollection<string>();
                foreach (var x in Models.MidiDevices.GetInputDevices())
                {
                    bc.Add(x);
                }
                return bc;
            }
        }

        /// <summary>
        /// Returns the available MIDI output devices
        /// </summary>
        public BindableCollection<string> OutputDevices
        {
            get
            {
                var bc = new BindableCollection<string>();
                foreach (var x in Models.MidiDevices.GetOutputDevices())
                {
                    bc.Add(x);
                }
                return bc;
            }
        }

        /// <summary>
        /// Returns  the list of MIDI channels
        /// </summary>
        public BindableCollection<int> MidiChannels
        {
            get
            {
                var bc = new BindableCollection<int>();
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
        public BindableCollection<int> PushChunkSizes
        {
            get
            {
                var bc = new BindableCollection<int>();
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
        public BindableCollection<int> PushChunkDelays
        {
            get
            {
                var bc = new BindableCollection<int>();
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
        public BindableCollection<int> BulkFetchDelays
        {
            get
            {
                var bc = new BindableCollection<int>();
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
