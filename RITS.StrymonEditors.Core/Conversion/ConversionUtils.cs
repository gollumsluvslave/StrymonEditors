using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.Conversion
{
    /// <summary>
    /// Helper conversion methods
    /// </summary>
    public static class ConversionUtils
    {
        /// <summary>
        /// Return the BPM equivalent of the supplied ms value 
        /// </summary>
        /// <param name="ms">Time in milliseconds to be converted</param>
        /// <returns></returns>
        public static double ConvertMillisecondsToBPM(int ms)
        {
            double bpm = 60000 / Convert.ToDouble(ms);
            return Math.Round(bpm, 1);
        }

        /// <summary>
        /// Return the Millisecond equivalent of the supplied BPM value 
        /// </summary>
        /// <param name="bpm">BPM value to be converted</param>
        /// <returns></returns>
        public static int ConvertBPMToMilliseconds(double bpm)
        {
            return Convert.ToInt32(60000 / bpm);
        }
        
        /// <summary>
        /// Return the BPM equivalent of the supplied MilliHz value
        /// </summary>
        /// <param name="mhz"> The millihz value to be converted</param>
        /// <returns></returns>
        public static double ConvertMilliHzToBPM(int mhz)
        {
            double hz = (mhz / 1000.0);
            double bpm = hz * 60;
            return Math.Round(bpm, 1);
        }

        /// <summary>
        /// Return the Millihz equivalent of the supplied BPM value 
        /// </summary>
        /// <param name="bpm">BPM value to be converted</param>
        /// <returns></returns>
        public static int ConvertBPMToMilliHz(double bpm)
        {
            return Convert.ToInt32(bpm / 60) * 1000;
        }
    }
}
