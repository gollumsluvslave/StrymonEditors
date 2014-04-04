using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RITS.StrymonEditor.Models;

namespace RITS.StrymonEditor
{
    /// <summary>
    /// Responsible for creating instances of <see cref="IPotValueConverter"/>
    /// based on the supplied <see cref="Parameter"/> and <see cref="Pot"/>
    /// </summary>
    public static class PotValueConverterFactory
    {
        public static IPotValueConverter Create(Parameter parameter, Pot pot)
        {
            if (parameter == null) return new DefaultPotValueConverter(0);
            if (parameter.Definition.Range != null)
            {
                return new DefaultPotValueConverter(parameter.Definition.Range.MaxValue);
            }
            else return new OptionListPotValueConverter(parameter.Definition.OptionList.Count - 1);
        }
    }

    /// <summary>
    /// Responsible for creating instances of <see cref="IValueLabelConverter"/>
    /// based on the supplied parameter - allows flexible ways
    /// of representing the data in the UI
    /// </summary>
    public static class ValueLabelConverterFactory
    {
        public static IValueLabelConverter Create(Parameter parameter)
        {
            if (parameter.HasFineControl) return new FineValueLabelConverter(parameter);
            if (parameter.Name == "Boost") return new BoostValueLabelConverter();
            if (parameter.Definition.OptionList != null && parameter.Definition.OptionList.Count > 0) return new OptionValueLabelConverter(parameter.Definition.OptionList);
            return new EchoValueLabelConverter();
        }
    }

    /// <summary>
    /// Responsible for creating instances of <see cref="IFineCoarseValueConverter"/>
    /// Based on the supplied ParameterDef. 
    /// Allows some flexibility for Mobius and BigSky to function differently
    /// </summary>
    public static class FineCoarseValueConverterFactory
    {
        public static IFineCoarseValueConverter Create(ParameterDef definition)
        {
            // TODO need different factory for Mobius where FineCoarse conversions have different ranges...
            return new DefaultFineCoarseValueConverter(definition);
        }
    }
}
