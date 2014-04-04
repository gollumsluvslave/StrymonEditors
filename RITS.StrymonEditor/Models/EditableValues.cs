using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RITS.StrymonEditor.Models
{

    [Serializable]
    public class Option:NameBase
    {
        [XmlAttribute]
        public double Value { get; set; }
    }

    [Serializable]
    public class Range
    {
        [XmlAttribute]
        public int MinValue { get; set; }
        [XmlAttribute]
        public int MaxValue { get; set; }

    }

    [Serializable]
    public class RangeOverride
    {
        [XmlAttribute]
        public string TriggerParameter { get; set; }
        [XmlAttribute]
        public int Value { get; set; }

        public Range Range { get; set; }

        public List<Increment> IncrementMap { get; set; }
    }

}
