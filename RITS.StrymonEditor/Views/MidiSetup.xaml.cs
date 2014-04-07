using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;
namespace RITS.StrymonEditor.Views
{
    /// <summary>
    /// Basic MIDI Setup form - very simple, no real need for a VM
    /// </summary>
    public partial class MidiSetup : Window
    {
        private IStrymonMidiManager midiManager;
        public MidiSetup(IStrymonMidiManager midiManager)
        {
            this.midiManager = midiManager;
            InitializeComponent();
            In.SelectedValue = Properties.Settings.Default.MidiInDevice;
            Out.SelectedValue = Properties.Settings.Default.MidiOutDevice;
            TimelineMidiChannel.SelectedValue = Properties.Settings.Default.TimelineMidiChannel;
            MobiusMidiChannel.SelectedValue = Properties.Settings.Default.MobiusMidiChannel;
            BigSkyMidiChannel.SelectedValue = Properties.Settings.Default.BigSkyMidiChannel;
        }

        public BindableCollection<string> InputDevices
        {
            get
            {   
                var bc =new  BindableCollection<string>();
                foreach (var x in Models.MidiDevices.GetInputDevices())
                {
                    bc.Add(x);
                }
                return bc;
            }
        }
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
       
        public BindableCollection<int> MidiChannels
        {
            get
            {
                var bc = new BindableCollection<int>();
                for(int c =1;c<17;c++)
                {
                    bc.Add(c);
                }
                return bc;
            }
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (In.Text != null) { Properties.Settings.Default.MidiInDevice = In.Text; }
            if (Out.Text != null) { Properties.Settings.Default.MidiOutDevice = Out.Text; }
            int mc =0;
            if(int.TryParse(TimelineMidiChannel.SelectedValue.ToString(),out mc))
            {
                Properties.Settings.Default.TimelineMidiChannel = mc;
            }
            if (int.TryParse(MobiusMidiChannel.SelectedValue.ToString(), out mc))
            {
                Properties.Settings.Default.MobiusMidiChannel = mc;
            }
            if (int.TryParse(BigSkyMidiChannel.SelectedValue.ToString(), out mc))
            {
                Properties.Settings.Default.BigSkyMidiChannel = mc;
            }
            // Re-init MIDI
            midiManager.InitMidi();
            this.Close();
        }
    }
}
