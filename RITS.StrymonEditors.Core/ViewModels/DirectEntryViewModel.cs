using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RITS.StrymonEditor.Messaging;
namespace RITS.StrymonEditor.ViewModels
{
    /// <summary>
    /// Subclass of <see cref="DialogViewModel"/> used to allow the user to 
    /// enter an exact value for the fine/coarse parameter
    /// </summary>
    public class DirectEntryViewModel : DialogViewModel
    {
        public DirectEntryViewModel(string fineValue)
        {
            Title = "Enter Fine Parameter Value";
            Text = fineValue;
            NotifyType = ViewModelMessages.DirectEntryValueEntered;

        }

        /// <summary>
        /// Ensures that the values entered in the <see cref="Views.Dialog"/> 
        /// are numeric 9int / double depending on the BPM flag
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
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
