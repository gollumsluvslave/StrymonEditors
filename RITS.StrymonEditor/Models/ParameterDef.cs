using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// The definition of a parameter across the 3 pedals
    /// This is loaded from configuration
    /// </summary>
    [Serializable]
    public class ParameterDef : NameBase
    {
        private Range _origFineRange;
        private Range _origCoarseRange;
        [XmlAttribute]
        public int PotId { get; set; }

        public Range Range { get; set; }
        public Range FineRange { get; set; }
        public Range CoarseRange { get; set; }

        public List<Option> OptionList { get; set; }

        private int _sysExOffset;
        [XmlIgnore]
        public int SysExOffset
        {
            get { return _sysExOffset; }
            set { _sysExOffset = value; }
        }

        [XmlIgnore]
        public bool IsRef
        {
            get
            {
                return Range == null && (OptionList == null || OptionList.Count == 0);
            }
        }

        [XmlIgnore]
        public bool HasFineControl
        {
            get { return PotId == 1; }
        }

        [XmlAttribute]
        public int PostOffset
        {
            get;
            set;
        }

        [XmlAttribute]
        public int ControlChangeNo
        {
            get;
            set;
        }

        public void OverrideRanges(RangeOverride rangeOverride)
        {
            if (_origFineRange == null) _origFineRange = FineRange;
            FineRange = rangeOverride.Range; ;
            if (_origCoarseRange == null) _origCoarseRange = CoarseRange;
            CoarseRange = rangeOverride.CoarseRange;
        }
 
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
