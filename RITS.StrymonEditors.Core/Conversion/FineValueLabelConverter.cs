using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.Conversion;

namespace RITS.StrymonEditor.Conversion
{
    /// <summary>
    /// Implementation of <see cref="IValueLabelConverter"/> that
    /// converts 'fine' values such as Milliseconds and Hertz to a textual representation
    /// 
    /// TODO - split into separate implementations depending on pedal and BPM setting?
    /// 
    /// </summary>
    public class FineValueLabelConverter:IValueLabelConverter
    {
        private Parameter _parameter;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public FineValueLabelConverter(Parameter parameter)
        {
            _parameter = parameter;
        }

        /// <summary>
        /// Returns the label  for the supplied 'fine' value 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ValueToLabel(int value)
        {
            // Possible reasons for inheritance here - makes serialization a bit more complex though
            if (_parameter.ContextPedalName == StrymonPedal.Mobius_Name)
            {
                if (Globals.IsBPMModeActive) return ConversionUtils.ConvertMilliHzToBPM(_parameter.FineValue).ToString() + " bpm";
                double hz = Convert.ToDouble(_parameter.FineValue) / 1000;
                hz = Math.Round(hz, 2);
                return hz + " Hz";
            }
            else
            {
                if (Globals.IsBPMModeActive) return ConversionUtils.ConvertMillisecondsToBPM(_parameter.FineValue).ToString() + " bpm";
                return _parameter.FineValue.ToString() + " ms";
            }
        }
    }
}
