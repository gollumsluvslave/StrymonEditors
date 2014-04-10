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
        /// <summary>
        /// The minimum value accpetbale
        /// </summary>
        [XmlAttribute]
        public int MinValue { get; set; }

        /// <summary>
        /// The maximum value acceptable
        /// </summary>
        [XmlAttribute]
        public int MaxValue { get; set; }

    }
}
