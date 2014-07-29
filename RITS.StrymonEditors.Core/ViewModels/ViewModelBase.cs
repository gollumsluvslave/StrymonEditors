using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using RITS.StrymonEditor.Messaging;
using RITS.StrymonEditor.IO;

namespace RITS.StrymonEditor.ViewModels
{
    /// <summary>
    /// Base class for all view models
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IColleague, IDisposable
    {
        private IFileIOService fileIOService;
        private IMessageDialog messageDialog;

        /// <summary>
        /// Singleton Mediator
        /// </summary>
        protected static IMediator mediatorInstance = new Mediator();
        /// <summary>
        /// Default constructor
        /// </summary>
        public ViewModelBase()
        {
            Mediator = mediatorInstance;
        }

        private IMediator mediator;
        /// <summary>
        /// Changed mediator pattern to follow a more flexible method to aid testing, and allow resetting
        /// </summary>
        public IMediator Mediator
        {
            get { return mediator; }
            set
            {
                if (mediator != null) DeRegisterFromMediator();
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
                    fileIOService = NativeHooks.Current.CreateFileIOService();
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
                    messageDialog = NativeHooks.Current.CreateMessageDialog();
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

    }




}
