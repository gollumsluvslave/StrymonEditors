using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RITS.StrymonEditor.Conversion;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.Messaging;
namespace RITS.StrymonEditor.ViewModels
{
    /// <summary>
    /// Factory class that is responsible for creating <see cref="PotViewModel"/> instances
    /// </summary>
    public static class PotViewModelFactory
    {
        /// <summary>
        /// Returns the correct <see cref="PotViewModel"/> based on the supplied <see cref="Pot"/> and <see cref="Parameter"/>
        /// </summary>
        /// <param name="pot"></param>
        /// <param name="linkedParameter"></param>
        /// <returns></returns>
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
    /// Standard / base pot view model
    /// </summary>
    public class PotViewModel:ViewModelBase
    {
        protected Pot _pot;
        protected IPotValueConverter _potValueConverter; 
        /// <summary>
        /// Default .ctor
        /// </summary>
        /// <param name="pot"></param>
        /// <param name="linkedParameter"></param>
        public PotViewModel(Pot pot, Parameter linkedParameter)
        {
            _pot = pot;
            _linkedParameter = linkedParameter;
            _potValueConverter = PotValueConverterFactory.Create(_linkedParameter, this._pot);
        }

        /// <summary>
        /// The id of the <see cref="Pot"/>
        /// </summary>
        public int Id 
        {
            get { return _pot.Id; }
        }

        protected string _labelOverride;

        /// <summary>
        /// The name of the context <see cref="StrymonPedal"/>
        /// </summary>
        public StrymonPedal ContextPedal { get; set; }

        /// <summary>
        /// A textual label for this pot
        /// </summary>
        public virtual string Label 
        {
            get { return _labelOverride;  }
            set 
            { 
                _labelOverride = value;
                OnPropertyChanged("Label");
            }
        }

        /// <summary>
        /// Expose the left property of the <see cref="Pot"/>
        /// </summary>
        public int Left { get { return _pot.Left; } }

        /// <summary>
        /// Expose the top property of the <see cref="Pot"/>
        /// </summary>
        public int Top { get { return _pot.Top; } }

        /// <summary>
        /// Expose the Hide property of the <see cref="Pot"/>
        /// </summary>
        public bool Hide { get { return _pot.Hide; } }

        protected double _angle;
        protected int _value;
        /// <summary>
        /// The angle of the <see cref="Pot"/>
        /// </summary>
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

        /// <summary>
        /// The underlying value of the <see cref="Pot"/> / <see cref="Parameter"/>
        /// </summary>
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
        /// <summary>
        /// The <see cref="Parameter"/> linked to the underlying <see cref="Pot"/>
        /// </summary>
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

        /// <summary>
        /// Exposes the value label of the linked <see cref="Parameter"/>
        /// </summary>
        public virtual string ValueLabel
        {
            get 
            {
                return string.Format("{0} {1}", LinkedParameter.Name, LinkedParameter.ValueLabel); 
            }
        }

        /// <summary>
        /// returns whether the underlying <see cref="Pot"/> is a 'fine' encoder pot
        /// </summary>
        public bool IsFineControlPot
        {
            get { return this is FinePotViewModel; }
        }

        /// <summary>
        /// returns whether the underlying <see cref="Pot"/> is a 'coarse' pot
        /// </summary>
        public bool IsCoarseControlPot
        {
            get { return this is CoarsePotViewModel; }
        }

        /// <summary>
        /// returns whether the underlying <see cref="Pot"/> is a dyanmic pot that can be assigned to differente parameters
        /// </summary>
        public bool IsDynamicControlPot
        {
            get { return this is DynamicPotViewModel; }
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


        /// <summary>
        /// Handle set/retrieval of the angle property
        /// </summary>
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

        
        /// <summary>
        /// Process <see cref="Pot"/> overrides
        /// </summary>
        /// <param name="machineOverridePot"></param>
        /// <param name="preset"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Overides the setting of the 'fine' value on the linked parameter
        /// Delegates to the <see cref="FineCoarseSynchroniser"/>
        /// </summary>
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

        /// <summary>
        /// Handle the supplied value as a result of a direct entry operation
        /// </summary>
        /// <param name="value"></param>
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
                    Value = ConversionUtils.ConvertBPMToMilliHz(value);
                }
                else
                {
                    Value = ConversionUtils.ConvertBPMToMilliseconds(value);
                }
            }
            // Need to reset DirectEntry flag
            LinkedParameter.DirectEntryChange = false;
        }
    }
}
