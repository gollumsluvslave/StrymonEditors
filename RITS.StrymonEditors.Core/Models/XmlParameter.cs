using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ServiceModel;

namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// A more lightweight representation of a parameter for saving to xml
    /// Name, Value and FineValue only
    /// </summary>
    [DataContract, XmlSerializerFormat]
    public class XmlParameter : NameBase
    {
        /// <summary>
        /// The value for the parameter
        /// </summary>
        [DataMember, XmlAttribute]
        public int Value { get; set; }

        /// <summary>
        /// The 'fine' value for the parameter - where applicable
        /// </summary>
        [DataMember, XmlAttribute]
        public int FineValue { get; set; }
    }
}
