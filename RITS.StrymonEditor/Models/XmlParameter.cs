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
        [XmlAttribute]
        public int Value { get; set; }
        [XmlAttribute]
        public int FineValue { get; set; }
    }
}
