using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RITS.StrymonEditor.Commands;
using RITS.StrymonEditor.Messaging;
namespace RITS.StrymonEditor.ViewModels
{

    /// <summary>
    /// An absract viewmodel that defines a type of text input operation
    /// Used in conjunction with the following
    /// The <see cref="Views.Dialog"/> control / window
    /// The <see cref="Views.IModalDialog"/> interface
    /// </summary>
    public abstract class DialogViewModel:ViewModelBase
    {
        /// <summary>
        /// A delegate to invoke to close any associated view / dialog
        /// </summary>
        public Action CloseAction { get; set; }

        /// <summary>
        /// The Title for this dialog
        /// </summary>
        private string title;
        public virtual string Title 
        {
            get { return title; }
            set
            { 
                title = value;
                OnPropertyChanged("Title");
            }
        }

        /// <summary>
        /// The text of the dialog
        /// </summary>
        private string text;
        public virtual string Text
        {
            get { return text; }
            set
            {
                text = value;
                OnPropertyChanged("Text");
            }
        }

        /// <summary>
        /// The type of <see cref="ViewModelMessages"/> message to notify upon completion
        /// </summary>
        public virtual ViewModelMessages NotifyType { get; set; }

        /// <summary>
        /// A command to execute upon completion
        /// </summary>
        public new virtual RelayCommand Complete
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    Mediator.NotifyColleagues(NotifyType, Text);
                    CloseAction();
                }));
            }
        }

        /// <summary>
        /// Defines a method for subclasses to implement to specify whether the input is valid
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual bool InputInvalid(string text)
        {
            return false;
        }
    }
}
