using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.ViewModels
{
    /// <summary>
    /// View Model responsible for the modal progress dialog used for Pedal Restore operation
    /// Potential reuse for other long-running modal operations
    /// </summary>
    public class ModalProgressDialogViewModel:ViewModelBase
    {
        private Action<object> work;
        public ModalProgressDialogViewModel(Action<object> modalWork)
        {
            work = modalWork;
        }

        /// <summary>
        /// Delegate that allows the vm to close the dialog / window
        /// </summary>
        public Action Close { get; set; }

        /// <summary>
        /// Whether or not the progressbar should be displayed
        /// </summary>
        private bool showProgressBar=true;
        public bool ShowProgressBar
        {
            get { return showProgressBar; }
            set { showProgressBar = value; OnPropertyChanged("ShowProgressBar"); }
        }

        /// <summary>
        /// The current status text for operation
        /// </summary>
        private string pbStatus;
        public string PBStatus
        {
            get { return pbStatus; }
            set { pbStatus = value; OnPropertyChanged("PBStatus"); }
        }

        /// <summary>
        /// The max for the operation
        /// </summary>
        private int pbMax;
        public int PBMax
        {
            get { return pbMax; }
            set { pbMax = value; OnPropertyChanged("PBMax"); }
        }

        /// <summary>
        /// The current value / position of the operation
        /// </summary>
        private int pbValue;
        public int PBValue
        {
            get { return pbValue; }
            set 
            { 
                pbValue = value; 
                OnPropertyChanged("PBValue");                
                if (pbValue >= PBMax)
                {
                    // ??
                }

            }
        }

        /// <summary>
        /// Initiate the operation on a background thread
        /// </summary>
        public void Start()
        {
            DoWork(work,this);
        }

        /// <summary>
        /// Completes the operation, displaying a message to the user and closing the dialog
        /// </summary>
        protected override void Complete()
        {
            MessageDialog.ShowInfo("Restore completed succesfully!", "Restore Backup");
            Close();
        }
    }
}
