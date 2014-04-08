using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RITS.StrymonEditor.Models;
namespace RITS.StrymonEditor.Conversion
{
    /// <summary>
    /// Default implementation of <see cref="IFineCoarseValueConverter"/>
    /// Similar to <see cref="DefaultPotValueConverter"/> this implementation
    /// depends on <see cref="PotValueMap"/>
    /// </summary>
    public class DefaultFineCoarseValueConverter : IFineCoarseValueConverter
    {
        private ParameterDef paramDef;
        private Range coarseRange;
        private Range fineRange;
        public DefaultFineCoarseValueConverter(ParameterDef definition)
        {
            paramDef = definition;
            fineRange = paramDef.FineRange;
            coarseRange = paramDef.CoarseRange == null ? paramDef.FineRange : paramDef.CoarseRange;

        }
        
        /// <summary>
        /// Convert a FineValue (flexible) to Coarse (0-127)
        /// </summary>
        /// <param name="fineValue"></param>
        /// <returns></returns>
        public int FineToCoarse(int fineValue)
        {
            if (fineValue >= coarseRange.MaxValue) return paramDef.Range.MaxValue;
            if (fineValue <= coarseRange.MinValue) return paramDef.Range.MinValue;
            else
            {
                var potValueItem = Globals.PotValueMap.LookupMap.FirstOrDefault(x => x.FineValue > fineValue);
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

        /// <summary>
        /// Ensures that any finevalue is within the bounds of it's currently defined range
        /// TODO - Maybe rename??
        /// </summary>
        /// <param name="fineValue"></param>
        /// <returns></returns>
        public int FineToFine(int fineValue)
        {
            if (fineValue >= paramDef.FineRange.MaxValue) return paramDef.FineRange.MaxValue;
            if (fineValue <= paramDef.FineRange.MinValue) return paramDef.FineRange.MinValue;
            return fineValue;
        }

        /// <summary>
        /// Convert a Coarse value (0-127) to Fine (flexible) for the currently loaded preset/pedal
        /// </summary>
        /// <param name="coarseValue"></param>
        /// <returns></returns>
        public int CoarseToFine(int coarseValue)
        {
            if (coarseValue >= paramDef.Range.MaxValue) return coarseRange.MaxValue;
            if (coarseValue <= paramDef.Range.MinValue) return coarseRange.MinValue;
            else
            {
                var potValueItem = Globals.PotValueMap.LookupMap.FirstOrDefault(x => x.Value == coarseValue);
                return potValueItem.FineValue;
            }

        }
    }
}
