using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.IO
{
    public interface IFileDialog
    {
        string DefaultExt { get; set; }
        string Filter { get; set; }
        string FileName { get; set; }
        bool? ShowDialog();
    }
}
