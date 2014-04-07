using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RITS.StrymonEditor.Messaging;
using RITS.StrymonEditor.Models;
namespace RITS.StrymonEditor.ViewModels
{
    /// <summary>
    /// View Model that mediates between the 
    /// view and parameters that are hidden in the physical pedal menu
    /// </summary>
    public class ParameterViewModel: ViewModelBase, IDisposable
    {
        public Parameter _parameter;
        public ParameterViewModel(Parameter parameter)
        {
            _parameter = parameter;
        }

        public override void RegisterWithMediator()
        {
            Mediator.Register(ViewModelMessages.ParameterChanged, HandleParameterChanged);
        }
        public override void DeRegisterFromMediator()
        {
            Mediator.UnRegister(ViewModelMessages.ParameterChanged, HandleParameterChanged);
        }


        /// <summary>
        /// Exposes the parameter name
        /// </summary>
        public string Name 
        {
            get { return _parameter.Name; }
        }

        /// <summary>
        /// Exposes the parameter value
        /// </summary>
        public int Value
        {
            get { return _parameter.Value; }
            set
            {
                _parameter.Value = value;
                // Different kinds of assignment / pot link??
                if (LinkedPot != null && _parameter.Value != LinkedPot.Value)
                {
                    LinkedPot.Value = _parameter.Value;
                }

                OnPropertyChanged("Value");
                OnPropertyChanged("Label");
                Mediator.NotifyColleagues(ViewModelMessages.ParameterChanged, _parameter);                
                Mediator.NotifyColleagues(ViewModelMessages.LCDUpdate, LCDUpdateValue);
            }
        }
        
        ///// <summary>
        ///// Exposes a meaningful label for the parameter
        ///// </summary>
        //public string Label
        //{
        //    get
        //    {                
        //        return _parameter.Label;
        //    }
        //    set
        //    {
        //        OnPropertyChanged("Label");
        //    }
        //}

        /// <summary>
        /// Exposes the configured definition
        /// </summary>
        public ParameterDef Definition
        {
            get { return _parameter.Definition; }
        }

        /// <summary>
        /// Assigns the parameter to a pot
        /// </summary>
        /// <param name="potVM"></param>
        public void AssignToDynamicPot(PotViewModel potVM)
        {
            potVM.Label = Name;
            LinkedPot = potVM;
            if (potVM.LinkedParameter != null) potVM.LinkedParameter.DynamicPotIdAssigned = 0; //Unassign parameter
            this._parameter.DynamicPotIdAssigned = potVM.Id;
            potVM.LinkedParameter = this._parameter;
        }


        private PotViewModel _linkedPot;
        /// <summary>
        /// Exposes the pot that this parameter 
        /// is linked to via the view models
        /// </summary>
        public PotViewModel LinkedPot
        {
            get { return _linkedPot; }
            set
            {
                _linkedPot = value;
            }
        }

        // Helper that provides a string value for the main LCD
        private string LCDUpdateValue
        {
            get
            {
                return string.Format("{0} {1}", Name, _parameter.ValueLabel); 
            }
        }

        // Necesary to keep dynamic controls in sync
        private void HandleParameterChanged(object p)
        {
            Parameter param = p as Parameter;
            if (param.Name == _parameter.Name)
            {
                OnPropertyChanged("Value");
                OnPropertyChanged("Label");
            }
        }

        public void Dispose()
        {
            base.Dispose();
        }
    }
}
