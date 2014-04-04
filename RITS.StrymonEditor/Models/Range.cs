using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Represents a range of values
    /// </summary>
    [Serializable]
    public class Range
    {
        [XmlAttribute]
        public int MinValue { get; set; }
        [XmlAttribute]
        public int MaxValue { get; set; }

    }
}
