using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Defines a 'machine' - e.g. dTape, Digital, Cloud etc
    /// </summary>
    [Serializable]
    public class StrymonMachine: NameBase
    {        
        /// <summary>
        /// Numeric value/id representing the machine/type
        /// </summary>
        [XmlAttribute]
        public int Value { get; set; }

        /// <summary>
        /// List of machine specific parameters
        /// </summary>
        public List<ParameterDef> MachineParameters { get; set; }

        /// <summary>
        /// Any machine specific pot config
        /// </summary>
        public List<Pot> Pots { get; set; }

        /// <summary>
        /// Map to override how pots travel
        /// </summary>
        public PotValueMap PotValueMap { get; set; }

        [XmlIgnore]
        public int CCValue
        {
            get
            {
                if (Value >= 7) return Value - 7;
                else
                {
                    return Value + 5;
                }
            }
        }
    }
}
