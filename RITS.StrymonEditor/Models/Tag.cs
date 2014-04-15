using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Represents a name/value pair metadata type tag
    /// </summary>
    public class Tag
    {
        public string TagName { get; set; }
        public string Value { get; set; }

        public List<string> AvailableValues
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Simplified data exchange type to keep data sizes down when searching
    /// </summary>
    public class PresetMetadata
    {
        [XmlAttribute]
        public int PresetId { get; set; }
        [XmlAttribute]
        public string PresetName { get; set; }
        [XmlAttribute]
        public string Author { get; set; }
        public string Url { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
