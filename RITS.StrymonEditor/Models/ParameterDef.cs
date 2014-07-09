using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// The definition of a parameter across the 3 pedals
    /// This is loaded from configuration initially, but various other properties are inferred / set
    /// </summary>
    [Serializable]
    public class ParameterDef : NameBase
    {
        private Range _origFineRange;
        private Range _origCoarseRange;

        [XmlAttribute]
        public int PotId { get; set; }

        /// <summary>
        /// A <see cref="Range"/> that represents the range of values that can be stored in the pedal for the parameter
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// A <see cref="Range"/> that represents the absolute range of 'fine' values that can be stored in the pedal for the parameter
        /// This is only applicable to the parameters that are linked to the Fine/Coarse controls on the pedals
        /// </summary>
        public Range FineRange { get; set; }

        /// <summary>
        /// A <see cref="Range"/> that represents the range of 'fine' values that can be stored 
        /// in the pedal for the parameter via the 'Coarse' <see cref="Pot"/>
        /// NB - This was required for Mobius as it is different to the other two pedals where the range of values possible via the coarse 
        /// pot is not the absolute range of values - the fine encoder control can reach the absolute values for these types of machine
        /// This is only applicable to the parameters that are linked to the Fine/Coarse controls on the pedals
        /// <see cref="RangeOverride"/> for additional information
        /// 
        /// TODO - moer explicit naming here might be worthwhile
        /// e.g. FineEncoderRange, CoarsePotRange etc
        /// 
        /// </summary>
        public Range CoarseRange { get; set; }

        /// <summary>
        /// List of <see cref="Option"/>s applicable to this type of parameter 
        /// </summary>
        public List<Option> OptionList { get; set; }

        /// <summary>
        /// The SysExOffset for this definition
        /// This value is calculated when a <see cref="StrymonPreset"/> is built up
        /// These offsets follow a sequential, order based pattern based on the number parameters relevant for the context <see cref="StrymonMachine"/>
        /// 1. Pot / ControlParameters
        /// 2. 'Hidden' Machine specific Parameters
        /// 3. Common Parameters across all machines
        /// However a number of common parameters require various offsets to be added handled by the PostOffset property
        /// </summary>
        private int _sysExOffset;
        [XmlIgnore]
        public int SysExOffset
        {
            get 
            {
                if (PotId > 5) return PotId + 1;
                else if (PotId == 2) return 3;
                else if (PotId == 3) return  2;
                else if(PotId > 0)return PotId;
                else
                {
                    return _sysExOffset;
                }
            }
            set 
            {
                if (PotId == 0)
                {
                    _sysExOffset = value;
                }
            }
        }

        /// <summary>
        /// Returns whether or not this definition is a refernce to a more complete definition in the parent pedal xml file
        /// Mechanism is simply to reduce redundant data entry in the xml definitions
        /// </summary>
        [XmlIgnore]
        public bool IsRef
        {
            get
            {
                return Range == null && (OptionList == null || OptionList.Count == 0);
            }
        }

        /// <summary>
        /// Returns whether or not this isntance is related to a 'fine/caosre' control <see cref="Pot"/> or <see cref="Parameter"/>
        /// </summary>
        [XmlIgnore]
        public bool HasFineControl
        {
            get { return PotId == 1; }
        }

        /// <summary>
        /// An offset to apply to the sequential sysex offsets after this isntance
        /// </summary>
        [XmlAttribute]
        public int PostOffset
        {
            get;
            set;
        }

        /// <summary>
        /// The Control Change number that is applicable to this instance
        /// </summary>
        [XmlAttribute]
        public int ControlChangeNo
        {
            get;
            set;
        }

        /// <summary>
        /// Overrides the coarse and fine <see cref="Range"/>s with different ones from the supplied <see cref="RangeOverride"/>
        /// This is utilised for more complex interactions where another parameter's value will trigger a change to the <see cref="Range"/>
        /// </summary>
        /// <param name="rangeOverride"></param>
        public void OverrideRanges(RangeOverride rangeOverride)
        {
            if (_origFineRange == null) _origFineRange = FineRange;
            FineRange = rangeOverride.Range; ;
            if (_origCoarseRange == null) _origCoarseRange = CoarseRange;
            CoarseRange = rangeOverride.CoarseRange;
        }
        
        /// <summary>
        /// Resets both the coarse and fine <see cref="Range"/>s back to their original values
        /// This is utilised for more complex interactions where another parameter's value will trigger a change to the <see cref="Range"/>
        /// </summary>
        public void ResetRange()
        {
            if (_origFineRange != null)
            {
                FineRange = _origFineRange;
            }
            CoarseRange = _origCoarseRange;
        }
    }
}
