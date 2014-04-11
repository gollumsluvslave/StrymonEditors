using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RITS.StrymonEditor.Messaging;
namespace RITS.StrymonEditor.ViewModels
{
    /// <summary>
    /// ViewMode responseible for the preset rename dialog 
    /// </summary>
    public class PresetRenameViewModel:DialogViewModel
    {
        /// <summary>
        /// .ctir passing in the current preset name
        /// </summary>
        /// <param name="name"></param>
        public PresetRenameViewModel(string name)
        {
            Title = "Rename Preset";
            Text = name;
            NotifyType = ViewModelMessages.PresetRenamed;
        }


    }
}
