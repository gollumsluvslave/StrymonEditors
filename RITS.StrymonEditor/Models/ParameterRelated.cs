using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RITS.StrymonEditor.Models
{
    [Serializable]
    public class Parameter //: INotifyPropertyChanged
    {
        public ParameterDef Definition { get; set; }
        [XmlAttribute]
        public string Name
        {
            get 
            {
                if (Definition == null) return "undefined";
                return Definition.Name; 
            
            }
            set { }
        }

        private int _value;
        [XmlAttribute]
        public int Value 
        {
            get { return _value; }
            set 
            { 
                _value = value;
            }
        }

        private int _fineValue;
        [XmlIgnore]
        public int FineValue
        {
            get
            {
                return _fineValue;
            }
            set
            {
                _fineValue = value ;
            }
        }

        public int SysExOffset
        {
            get { return Definition.SysExOffset; }
            set { }
        }
        public XmlParameter ToXmlParameter()
        {
            return new XmlParameter{Name=this.Name, Value=this.Value};
        }


        


        [XmlIgnore]
        public string Label
        {
            get
            {
                var c = LabelValueConverterFactory.Create(this);
                if (c != null)
                {
                    return c.ValueToLabel(Value);
                }
                return null;
            }
            set
            {
                
            }
        }

        public bool HasFineControl
        {
            get { return Definition.HasFineControl; }
        }
    }

    [Serializable]
    public class ParameterDef : NameBase
    {
        private Range _origRange;
        [XmlAttribute]
        public string PotId { get; set; }

        public Range Range {get;set;}
        public Range FineRange { get; set; }
        
        public List<Option> OptionList { get; set; }

        private int _sysExOffset;

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
            get { return PotId == "1"; }        
        }

        [XmlAttribute]
        public int PostOffset
        {
            get;
            set;
        }
        
        public void OverrideFineRange(Range source)
        {
            if (_origRange == null) _origRange = FineRange;
            FineRange = source;            
        }

        public void ResetRange()
        {
            if (_origRange != null)
            {
                FineRange = _origRange;
            }
        }
    }

    [Serializable]
    public class XmlParameter:NameBase
    {
        public int Value { get; set; }
    }
}
