using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor
{
    /// <summary>
    /// Interface that defines operations for conversion between fine / coarse values
    /// </summary>
    public interface IFineCoarseValueConverter
    {
        int FineToCoarse(int fineValue);
        int CoarseToFine(int coarseValue);
        int FineToFine(int fineValue);
    }
}
