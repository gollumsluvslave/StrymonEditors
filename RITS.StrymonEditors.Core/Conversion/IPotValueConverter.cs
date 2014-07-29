using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.Conversion
{
    /// <summary>
    /// Interface to allow different implementations of converting an angle to a pot value
    /// </summary>
    public interface IPotValueConverter
    {
        /// <summary>
        /// Defines the conversion of a value to an angle
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        double ValueToAngle(int value);

        /// <summary>
        /// Defines the conversion of an angle to a value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        int AngleToValue(double value);
    }
}
