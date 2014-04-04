using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.ViewModels
{
    public class PresetRenameViewModel:DialogViewModel
    {
        public PresetRenameViewModel(string name)
        {
            Title = "Rename Preset";
            Text = name;
            NotifyType = ViewModelMessages.PresetRenamed;
        }


    }
}
