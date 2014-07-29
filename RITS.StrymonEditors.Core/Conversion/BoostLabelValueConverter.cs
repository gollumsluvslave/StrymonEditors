using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.Conversion
{
    /// <summary>
    /// Implementation of <see cref="IValueLabelConverter"/> for the Boost parameter
    /// Handles the conversion of the value (0,1,2) for 
    /// the Boost control, turning it into a string between -3.0db and +3.0db
    /// </summary>
    public class BoostValueLabelConverter : IValueLabelConverter
    {
        double min = -3.0;
        double max = 3.0;

        /// <summary>
        /// 
        /// </summary>
        public BoostValueLabelConverter()
        {
        }

        /// <summary>
        /// Converts a boost value into a textual label
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ValueToLabel(int value)
        {
            double incr = Math.Round((value * 0.1), 1);
            double val = (min + incr);
            if (val <= min) val = min;
            if (val >= max) val = max;

            string sign = "";
            sign = val > 0 ? "+" : "";
            return string.Format("{0}{1} db", sign, val.ToString("0.0"));

        }
    }
}
