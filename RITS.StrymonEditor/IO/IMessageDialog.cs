using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.IO
{
    public interface IMessageDialog
    {
        bool ShowInfo(string message, string caption);
        bool ShowYesNo(string message, string caption);
        bool ShowError(string message, string caption);
    }
}
