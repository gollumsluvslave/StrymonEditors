using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RITS.StrymonEditor.Models;
namespace RITS.StrymonEditor.Conversion
{
    /// <summary>
    /// Handles the conversion of the value (0,1,2) for a 
    /// drop down option to its textual equivalent in the config
    /// </summary>
    public class OptionValueLabelConverter : IValueLabelConverter
    {
        private List<Option> optionList;
        public OptionValueLabelConverter(List<Option> optionList)
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
                else if (value < optionList.Min(x => x.Value))
                {
                    opt = optionList.First();
                }
            }
            if (opt == null)
            {
                throw new ArgumentOutOfRangeException(string.Format("Value {0} is not valid for the supplied Option list", value));
            }
            return opt.Name;
        }
    }
}
