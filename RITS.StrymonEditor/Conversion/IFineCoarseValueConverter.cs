using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.Conversion
{
    /// <summary>
    /// Interface that defines operations for conversion between fine / coarse values
    /// </summary>
    public interface IFineCoarseValueConverter
    {
        /// <summary>
        /// Defines the 'fine' (e.g. 60-2500ms) to 'coarse' (typically 0-127) conversion
        /// </summary>
        /// <param name="fineValue">the fine value to convert into coarse</param>
        /// <returns></returns>
        int FineToCoarse(int fineValue);

        /// <summary>
        /// Defines the 'coarse' (typically 0-127) to 'fine' (e.g. 60-2500ms) conversion
        /// </summary>
        /// <param name="fineValue">the fine value to convert into coarse</param>
        /// <returns></returns>
        int CoarseToFine(int coarseValue);

        /// <summary>
        /// Ensures that fine values respect their configured range boundaries... not well named
        /// 
        /// TODO - Rename / Sanity check
        /// 
        /// </summary>
        /// <param name="fineValue"></param>
        /// <returns></returns>
        int FineToFine(int fineValue);
    }
}
