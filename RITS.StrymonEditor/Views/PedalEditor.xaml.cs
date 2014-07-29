using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;
using RITS.StrymonEditor.Messaging;
namespace RITS.StrymonEditor.Views
{
    /// <summary>
    /// The editor window - hosting modal window for the <see cref="EditorView"/>
    /// </summary>
    public partial class PedalEditor : Window, INotifyPropertyChanged
    {
        private StrymonPreset editingPreset;
        private IStrymonMidiManager midiManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="preset"></param>
        /// <param name="midiManager"></param>
        public PedalEditor(StrymonPreset preset, IStrymonMidiManager midiManager)
        {
            editingPreset = preset;
            this.midiManager = midiManager;
            InitializeComponent();                        
        }

        #region INotify
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private StrymonPedalViewModel _pedalViewModel;
        /// <summary>
        /// Exposes the main <see cref="StrymonPedalViewModel"/> viewmodel for databinding
        /// </summary>
        public StrymonPedalViewModel PedalViewModel
        {
            get
            {
                if (_pedalViewModel == null) 
                {
                    _pedalViewModel = new StrymonPedalViewModel(editingPreset, midiManager);
                    _pedalViewModel.CloseWindow = this.Close;
                }
                
                return _pedalViewModel;
            }
            set
            {
                _pedalViewModel.Dispose();
                _pedalViewModel = null;
                if (value == null) return;
                // Dereference messages etc                
                
                _pedalViewModel = value;
                OnPropertyChanged("PedalViewModel");
            }
        }

        public int StartupTop
        {
            get { return Properties.Settings.Default.StartupTop; }
            set { Properties.Settings.Default.StartupTop = value; }
        }

        public int StartupLeft
        {
            get { return Properties.Settings.Default.StartupLeft; }
            set { Properties.Settings.Default.StartupLeft = value; }
        }


        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (PedalViewModel.IsDirty)
            {                
                MessageBoxResult result = MessageBox.Show("There are unsaved edits, do you wish to close?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
            Globals.PotValueMap = null;
            PedalViewModel.Dispose();
            PedalViewModel = null;
        }

    }
}
