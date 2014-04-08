using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.ViewModels
{
    public class ModalProgressDialogViewModel:ViewModelBase
    {
        private Action<object> work;
        public ModalProgressDialogViewModel(Action<object> modalWork)
        {
            work = modalWork;
        }
        public Action Close { get; set; }
        private bool showProgressBar=true;
        public bool ShowProgressBar
        {
            get { return showProgressBar; }
            set { showProgressBar = value; OnPropertyChanged("ShowProgressBar"); }
        }


        private string pbStatus;
        public string PBStatus
        {
            get { return pbStatus; }
            set { pbStatus = value; OnPropertyChanged("PBStatus"); }
        }

        private int pbMax;
        public int PBMax
        {
            get { return pbMax; }
            set { pbMax = value; OnPropertyChanged("PBMax"); }
        }

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

        public void Start()
        {
            DoWork(work,this);
        }

        protected override void Complete()
        {
            MessageDialog.ShowInfo("Restore completed succesfully!", "Restore Backup");
            Close();
        }
    }
}
