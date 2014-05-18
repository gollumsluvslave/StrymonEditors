using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RITS.StrymonEditor.Conversion;
using RITS.StrymonEditor.ViewModels;
namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Helper class is responsible for synchronising the coarse 
    /// and fine pots and any edits to the underlying shared parameter
    /// </summary>
    public class FineCoarseSynchroniser
    {
        private IFineCoarseValueConverter converter;
        private Parameter fineCoarseParam;
        private PotViewModel coarsePot;
        private PotViewModel finePot;
        public bool synchInProgress;
        public FineCoarseSynchroniser(PotViewModel coarse, PotViewModel fine)
        {
            coarsePot = coarse;
            finePot = fine;
            fineCoarseParam = coarse.LinkedParameter;
            converter = FineCoarseValueConverterFactory.Create(fineCoarseParam.Definition);
        }

        /// <summary>
        /// Sets the coarse pot value (0-127)
        /// </summary>
        /// <param name="value"></param>
        public void SetCoarseValue(int value)
        {
            fineCoarseParam.Value = value;
            if (!synchInProgress && !Globals.IsPedalViewLoading)
            {
                synchInProgress = true;
                fineCoarseParam.FineEncoderLastChange = false;
                fineCoarseParam.FineValue = converter.CoarseToFine(value);
                finePot.Value = fineCoarseParam.FineValue;
                synchInProgress = false;
            }

        }

        /// <summary>
        /// Sets the fine value - ms, hertz etc
        /// </summary>
        /// <param name="value"></param>
        public void SetFineValue(int value)
        {
            fineCoarseParam.FineValue = converter.FineToFine(value);
            if (!synchInProgress)
            {
                synchInProgress = true;
                fineCoarseParam.FineEncoderLastChange = true;
                fineCoarseParam.Value = converter.FineToCoarse(value);
                coarsePot.Value = fineCoarseParam.Value;
                synchInProgress = false;
            }


        }

    }
}
