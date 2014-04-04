using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.ViewModels
{
    public abstract class DialogViewModel:ViewModelBase
    {
        public Action CloseAction { get; set; }

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

        public virtual ViewModelMessages NotifyType { get; set; }

        public virtual RelayCommand Complete
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

        public virtual bool InputInvalid(string text)
        {
            return false;
        }
    }
}
