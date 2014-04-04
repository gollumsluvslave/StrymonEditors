using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.Models
{
    public class FineCoarseHelper
    {
        private IFineCoarseValueConverter converter;
        private Parameter fineCoarseParam;

        public FineCoarseHelper(Parameter fineCoarseParam)
        {
            this.fineCoarseParam = fineCoarseParam;
            converter = FineCoarseValueConverterFactory.Create(fineCoarseParam.Definition);
        }

        public void SetCoarseValue(int value)
        {
            fineCoarseParam.Value = value;
            if (!Globals.SynchInProgress) fineCoarseParam.FineValue = converter.CoarseToFine(value);
        }
        public void SetFineValue(int value)
        {
            fineCoarseParam.FineValue = converter.FineToFine(value);
            if(!Globals.SynchInProgress)fineCoarseParam.Value = converter.FineToCoarse(value);
        }

    }
}
