using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RITS.StrymonEditor.Commands;
using RITS.StrymonEditor.Messaging;
using RITS.StrymonEditor.Models;
namespace RITS.StrymonEditor.ViewModels
{
    /// <summary>
    /// View model that wraps the <see cref="StrymonMachine"/> to the UI / View
    /// </summary>
    public class StrymonMachineViewModel : ViewModelBase
    {
        public StrymonMachine _machine;

        /// <summary>
        /// Default .ctor
        /// </summary>
        /// <param name="machine"></param>
        public StrymonMachineViewModel(StrymonMachine machine)
        {
            _machine = machine;
        }

        /// <summary>
        /// Exposes the name of the machine
        /// </summary>
        public string Name
        {
            get { return _machine.Name; }
        }

        /// <summary>
        /// Exposes the underlying value / id of the machine (0-11)
        /// </summary>
        public int Value
        {
            get { return _machine.Value; }
        }

        private bool _isActive;
        /// <summary>
        /// Determines whether the underlying <see cref="StrymonMachine"/> is currently active in the active <see cref="StrymonPreset"/>
        /// </summary>
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                OnPropertyChanged("IsActive");
            }

        }

        /// <summary>
        /// Command that is invoked when the Machine is changed via a button
        /// </summary>
        public RelayCommand<object> SelectType
        {
            get
            {
                return new RelayCommand<object>(new Action<object>(x =>
                {
                    if (!Globals.MachineLocked)
                    {
                        Mediator.NotifyColleagues(ViewModelMessages.MachineSelected, this);
                    }
                }));
            }
        }

    }
}
