using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.Conversion
{
    /// <summary>
    /// OptionList implementation of <see cref="IPotValueConverter"/>
    /// This is used to handle dynamic pot assignments in Mobius and BigSky 
    /// that do not have a 0-127 value range 
    /// </summary>
    public class OptionListPotValueConverter : IPotValueConverter
    {
        private double max;
        private double valueToAngleRatio;
        private double angleToValueRatio;
        public OptionListPotValueConverter(int maxValue)
        {
            max = maxValue;
            valueToAngleRatio = Convert.ToDouble(290) / max;
            angleToValueRatio = max / 290;
        }
        public double ValueToAngle(int value)
        {
            if (value < 0) return 0;
            if (value >= max)
            {
                return 290;
            }
            return value * valueToAngleRatio;
        }
        public int AngleToValue(double value)
        {
            if (value < 0) return 0;
            if (value >= 290) return Convert.ToInt32(max);
            return Convert.ToInt32(value * angleToValueRatio);
        }
    }
}
