using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RITS.StrymonEditor.Conversion;

namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Any parameter across the Strymon pedals, including it's value
    /// </summary>
    public class Parameter
    {
        public ParameterDef Definition { get; set; }
        public string Name
        {
            get 
            {
                if (Definition == null) return "undefined";
                return Definition.Name; 
            
            }
            set { }
        }

        public string ContextPedalName { get; set; }
        private int _value;
        public int Value 
        {
            get { return _value; }
            set 
            { 
                _value = value;
            }
        }

        private int _prevfineValue;
        private int _fineValue;
        public int FineValue
        {
            get
            {
                return _fineValue;
            }
            set
            {
                _prevfineValue = _fineValue;
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
            if (HasFineControl)
            {
                return new XmlParameter { Name = this.Name, Value = this.Value, FineValue = this.FineValue };
            }
            else
            {
                return new XmlParameter { Name = this.Name, Value = this.Value };
            }
        }
        
        public string Label
        {
            get
            {
                var c = ValueLabelConverterFactory.Create(this);
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

        public string ValueLabel
        {
            get
            {
                var c = ValueLabelConverterFactory.Create(this);
                return c.ValueToLabel(Value);
            }
            set
            {

            }
        }

        public bool HasFineControl
        {
            get { return Definition.HasFineControl; }
        }

        public int DynamicPotIdAssigned { get; set; }

        private bool _fineEncoderLastChange;

        public bool FineEncoderLastChange
        {
            get 
            {
                return _fineEncoderLastChange;
            }
            set
            {
                if (HasFineControl)
                {
                    _fineEncoderLastChange = value;
                }
            }
        }

        public int FineEncoderCCValue
        {
            get
            {
                if (_prevfineValue >= _fineValue) return 1;
                else return 0;
            }
        }

        public bool DirectEntryChange { get; set; }
    }


}
