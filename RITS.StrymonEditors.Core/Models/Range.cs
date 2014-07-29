using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ServiceModel;

namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Represents a range of values
    /// </summary>
    [DataContract, XmlSerializerFormat]
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
