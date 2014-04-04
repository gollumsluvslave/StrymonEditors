using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Type that abstracts the value, angle and fine value relationships of a Pot/Dial using Clock Positions as reference
    /// </summary>
    [Serializable]
    public class PotValueItem
    {
        [XmlAttribute]
        public int Value { get; set; }
        
        [XmlAttribute]
        public double Angle { get; set; }

        [XmlAttribute]
        public string ClockPosition { get; set; }

        [XmlIgnore]
        public int FineValue { get; set; }

    }

}
