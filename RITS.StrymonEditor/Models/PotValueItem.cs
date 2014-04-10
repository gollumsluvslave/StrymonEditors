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
        /// <summary>
        /// The underlying 'coarse' value that is applicable for the angle / clock position
        /// </summary>
        [XmlAttribute]
        public int Value { get; set; }

        /// <summary>
        /// The underlying 'fine' value that is applicable for the angle / clock position
        /// Where not applicable (non fine/coarse) parameters this will be 0 and not present
        /// </summary>
        [XmlIgnore]
        public int FineValue { get; set; }

        /// <summary>
        /// The angle that is relevant for the clock position
        /// </summary>
        [XmlAttribute]
        public double Angle { get; set; }

        /// <summary>
        /// The 'clock position' for this tem
        /// NB this is simply an information / description field that makes teh xml more readable
        /// It is not actually used in code
        /// </summary>
        [XmlAttribute]
        public string ClockPosition { get; set; }


    }

}
