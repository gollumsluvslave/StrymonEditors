using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Defines a 'Pot' or dial on the Strymon pedals
    /// </summary>
    [Serializable]
    public class Pot
    {
        [XmlAttribute]
        public int Id { get; set; }

        [XmlAttribute]
        public string DefaultLabel { get; set; }

        [XmlAttribute]
        public string Label { get; set; }

        [XmlAttribute]
        public int Left { get; set; }

        [XmlAttribute]
        public int Top { get; set; }

        [XmlAttribute]
        public bool Hide { get; set; }

        [XmlAttribute]
        public bool IsDynamic { get; set; }

        [XmlIgnore]
        public bool IsFine
        {
            get { return Id == 0; }
        }
        [XmlIgnore]
        public bool IsCoarse
        {
            get { return Id == 1; }
        }

        /// <summary>
        /// Specifies overrides to the valid ranges for a pot with a specific parameter
        /// This is only necessary for the Fine/Coarse pots
        /// </summary>
        [XmlElement(ElementName="RangeOverride")]
        public List<RangeOverride> RangeOverrides { get; set; }
    }

    
}
