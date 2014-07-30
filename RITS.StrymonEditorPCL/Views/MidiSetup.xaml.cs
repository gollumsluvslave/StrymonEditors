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
using RITS.StrymonEditor.MIDI;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;
namespace RITS.StrymonEditor.Views
{
    /// <summary>
    /// Modal MIDI Setup window
    /// </summary>
    public partial class MidiSetup : Window
    {
        private IStrymonMidiManager midiManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="midiManager"></param>
        public MidiSetup(IStrymonMidiManager midiManager)
        {
            this.midiManager = midiManager;
            InitializeComponent();
        }
        
        private MidiSetupViewModel viewModel;
        /// <summary>
        /// Exposes the <see cref="MidiSetupViewModel"/> viewmodel for databinding
        /// </summary>
        public MidiSetupViewModel ViewModel
        {
            get
            {
                if(viewModel == null) viewModel = new MidiSetupViewModel(this.midiManager, this.Close);
                return viewModel;
            }
        }        
    }
}
