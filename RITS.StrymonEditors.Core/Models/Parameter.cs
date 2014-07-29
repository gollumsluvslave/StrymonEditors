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
    /// Represents a parameter used in the 3 <see cref="StrymonPedal"/>s
    /// </summary>
    public class Parameter
    {
        public ParameterDef Definition { get; set; }

        /// <summary>
        /// Returns the name of this parameter
        /// Delegates to the <see cref="ParameterDef"/>
        /// </summary>
        public string Name
        {
            get 
            {
                if (Definition == null) return "undefined";
                return Definition.Name; 
            
            }
            set { }
        }

        /// <summary>
        /// Specifies / sets what the name of the <see cref="StrymonPedal"/> that this parameter is sourced from
        /// </summary>
        public string ContextPedalName { get; set; }

        /// <summary>
        /// Gets / sets the value of this parameter
        /// </summary>
        private int _value;
        public int Value 
        {
            get { return _value; }
            set 
            { 
                _value = value;
            }
        }

        /// <summary>
        /// Gets or sets the 'fine' value for this parameter
        /// This is only relevant to the parameters associated with the Fine/Coarse <see cref="Pot"/>s
        /// </summary>
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

        /// <summary>
        /// Returns the SysExOffset for this parameter
        /// Delegates to the <see cref="ParameterDef"/>
        /// </summary>
        public int SysExOffset
        {
            get { return Definition.SysExOffset; }
            set { }
        }

        
        /// <summary>
        /// Returns a textual label for the name of this parameter
        /// This allows the static name to be overridden and provide better information to the user
        /// Relevant for both dynamic parameters in Mobius and BigSky - Param1, Param2
        /// and also for specific machines such as dTape on Timeline where Grit is actually 'Tape Bias'
        /// 
        /// TODO - rename to 'NameLabel' to make explicit
        /// 
        /// </summary>
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

        /// <summary>
        /// Returns a textual label for the underlying value of this parameter
        /// NB this is very different from the Label property
        /// </summary>
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

        /// <summary>
        /// Based on the <see cref="ParameterDef"/>, returns whether or not this parameter has a fine control element
        /// </summary>
        public bool HasFineControl
        {
            get { return Definition.HasFineControl; }
        }

        /// <summary>
        /// Specifies the Id of the <see cref="Pot"/> that the parameter has been assigned to for the dynamic pot function
        /// in Mobius and BigSky
        /// </summary>
        public int DynamicPotIdAssigned { get; set; }

        /// <summary>
        /// Flag that indicates whether or not the last change was made using the Fine Encoder
        /// as opposed to the Coarse 'pot'
        /// </summary>
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

        /// <summary>
        /// Returns the CC Value to be used for the parameter on an encoder CC change
        /// </summary>
        public int FineEncoderCCValue
        {
            get
            {
                if (_prevfineValue >= _fineValue) return 1;
                else return 0;
            }
        }

        /// <summary>
        /// Specifies whether the last change was a 'direct entry' change or not
        /// NB - the reason for this flag is that direct entry values need a more complex
        /// series of CC messages to synchronise the pedal 
        /// </summary>
        public bool DirectEntryChange { get; set; }

        /// <summary>
        /// Returns this instance as a more lightweight <see cref="XmlParameter"/> representation
        /// </summary>
        /// <returns></returns>
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
    }


}
