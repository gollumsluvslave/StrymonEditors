using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// A more lightweight representation of a parameter for saving to xml
    /// Name, Value and FineValue only
    /// </summary>
    [Serializable]
    public class XmlParameter : NameBase
    {
        /// <summary>
        /// The value for the parameter
        /// </summary>
        [XmlAttribute]
        public int Value { get; set; }

        /// <summary>
        /// The 'fine' value for the parameter - where applicable
        /// </summary>
        [XmlAttribute]
        public int FineValue { get; set; }
    }
}
