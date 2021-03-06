﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.Conversion
{
    /// <summary>
    /// Default implementation of <see cref="IPotValueConverter"/>
    /// Dynamic based on the supplied maximum value, delegates most of the 
    /// work to <see cref="Models.PotValueMap"/>
    /// </summary>
    public class DefaultPotValueConverter : IPotValueConverter
    {
        private double max;
        private double valueToAngleRatio;
        private double angleToValueRatio;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxValue"></param>
        public DefaultPotValueConverter(int maxValue)
        {
            max = maxValue;
            valueToAngleRatio = Convert.ToDouble(290) / max;
            angleToValueRatio = max / 290;
        }

        /// <summary>
        /// Conversion of value to angle
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public double ValueToAngle(int value)
        {
            if (value <= 0) return 0;
            if (value >= max)
            {
                return 290;
            }
            // Dynamically assigned pots with smaller max
            if (max < 127) return valueToAngleRatio * value;
            return Globals.PotValueMap.GetAngleForValue(value);
        }

        /// <summary>
        /// Conversion of angle to value
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public int AngleToValue(double angle)
        {
            if (angle <= 0) return 0;
            if (angle >= 290) return Convert.ToInt32(max);
            // Dynamically assigned pots with smaller max
            if (max < 127) return Convert.ToInt32(angle / valueToAngleRatio);
            return Globals.PotValueMap.GetValueForAngle(angle);
        }
    }
}
