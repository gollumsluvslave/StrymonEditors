using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace RITS.StrymonEditor.Commands
{
    // Various MVVM interprations  - mostly credit to Josh Smith
    /// <summary>
    /// Relay command with parameter of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RelayCommand<T> : ICommand
    {
        #region Fields

        readonly Action<T> _execute = null;
        readonly Predicate<T> _canExecute = null;
        private bool _isEnabled;
        private T parameter;
        #endregion // Fields

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="execute"></param>
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

        //public bool IsEnabled
        //{
        //    get
        //    {
        //        if (_canExecute == null) return true;
        //        bool enabled = _canExecute(parameter);
        //        if (enabled != _isEnabled)
        //        {
        //            _isEnabled = enabled;
        //            if (CanExecuteChanged != null)
        //            {
        //                CanExecuteChanged(this, EventArgs.Empty);
        //            }
        //        }
                
        //        return _isEnabled;
        //    }
        //}

        #region ICommand Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null) return true;
            else
            {
                this.parameter = (T)parameter;
                return _canExecute(this.parameter);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                    NativeHooks.Current.AddCanExecuteRequerySuggested(value);
            }
            remove
            {
                if (_canExecute != null)
                    NativeHooks.Current.RemoveCanExecuteRequerySuggested(value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
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
        private bool _isEnabled;
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

        //public bool IsEnabled
        //{
        //    get 
        //    {
        //        bool enabled = _canExecute();
        //        if (enabled!=_isEnabled)
        //        {
        //            _isEnabled = enabled;
        //            if (CanExecuteChanged != null)
        //            {
        //                CanExecuteChanged(this, EventArgs.Empty);
        //            }
        //        }
                
        //        return _isEnabled;
        //    }
        //    set
        //    {
        //        _isEnabled = value;
        //    }
        //}

        #endregion // Constructors

        #region ICommand Members

        /// <summary>
        /// Can Exceute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null) return true;
            else return _canExecute();
        }

        /// <summary>
        /// Event Handler for CanExecuteChanged
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                    NativeHooks.Current.AddCanExecuteRequerySuggested(value);
            }
            remove
            {
                if (_canExecute != null)
                    NativeHooks.Current.RemoveCanExecuteRequerySuggested(value);
            }
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _execute();
        }

        #endregion // ICommand Members
    }
}
