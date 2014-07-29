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
    /// Type that represents an option to be selected from a list
    /// </summary>
    [DataContract, XmlSerializerFormat]
    public class Option : NameBase
    {
        /// <summary>
        /// The underlying value for this option
        /// </summary>
        [XmlAttribute]
        public double Value { get; set; }
    }



}
