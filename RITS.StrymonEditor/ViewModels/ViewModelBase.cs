using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading;
using RITS.StrymonEditor.Messaging;

namespace RITS.StrymonEditor.ViewModels
{
    /// <summary>
    /// Base class for all view models
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IColleague, IDisposable
    {
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

    // Various MVVM interprations  - mostly credit to Josh Smith for the ideas
    /// <summary>
    /// Relay command with parameter of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RelayCommand<T> : ICommand
    {
        #region Fields

        readonly Action<T> _execute = null;
        readonly Predicate<T> _canExecute = null;

        #endregion // Fields

        #region Constructors

        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public void RaiseCanExecuteChanged()
        {

            CommandManager.InvalidateRequerySuggested();

        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        #endregion // ICommand Members
    }

    /// <summary>
    /// Simple RelayCommand - delegates to parameterless Action
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Fields

        readonly Action _execute;
        readonly Func<bool> _canExecute;

        #endregion // Fields

        #region Constructors

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public void RaiseCanExecuteChanged()
        {

            CommandManager.InvalidateRequerySuggested();

        }

        public void Execute(object parameter)
        {
            _execute();
        }

        #endregion // ICommand Members
    }

    /// <summary>
    /// Helper collection class to allow easy two-way binding of collections / lists
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BindableCollection<T> : IList<T>, INotifyCollectionChanged
    {
        private IList<T> collection = new List<T>();
        private Dispatcher dispatcher;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public BindableCollection()
        {
            dispatcher = Dispatcher.CurrentDispatcher;
        }
        public void Add(T item)
        {
            if (Thread.CurrentThread == dispatcher.Thread)
                DoAdd(item);
            else
                dispatcher.Invoke((Action)(() =>
                {
                    DoAdd(item);
                }));
        }
        private void DoAdd(T item)
        {
            collection.Add(item);
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }
        public void Clear()
        {
            if (Thread.CurrentThread == dispatcher.Thread)
                DoClear();
            else
                dispatcher.Invoke((Action)(() =>
                {
                    DoClear();
                }));
        }
        private void DoClear()
        {
            collection.Clear();
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public bool Contains(T item)
        {
            var result = collection.Contains(item);
            return result;
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }
        public int Count
        {
            get
            {
                var result = collection.Count;
                return result;
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return collection.IsReadOnly;
            }
        }
        public bool Remove(T item)
        {
            if (Thread.CurrentThread == dispatcher.Thread)
                return DoRemove(item);
            else
            {
                var op = dispatcher.BeginInvoke(new Func<T, bool>(DoRemove), item);
                if (op == null || op.Result == null)
                    return false;
                return (bool)op.Result;
            }
        }
        private bool DoRemove(T item)
        {
            var index = collection.IndexOf(item);
            if (index == -1)
            {
                return false;
            }
            var result = collection.Remove(item);
            if (result && CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            return result;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return collection.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }
        public int IndexOf(T item)
        {
            var result = collection.IndexOf(item);
            return result;
        }
        public void Insert(int index, T item)
        {
            if (Thread.CurrentThread == dispatcher.Thread)
                DoInsert(index, item);
            else
                dispatcher.Invoke((Action)(() =>
                {
                    DoInsert(index, item);
                }));
        }
        private void DoInsert(int index, T item)
        {
            collection.Insert(index, item);
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }
        public void RemoveAt(int index)
        {
            if (Thread.CurrentThread == dispatcher.Thread)
                DoRemoveAt(index);
            else
                dispatcher.Invoke((Action)(() =>
                {
                    DoRemoveAt(index);
                }));
        }
        private void DoRemoveAt(int index)
        {
            if (collection.Count == 0 || collection.Count <= index)
            {
                return;
            }
            collection.RemoveAt(index);
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public T this[int index]
        {
            get
            {
                var result = collection[index];
                return result;
            }
            set
            {
                if (collection.Count == 0 || collection.Count <= index)
                {
                    return;
                }
                collection[index] = value;
            }
        }
    }

    /// <summary>
    /// View model for generic MenuItem binding
    /// </summary>
    public class MenuItemViewModel : ViewModelBase
    {
        public MenuItemViewModel Parent { get; set; }
        public MenuItemViewModel()
        {
            Children = new BindableCollection<MenuItemViewModel>();
        }

        private bool isSeparator;
        public bool IsSeparator
        {

            get { return isSeparator; }

            set
            {
                isSeparator = value;
                OnPropertyChanged("IsSeparator");
            }
        }

        private string _menuText;
        public string MenuText
        {

            get { return _menuText; }

            set
            {
                _menuText = value;
                OnPropertyChanged("MenuText");
            }
        }

        private string _inputgestureText;
        public string InputGestureText
        {

            get { return _inputgestureText; }

            set
            {
                _inputgestureText = value;
                OnPropertyChanged("InputGestureText");
            }
        }

        private object tag;
        public object Tag
        {

            get { return tag; }

            set
            {
                tag = value;
                OnPropertyChanged("Tag");
            }
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }

            set
            {
                _isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        private bool _isCheckable;
        public bool IsCheckable
        {
            get { return _isCheckable; }

            set
            {
                _isCheckable = value;
                OnPropertyChanged("IsCheckable");
            }
        }

        private ICommand command;
        public ICommand Command
        {
            get { return command; }

            set
            {
                command = value;
                OnPropertyChanged("Command");
            }
        }

        private ICommand checkedCommand;
        public ICommand CheckedCommand
        {
            get { return checkedCommand; }

            set
            {
                checkedCommand = value;
                OnPropertyChanged("CheckedCommand");
            }
        }

        private ICommand uncheckedCommand;
        public ICommand UncheckedCommand
        {
            get { return uncheckedCommand; }

            set
            {
                uncheckedCommand = value;
                OnPropertyChanged("UncheckedCommand");
            }
        }

        private BindableCollection<MenuItemViewModel> _children;
        public BindableCollection<MenuItemViewModel> Children
        {
            get { return _children; }

            set
            {
                _children = value;
                OnPropertyChanged("Children");
            }
        }

        public bool IsDestructive { get; set; }
    }

}
