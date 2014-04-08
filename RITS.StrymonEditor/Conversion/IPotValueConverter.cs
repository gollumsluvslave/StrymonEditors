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
        double ValueToAngle(int value);
        int AngleToValue(double value);
    }
}
