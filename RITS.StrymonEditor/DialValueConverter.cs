using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RITS.StrymonEditor.Models;

namespace RITS.StrymonEditor
{
    public interface IPotValueConverter
    {
        double ValueToAngle(int value);
        int AngleToValue(double value);
    }

    public static class PotValueConverterFactory
    {
        public static IPotValueConverter Create(Parameter parameter, Pot pot)
        {
            if (parameter.Definition.Range != null)
            {
                return new DefaultPotValueConverter(parameter.Definition.Range.MaxValue);
            }
            else  return new DefaultPotValueConverter(parameter.Definition.OptionList.Count-1);
        }
    }

    public class DefaultPotValueConverter : IPotValueConverter
    {
        private double max;
        private double valueToAngleRatio;
        private double angleToValueRatio;
        public DefaultPotValueConverter(int maxValue)
        {
            max = maxValue;
            valueToAngleRatio = Convert.ToDouble(290) / max;
            angleToValueRatio = max / 290;
        }
        public double ValueToAngle(int value)
        {
            if (value >= max)
            {
                return 290;
            }
            return Globals.PotValueMap.GetAngleForValue(value);
        }
        public int AngleToValue(double value)
        {
            if (value >= 290) return Convert.ToInt32(max);
            return Globals.PotValueMap.GetValueForAngle(value);
        }
    }


    public interface ILabelValueConverter
    {
        string ValueToLabel(int value);
    }
    public static class LabelValueConverterFactory
    {
        public static ILabelValueConverter Create(Parameter parameter)
        {
            if (parameter.Name == "Boost")
            {
                return new BoostLabelValueConverter();
            }
            else if (parameter.Definition.OptionList.Count > 0)
            {
                return new OptionValueConverter(parameter.Definition.OptionList);
            }
            else return null;
        }
    }


    public class BoostLabelValueConverter : ILabelValueConverter
    {
        double min = -3.0;
        public BoostLabelValueConverter()
        {
        }
        public string ValueToLabel(int value)
        {
            double incr = Math.Round((value * 0.1),1);
            double val = (min + incr);
            string sign= "";
            sign = val > 0 ? "+" : "";
            return string.Format("{0}{1} db",sign,val.ToString("0.0"));

        }
    }
    public class OptionValueConverter : ILabelValueConverter
    {
        private List<Option> optionList;
        public OptionValueConverter(List<Option> optionList)
        {
            this.optionList = optionList;
        }
        public string ValueToLabel(int value)
        {
            var opt = optionList.FirstOrDefault(x => x.Value == value);
            if (opt == null)
            {
                if (value > optionList.Max(x => x.Value))
                {

                    opt = optionList.Last();
                }
            }
            return opt.Name;

        }
    }

    public interface IFineCoarseValueConverter
    {
        int FineToCoarse(int fineValue);
        int CoarseToFine(int coarseValue);
        int FineToFine(int fineValue);
    }

    public static class FineCoarseValueConverterFactory
    {
        public static IFineCoarseValueConverter Create(ParameterDef definition)
        {
            return new TimeFineCoarseValueConverter(definition);
        }
    }

    public class TimeFineCoarseValueConverter: IFineCoarseValueConverter
    {
        private ParameterDef paramDef;
        
        public TimeFineCoarseValueConverter(ParameterDef definition)
        {
            paramDef = definition;
            
        }
        public int FineToCoarse(int fineValue)
        {
            if (fineValue >= paramDef.FineRange.MaxValue) return paramDef.Range.MaxValue;
            if (fineValue <= paramDef.FineRange.MinValue) return paramDef.Range.MinValue;
            else
            {
                var potValueItem = Globals.PotValueMap.LookupMap.FirstOrDefault(x=>x.FineValue > fineValue);
                if (potValueItem != null)
                {
                    var prev = Globals.PotValueMap.LookupMap.FirstOrDefault(x => x.Value == potValueItem.Value - 1);
                    if (prev != null)
                    {
                        return prev.Value;
                    }
                }
                return paramDef.Range.MaxValue;
            }
        }

        public int FineToFine(int fineValue)
        {
            if (fineValue >= paramDef.FineRange.MaxValue) return paramDef.FineRange.MaxValue;
            if (fineValue <= paramDef.FineRange.MinValue) return paramDef.FineRange.MinValue;
            return fineValue;
        }
        
        // Suspect? If we already have the absolute fine value, we don't really need to convert the coarse??
        public int CoarseToFine(int coarseValue)
        {
            if (coarseValue >= paramDef.Range.MaxValue) return paramDef.FineRange.MaxValue;
            if (coarseValue <= paramDef.Range.MinValue) return paramDef.FineRange.MinValue;
            else
            {
                var potValueItem = Globals.PotValueMap.LookupMap.FirstOrDefault(x => x.Value == coarseValue);
                return potValueItem.FineValue;
            }
            
        }
    }
}
