using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.Messaging;
namespace RITS.StrymonEditor.ViewModels
{
    // TODO
    public class PresetControlViewModel: ViewModelBase, IDisposable

    {
        StrymonPedal pedal;
        IStrymonMidiManager midiManager;
        public PresetControlViewModel(string mode, StrymonPedal contextPedal, IStrymonMidiManager midiManager)
        {
            pedal = contextPedal;
            PresetIsEnabled = true;
            Mode = mode;
            PresetIndex = 0;
            this.midiManager = midiManager;
        }

        public override void RegisterWithMediator()
        {
            Mediator.Register(ViewModelMessages.BulkLoadComplete, BulkLoadCompleteCallback);
        }
        public override void DeRegisterFromMediator()
        {
            Mediator.UnRegister(ViewModelMessages.BulkLoadComplete, BulkLoadCompleteCallback);
        }


        private void BulkLoadCompleteCallback(object o)
        {
            execute.RaiseCanExecuteChanged();
        }

        public void Dispose()
        {
            base.Dispose();
        }
        private string mode;
        public string Mode
        {
            get { return mode; }
            set { mode = value; OnPropertyChanged("Mode"); }
        }

        public string PresetName 
        {
            get { return string.Format("{0} : {1}", GetPresetCode(PresetIndex), pedal.GetPresetName(PresetIndex)); }
            set { }
        }

        private int presetIndex;
        public int PresetIndex 
        {
            get { return presetIndex; }
            set 
            {
                if (value < 0) value = pedal.PresetCount - 1;
                if (value > (pedal.PresetCount - 1)) value = 0;

                presetIndex = value; 
                OnPropertyChanged("PresetIndex");
                OnPropertyChanged("PresetName");
            }
        }

        private bool presetIsEnabled;
        public bool PresetIsEnabled
        {
            get { return presetIsEnabled; }
            set { presetIsEnabled = value; OnPropertyChanged("PresetIsEnabled"); }
        }


        private RelayCommand execute;
        public RelayCommand Execute 
        {
            get 
            {
                if (execute == null)
                {
                    execute = new RelayCommand(new Action(() => 
                    {
                        ExecuteCommand();
                    }),new Func<bool>(()=>!midiManager.IsBulkFetching));
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

        public string GetPresetCode(int i)
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
