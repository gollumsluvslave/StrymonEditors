using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RITS.StrymonEditor.MIDI;
using RITS.StrymonEditor.Commands;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.Messaging;
namespace RITS.StrymonEditor.ViewModels
{
    /// <summary>
    /// ViewModel reponsible for the <see cref="Views.PresetControl"/> view
    /// </summary>
    public class PresetControlViewModel: ViewModelBase, IDisposable

    {
        StrymonPedal pedal;
        IStrymonMidiManager midiManager;
        /// <summary>
        /// Default .ctor
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="contextPedal"></param>
        /// <param name="midiManager"></param>
        public PresetControlViewModel(string mode, StrymonPedal contextPedal, IStrymonMidiManager midiManager)
        {
            pedal = contextPedal;
            PresetIsEnabled = true;
            Mode = mode;
            PresetIndex = 0;
            this.midiManager = midiManager;
        }

        #region IColleague
        /// <inheritdoc/>
        public override void RegisterWithMediator()
        {
            Mediator.Register(ViewModelMessages.BulkLoadPedalComplete, BulkLoadCompleteCallback);
        }

        /// <inheritdoc/>
        public override void DeRegisterFromMediator()
        {
            Mediator.UnRegister(ViewModelMessages.BulkLoadPedalComplete, BulkLoadCompleteCallback);
        }

        // Callback for BulkLoadComplete - to update menu
        private void BulkLoadCompleteCallback(object o)
        {
            execute.RaiseCanExecuteChanged();
        }

        #endregion

        /// <inheritdoc/>
        public override void Dispose()
        {
            base.Dispose();
        }

        /// <summary>
        /// The mode of operation - either Fetch / Push
        /// </summary>
        private string mode;
        public string Mode
        {
            get { return mode; }
            set { mode = value; OnPropertyChanged("Mode"); }
        }

        /// <summary>
        /// The preset name to display
        /// </summary>
        public string PresetName 
        {
            get { return string.Format("{0} : {1}", GetPresetCode(PresetIndex), pedal.GetPresetName(PresetIndex)); }
            set { }
        }

        private int presetIndex;
        /// <summary>
        /// The preset index to display
        /// </summary>
        public int PresetIndex 
        {
            get { return presetIndex; }
            set 
            {
                if (Globals.MachineLocked && Mode=="Fetch")
                {
                    bool dec = (presetIndex > value);
                    var list = pedal.LockedMachineSpecificPresets();
                    if (list.Count == 0) return;
                    var first = list.FirstOrDefault();
                    
                    var last = list.LastOrDefault();
                    if (value < first.Index)
                    {
                        presetIndex = last.Index;
                    }
                    else if (value > last.Index)
                    {
                        presetIndex = first.Index;
                    }
                    else
                    {
                        var preset = dec ? list.OrderByDescending(x => x.Index).FirstOrDefault(x => x.Index <= value) : list.FirstOrDefault(x => x.Index >= value);
                        presetIndex = preset.Index;
                    }
                }
                else
                {
                    // TODO : Lock Machine effect
                    if (value < 0) value = pedal.PresetCount - 1;
                    if (value > (pedal.PresetCount - 1)) value = 0;

                    presetIndex = value;
                }
                OnPropertyChanged("PresetIndex");
                OnPropertyChanged("PresetName");
            }
        }

        private bool presetIsEnabled;
        /// <summary>
        /// Determines whether or not the preset control should be enabled
        /// </summary>
        public bool PresetIsEnabled
        {
            get { return presetIsEnabled; }
            set { presetIsEnabled = value; OnPropertyChanged("PresetIsEnabled"); }
        }

        private RelayCommand execute;
        /// <summary>
        /// Command that executes the fethc / push dependent on the current mode
        /// </summary>
        public RelayCommand Execute 
        {
            get 
            {
                if (execute == null)
                {
                    execute = new RelayCommand(new Action(() => 
                    {
                        ExecuteCommand();
                    }), new Func<bool>(() => pedal.RawPresetData.Count == pedal.PresetCount));
                }
                return execute;
            }
        }

        private void ExecuteCommand()
        {
            if (Mode == "Fetch")
            {
                Mediator.NotifyColleagues(ViewModelMessages.FetchPresetRequested, PresetIndex);
            }
            else if (Mode == "Push")
            {
                Mediator.NotifyColleagues(ViewModelMessages.PushPresetRequested, PresetIndex);
            }
        }

        private string GetPresetCode(int i)
        {
            int div = (pedal.PresetCount == 300) ? 3 : 2;
            int bank = i / div;
            int remainder = i % div;
            string bankString = bank.ToString();
            int len = bankString.Length;
            if (len < div)
            {
                bankString = "0" + bankString;
            }
            if (div == 3)
            {
                if (remainder == 0) bankString += "A";
                if (remainder == 1) bankString += "B";
                if (remainder == 2) bankString += "C";
            }
            else
            {
                if (remainder == 0) bankString += "A";
                if (remainder == 1) bankString += "B";
            }
            return bankString;
        }

    }
}
