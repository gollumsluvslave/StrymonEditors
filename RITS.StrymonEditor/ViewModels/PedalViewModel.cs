using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

using RITS.StrymonEditor.Models;

namespace RITS.StrymonEditor.ViewModels
{
    public class PedalViewModel:ViewModelBase
    {
        private StrymonPedalBase pedal;
        public PedalViewModel(StrymonPedalBase pedal)
        {

            this.pedal = pedal;
            this.preset = new StrymonPreset(pedal);
            Globals.ActivePreset = this.preset;
        }

        private StrymonPreset preset;
        private int _ct;
        public int CurrentType 
        { 
            get {return _ct;}
            set { _ct = value; OnPropertyChanged("CurrentType"); }
        }
        private double _mp1a=0;
        public double MainPot1Angle
        {
            get { return _mp1a; }
            set 
            { 
                _mp1a = value; 
                OnPropertyChanged("MainPot1Angle");
                OnPropertyChanged("MainPot1Value");
                LCDValue = "Time (" + MainPot1Value.ToString() + ")";
                
            }
        }
        public RelayCommand<object> SelectType
        {
            get
            {
                return new RelayCommand<object>(new Action<object>(x =>
                {
                    CurrentType = Convert.ToInt32(x);
                    OnPropertyChanged("CurrentType");
                }));
            }
        }
        public int MainPot1Value
        {
            get { return  DialValueConverter.AngleToValue(_mp1a);}
        }

        private double _mp2a=0;
        public double MainPot2Angle
        {
            get { return _mp2a; }
            set 
            { 
                _mp2a = value; 
                OnPropertyChanged("MainPot2Angle");
                OnPropertyChanged("MainPot2Value");
                LCDValue = "Repeats (" + MainPot2Value.ToString() + ")";
            }
        }
        public int MainPot2Value
        {
            get { return DialValueConverter.AngleToValue(_mp2a); }
        }

        public StrymonMachine Machine 
        {
            get { return preset.Machine; }
            set
            {
                preset.Machine = value;
                OnPropertyChanged("Machine");
                hc = null;
                OnPropertyChanged("HiddenParameters");
            }
        }

        private double _mp3a;
        public double MainPot3Angle
        {
            get { return _mp3a; }
            set 
            { 
                _mp3a = value; 
                OnPropertyChanged("MainPot3Angle");
                OnPropertyChanged("MainPot3Value");
                LCDValue = "Mix (" + MainPot3Value.ToString() + ")";
            }
        }
        public int MainPot3Value
        {
            get { return DialValueConverter.AngleToValue(_mp3a); }
        }


        private double _pp1a;
        public double ParamPot1Angle
        {
            get { return _pp1a; }
            set 
            { 
                _pp1a = value;
                OnPropertyChanged("ParamPot1Angle");
                OnPropertyChanged("ParamPot1Value");
                LCDValue = "Filter (" + ParamPot1Value.ToString() + ")";
            }
        }
        public int ParamPot1Value
        {
            get { return DialValueConverter.AngleToValue(_pp1a); }
        }

        private double _pp2a;
        public double ParamPot2Angle
        {
            get { return _pp2a; }
            set 
            {
                _pp2a = value;
                OnPropertyChanged("ParamPot2Angle");
                OnPropertyChanged("ParamPot2Value");
                LCDValue = "Grit (" + ParamPot2Value.ToString() + ")";
            }
        }
        public int ParamPot2Value
        {
            get { return DialValueConverter.AngleToValue(_pp2a); }
        }

        private double _pp3a;
        public double ParamPot3Angle
        {
            get { return _pp3a; }
            set 
            {
                _pp3a = value; 
                OnPropertyChanged("ParamPot3Angle");
                OnPropertyChanged("ParamPot3Value");
                LCDValue = "Speed (" + ParamPot3Value.ToString() + ")";
            }
        }
        public int ParamPot3Value
        {
            get { return DialValueConverter.AngleToValue(_pp3a); }
        }


        private double _pp4a;
        public double ParamPot4Angle
        {
            get { return _pp4a; }
            set 
            {
                _pp4a = value; 
                OnPropertyChanged("ParamPot4Angle");
                OnPropertyChanged("ParamPot4Value");
                LCDValue = "Depth (" + ParamPot4Value.ToString() + ")";
            }
        }
        public int ParamPot4Value
        {
            get { return DialValueConverter.AngleToValue(_pp4a); }
        }

        private string _lcdValue;
        public string LCDValue 
        { 
            get { return _lcdValue; }
            set { _lcdValue = value; OnPropertyChanged("LCDValue"); }
        }
        public void LoadPreset()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".syx";
            dlg.Filter = "Sysex Files (.syx)|*.syx";
            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                SysExParser parser = new SysExParser();
                SysExMessage msg = parser.Parse(filename);
                
                preset = new StrymonPreset(pedal,msg);
                Globals.ActivePreset = this.preset;
                this.Machine=preset.Machine;
                OnPropertyChanged("HiddenParameters");
            }
            // Synch Preset to UI - necessary?
            MainPot3Angle = DialValueConverter.ValueToAngle(preset.GetPotValue("MainPot3"));
            MainPot2Angle = DialValueConverter.ValueToAngle(preset.GetPotValue("MainPot1"));
            ParamPot1Angle = DialValueConverter.ValueToAngle(preset.GetPotValue("ParamPot1"));
            ParamPot2Angle = DialValueConverter.ValueToAngle(preset.GetPotValue("ParamPot2"));
            ParamPot3Angle = DialValueConverter.ValueToAngle(preset.GetPotValue("ParamPot3"));
            ParamPot4Angle = DialValueConverter.ValueToAngle(preset.GetPotValue("ParamPot4"));

            LCDValue = preset.Name;
        }

        public void SavePreset()
        {
            // Synch UI to Preset - deffo better way to do this!
            preset.SetPotValue("MainPot3", MainPot3Value);
            preset.SetPotValue("MainPot2", MainPot2Value);
            preset.SetPotValue("ParamPot1", ParamPot1Value);
            preset.SetPotValue("ParamPot2", ParamPot2Value);
            preset.SetPotValue("ParamPot3", ParamPot3Value);
            preset.SetPotValue("ParamPot4", ParamPot4Value);

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".syx";
            dlg.Filter = "Sysex Files (.syx)|*.syx";
            string filePath = null;
            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                filePath = dlg.FileName;
                preset.SaveAs(filePath);
            }

        }

        public BindableCollection<StrymonMachine> MachineTypes
        {
            get
            {
                var x = new BindableCollection<StrymonMachine>();
                foreach (var m in pedal.Machines)
                {
                    x.Add(m);
                }
                return x;
            }
        }
        private BindableCollection<Parameter> hc;
        public BindableCollection<Parameter> HiddenParameters
        {
            get
            {
                if (hc == null)
                {
                    hc = new BindableCollection<Parameter>();
                    if (preset != null)
                    {
                        foreach (var p in preset.HiddenParameters)
                        {
                            hc.Add(p);
                        }
                    }
                    else { hc = null; }
                }
                return hc;
            }
            set
            {
                hc = value;
                OnPropertyChanged("HiddenParameters");
            }
        }
    }
}
