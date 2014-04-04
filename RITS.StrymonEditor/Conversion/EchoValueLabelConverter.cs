using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor
{
    /// <summary>
    /// Handles the conversion of the value (0,1,2) for 
    /// the Boost control, turning it into a string between -3.0db and +3.0db
    /// </summary>
    public class EchoValueLabelConverter : IValueLabelConverter
    {
        public EchoValueLabelConverter()
        {
        }
        public string ValueToLabel(int value)
        {
            return value.ToString();

        }
    }
}
