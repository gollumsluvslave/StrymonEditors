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
            Children = new BindableCollection<MenuItemViewModel>();
        }

        /// <summary>
        /// Indicates this item should be a separator
        /// </summary>
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

        /// <summary>
        /// The text for the menu item
        /// </summary>
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

        /// <summary>
        /// The input gesture text for the menu item
        /// </summary>
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

        /// <summary>
        /// The tag for this menu item
        /// </summary>
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

        /// <summary>
        /// Whether this menu item is enabled
        /// </summary>
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

        /// <summary>
        /// Whether this menu item is checked
        /// </summary>
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

        /// <summary>
        /// Whether this menu item can be checked
        /// </summary>
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

        /// <summary>
        /// The ICommand for this menu item
        /// </summary>
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

        /// <summary>
        /// ICommand to be executed whether the checkable menu item is set to checked
        /// </summary>
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

        /// <summary>
        /// ICommand to be executed whether the checkable menu item is set to unchecked
        /// </summary>
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

        /// <summary>
        /// The collection of child menu items
        /// </summary>
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

        /// <summary>
        /// Whether or not this menu item is destructive - NOT USED
        /// </summary>
        public bool IsDestructive { get; set; }
    }
}
