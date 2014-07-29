using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.Conversion
{
    /// <summary>
    /// Conversion interface that defines the contract for converting values into textual 'label' equivalents
    /// </summary>
    public interface IValueLabelConverter
    {
        /// <summary>
        /// Method that returns the textual 'label' equivalent for the supplied value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string ValueToLabel(int value);
    }
}
