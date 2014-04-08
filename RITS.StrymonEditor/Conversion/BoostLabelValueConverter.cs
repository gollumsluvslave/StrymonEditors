using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.Conversion
{
    /// <summary>
    /// Handles the conversion of the value (0,1,2) for 
    /// the Boost control, turning it into a string between -3.0db and +3.0db
    /// </summary>
    public class BoostValueLabelConverter : IValueLabelConverter
    {
        double min = -3.0;
        public BoostValueLabelConverter()
        {
        }
        public string ValueToLabel(int value)
        {
            double incr = Math.Round((value * 0.1), 1);
            double val = (min + incr);
            string sign = "";
            sign = val > 0 ? "+" : "";
            return string.Format("{0}{1} db", sign, val.ToString("0.0"));

        }
    }
}
