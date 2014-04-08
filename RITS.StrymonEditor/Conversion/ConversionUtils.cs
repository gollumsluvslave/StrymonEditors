using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.Conversion
{
    public static class ConversionUtils
    {
        public static double ConvertMillisecondsToBPM(int ms)
        {
            double bpm = 60000 / Convert.ToDouble(ms);
            return Math.Round(bpm, 1);
        }

        public static int ConvertBPMToMilliseconds(double bpm)
        {
            return Convert.ToInt32(60000 / bpm);
        }
        public static double ConvertMilliHzToBPM(int mhz)
        {
            double hz = (mhz / 1000);
            double bpm = hz * 60;
            return Math.Round(bpm, 1);
        }

        public static int ConvertBPMToMilliHz(double bpm)
        {
            return Convert.ToInt32(bpm / 60) * 1000;
        }
    }
}
