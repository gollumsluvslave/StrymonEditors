using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.Conversion
{
    /// <summary>
    /// Implementation of <see cref="IValueLabelConverter"/> that 
    /// simply echoes back the supplied value as a string with no other alteration
    /// </summary>
    public class EchoValueLabelConverter : IValueLabelConverter
    {
        public EchoValueLabelConverter()
        {
        }

        /// <summary>
        /// Returns the supplied value as a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ValueToLabel(int value)
        {
            return value.ToString();

        }
    }
}
