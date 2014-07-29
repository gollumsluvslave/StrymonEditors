using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ServiceModel;
namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Base class for model types that have a name property
    /// </summary>
    [DataContract, XmlSerializerFormat]
    public class NameBase
    {
        /// <summary>
        /// The name of this instance
        /// </summary>
        [DataMember, XmlAttribute]
        public string Name { get; set; }
    }
}
