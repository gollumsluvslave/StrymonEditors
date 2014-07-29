using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace RITS.StrymonEditor.ViewModels
{
    /// <summary>
    /// View model for generic MenuItem binding
    /// </summary>
    public class MenuItemViewModel : ViewModelBase
    {
        public MenuItemViewModel Parent { get; set; }
        public MenuItemViewModel()
        {
            Children = NativeHooks.Current.CreateList <MenuItemViewModel>();
        }

        private bool isSeparator;
        /// <summary>
        /// Indicates this item should be a separator
        /// </summary>
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
        /// <summary>
        /// The text for the menu item
        /// </summary>
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
        /// <summary>
        /// The input gesture text for the menu item
        /// </summary>
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
        /// <summary>
        /// The tag for this menu item
        /// </summary>
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
        /// <summary>
        /// Whether this menu item is enabled
        /// </summary>
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
        /// <summary>
        /// Whether this menu item is checked
        /// </summary>
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
        /// <summary>
        /// Whether this menu item can be checked
        /// </summary>
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
        /// <summary>
        /// The ICommand for this menu item
        /// </summary>
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
        /// <summary>
        /// ICommand to be executed whether the checkable menu item is set to checked
        /// </summary>
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
        /// <summary>
        /// ICommand to be executed whether the checkable menu item is set to unchecked
        /// </summary>
        public ICommand UncheckedCommand
        {
            get { return uncheckedCommand; }

            set
            {
                uncheckedCommand = value;
                OnPropertyChanged("UncheckedCommand");
            }
        }

        private IList<MenuItemViewModel> _children;
        /// <summary>
        /// The collection of child menu items
        /// </summary>
        public IList<MenuItemViewModel> Children
        {
            get { return _children; }

            set
            {
                _children = value;
                OnPropertyChanged("Children");
            }
        }

        /// <summary>
        /// Whether or not this menu item is destructive - NOT USED
        /// </summary>
        public bool IsDestructive { get; set; }
    }
}
