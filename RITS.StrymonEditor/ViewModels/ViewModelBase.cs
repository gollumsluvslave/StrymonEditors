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

        /// <summary>
        /// Property to allow IO mocks to be injected in
        /// </summary>
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
        
        /// <summary>
        /// Property to allow dialog mocks to be injected in
        /// </summary>
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


        /// <summary>
        /// Helper method for all ViewModels to delegate any kind of work that requires UI time, with a parameter
        /// </summary>
        /// <param name="work"></param>
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

        /// <summary>
        /// Helper method for all ViewModels to delegate any kind of work that requires UI time but must be executed synchronously
        /// </summary>
        /// <param name="work"></param>
        public void DoWorkSync(Action work)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            work();
            Mouse.OverrideCursor = null;
        }

        /// <inheritdoc/>
        public virtual void RegisterWithMediator()
        {
            
        }
        /// <inheritdoc/>
        public virtual void DeRegisterFromMediator()
        {
        }

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            DeRegisterFromMediator();
        }

        /// <summary>
        /// Complete callback for long running work
        /// </summary>
        protected virtual void Complete()
        {
            Mouse.OverrideCursor = null;
        }
    }




}
