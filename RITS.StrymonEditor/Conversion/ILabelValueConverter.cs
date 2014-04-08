using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.Conversion
{
    public interface IValueLabelConverter
    {
        string ValueToLabel(int value);
    }
}
