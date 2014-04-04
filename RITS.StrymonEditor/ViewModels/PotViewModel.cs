using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RITS.StrymonEditor.Models;

namespace RITS.StrymonEditor.ViewModels
{
    /// <summary>
    /// Factory class that is responsible for creating <see cref="PotViewModel"/> instances
    /// </summary>
    public static class PotViewModelFactory
    {
        public static PotViewModel Create(Pot pot, Parameter linkedParameter)
        {
            if (pot.IsDynamic)
            {
                return new DynamicPotViewModel(pot, linkedParameter);
            }
            if (pot.IsFine)
            {
                return new FinePotViewModel(pot, linkedParameter);
            }
            if (pot.IsCoarse)
            {
                return new CoarsePotViewModel(pot, linkedParameter);
            }
            return new PotViewModel(pot, linkedParameter);
        }
    }

    /// <summary>
    /// Standard pot view model
    /// </summary>
    public class PotViewModel:ViewModelBase
    {
        protected Pot _pot;
        protected IPotValueConverter _potValueConverter; 
        public PotViewModel(Pot pot, Parameter linkedParameter)
        {
            _pot = pot;
            _linkedParameter = linkedParameter;
            _potValueConverter = PotValueConverterFactory.Create(_linkedParameter, this._pot);
        }
        public int Id 
        {
            get { return _pot.Id; }
        }

        protected string _labelOverride;

        public StrymonPedal ContextPedal { get; set; }
        public virtual string Label 
        {
            get { return _labelOverride;  }
            set 
            { 
                _labelOverride = value;
                OnPropertyChanged("Label");
            }
        }

        public int Left { get { return _pot.Left; } }

        public int Top { get { return _pot.Top; } }

        public bool Hide { get { return _pot.Hide; } }

        protected double _angle;
        protected int _value;
        public virtual double Angle
        {
            get 
            {
                return _angle; 
            }
            set
            {
                _angle = value;
                _value = _potValueConverter.AngleToValue(_angle);
                LinkedParameter.Value = Value;
                OnPropertyChanged("Angle");
                OnPropertyChanged("Value");
                Mediator.NotifyColleagues(ViewModelMessages.ParameterChanged, LinkedParameter);
                Mediator.NotifyColleagues(ViewModelMessages.LCDUpdate, ValueLabel);

            }
        }

        public virtual int Value
        {
            get 
            {
                return _value;
            }
            set 
            {
                _value = value;

                Angle = _potValueConverter.ValueToAngle(value);
            }
        }
        
        
        protected Parameter _linkedParameter;
        public Parameter LinkedParameter
        {
            get { return _linkedParameter; }
            set
            {
                _linkedParameter = value;
                _potValueConverter = PotValueConverterFactory.Create(_linkedParameter, this._pot);
                Value = _linkedParameter.Value;
                
            }
        }

        public virtual string ValueLabel
        {
            get 
            {
                return string.Format("{0} {1}", LinkedParameter.Name, LinkedParameter.ValueLabel); 
            }
        }

        public bool IsFineControlPot
        {
            get { return this is FinePotViewModel; }
        }

        public bool IsCoarseControlPot
        {
            get { return this is CoarsePotViewModel; }
        }

        public bool IsDynamicControlPot
        {
            get { return this is DynamicPotViewModel; }
        }

        public bool IsNormalControlPot
        {
            get { return !IsFineControlPot && !IsCoarseControlPot && !IsDynamicControlPot; }
        }

    }

    /// <summary>
    /// Dynamic pot view model for Mobius and BigSky dynamic parameters
    /// </summary>
    public class DynamicPotViewModel : PotViewModel
    {
        public DynamicPotViewModel(Pot pot,Parameter linkedParameter):base(pot,linkedParameter)
        {
            
        }
    }

    /// <summary>
    /// Coarse pot view model
    /// </summary>
    public class CoarsePotViewModel : PotViewModel
    {
        public CoarsePotViewModel(Pot pot, Parameter linkedParameter)
            : base( pot, linkedParameter)
        {
        }


        public override double Angle
        {
            get
            {
                return _angle;
            }
            set
            {
                _angle = value;
                _value = _potValueConverter.AngleToValue(_angle);
                // TODO Different Ranges for Mobius Coarse vs Fine?
                if (Globals.FineCoarseSynchroniser != null)
                {
                    Globals.FineCoarseSynchroniser.SetCoarseValue(_value);
                    OnPropertyChanged("Angle");
                    OnPropertyChanged("Value");

                    Mediator.NotifyColleagues(ViewModelMessages.ParameterChanged, LinkedParameter);
                    Mediator.NotifyColleagues(ViewModelMessages.LCDUpdate, ValueLabel);
                }
            }
        }


        

        public bool HandleFineRangeOverrides(Pot machineOverridePot, StrymonPreset preset)
        {
            // Range Overrides to FineRange
            if (machineOverridePot != null)
            {
                Label = machineOverridePot.Label;
                foreach (var ro in machineOverridePot.RangeOverrides)
                {
                    if (ro.TriggerParameter == null)
                    {
                        LinkedParameter.Definition.OverrideRanges(ro);
                        Globals.PotValueMap.ApplyFineValueIncrementMap(ro.IncrementMap, LinkedParameter.Definition);
                        return true;
                    }
                    else
                    {
                        var roParam = preset.AllParameters.FirstOrDefault(x => x.Name == ro.TriggerParameter && x.Value == ro.Value);
                        if (roParam != null)
                        {
                            LinkedParameter.Definition.OverrideRanges(ro);
                            Globals.PotValueMap.ApplyFineValueIncrementMap(ro.IncrementMap, LinkedParameter.Definition);
                            return true;
                        }
                    }
                }
            }
            return false;
        }


    }

    /// <summary>
    /// Fine pot view model
    /// </summary>
    public class FinePotViewModel : PotViewModel
    {
        public FinePotViewModel(Pot pot, Parameter linkedParameter)
            : base(pot, linkedParameter)
        {

        }


        public override int Value
        {
            get
            {
                return LinkedParameter.FineValue;
            }
            set
            {
                if (Globals.FineCoarseSynchroniser != null)
                {
                    Globals.FineCoarseSynchroniser.SetFineValue(value);
                    OnPropertyChanged("Value");
                    Mediator.NotifyColleagues(ViewModelMessages.ParameterChanged, LinkedParameter);
                    Mediator.NotifyColleagues(ViewModelMessages.LCDUpdate, ValueLabel);
                }
                
            }
        }

        public void HandleDirectEntry(double value)
        {
            // Now we have a differentiator
            LinkedParameter.DirectEntryChange = true;
            if (!Globals.IsBPMModeActive)
            {
                Value = Convert.ToInt32(value);
            }
            else
            {
                if (LinkedParameter.ContextPedalName == StrymonPedal.Mobius_Name)
                {
                    Value = Globals.ConvertBPMToMilliHz(value);
                }
                else
                {
                    Value = Globals.ConvertBPMToMilliseconds(value);
                }
            }
            LinkedParameter.DirectEntryChange = false;
        }
    }
}
