using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading;
using RITS.StrymonEditor.Messaging;
using RITS.StrymonEditor.IO;
using RITS.StrymonEditor.Views;

namespace RITS.StrymonEditor.ViewModels
{
    /// <summary>
    /// Base class for all view models
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IColleague, IDisposable
    {
        private IFileIOService fileIOService;
        private IMessageDialog messageDialog;
        protected static IMediator mediatorInstance = new Mediator();
        /// <summary>
        /// Default constructor
        /// </summary>
        public ViewModelBase()
        {
            Mediator = mediatorInstance;
        }
        
        /// <summary>
        /// Changed mediator pattern to follow a more flexible method to aid testing, and allow resetting
        /// </summary>
        private IMediator mediator;
        public IMediator Mediator 
        {
            get { return mediator; }
            set 
            {
                if(mediator!=null) DeRegisterFromMediator();
                mediator = value;
                RegisterWithMediator();
            }
        } // Should be private set, but leave as public to allow testing with Mock

        // Property to allow IO mocks to be injected in
        public IFileIOService FileIOService
        {
            get
            {
                if (fileIOService == null)
                {
                    fileIOService = new FileIOService(new FileDialogOpen(), new FileDialogSave(), new MessageDialog());
                }
                return fileIOService;
            }
            set
            {
                fileIOService = value;
            }
        }

        public IMessageDialog MessageDialog
        {
            get
            {
                if (messageDialog == null)
                {
                    messageDialog = new MessageDialog(); ;
                }
                return messageDialog;
            }
            set
            {
                messageDialog = value;
            }
        }



        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));                
            }
        }

        #endregion // INotifyPropertyChanged Members


        /// <summary>
        /// Helper method for all ViewModels to delegate any kind of work that requires UI time
        /// </summary>
        /// <param name="work"></param>
        public void DoWork(Action work)
        {
            if (!Thread.CurrentThread.IsBackground)
            {
                var worker = new BackgroundWorker();

                worker.DoWork += (sender, e) => work();
                worker.RunWorkerCompleted += (sender, e) => this.Complete();                
                worker.RunWorkerAsync();
            }
            else
            {
                work();
            }
        }
        public void DoWork(Action<object> work, object arg)
        {
            if (!Thread.CurrentThread.IsBackground)
            {
                var worker = new BackgroundWorker();
                worker.DoWork += (sender, e) => work(arg);
                worker.RunWorkerCompleted += (sender, e) => this.Complete();
                worker.RunWorkerAsync();
            }
            else
            {
                work(arg);
            }
        }
        public void DoWorkSync(Action work)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            work();
            Mouse.OverrideCursor = null;
        }

        public virtual void RegisterWithMediator()
        {
            
        }
        public virtual void DeRegisterFromMediator()
        {
        }

        public virtual void Dispose()
        {
            DeRegisterFromMediator();
        }

        protected virtual void Complete()
        {
            Mouse.OverrideCursor = null;
        }
    }




}
