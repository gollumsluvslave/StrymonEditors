using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.ViewModels
{
    public class DirectEntryViewModel : DialogViewModel
    {
        public DirectEntryViewModel(string fineValue)
        {
            Title = "Enter Fine Parameter Value";
            Text = fineValue;
            NotifyType = ViewModelMessages.DirectEntryValueEntered;

        }

        public override bool InputInvalid(string text)
        {
            if (!Globals.IsBPMModeActive)
            {
                int result;
                if (!int.TryParse(text, out result))
                {
                    return true;
                }
            }
            else
            {
                double result;
                if (!double.TryParse(text, out result))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
