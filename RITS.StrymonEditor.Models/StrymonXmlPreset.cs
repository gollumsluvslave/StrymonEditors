using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Light-weight xml representation of a <see cref="StrymonPreset"/>
    /// </summary>
    [Serializable]
    public class StrymonXmlPreset : NameBase
    {
        public StrymonXmlPreset()
        {
        }

        /// <summary>
        /// The name of the pedal this preset is for
        /// </summary>
        [XmlAttribute]
        public int Pedal { get; set; }

        /// <summary>
        /// The value of the machine used in the preset
        /// </summary>
        [XmlAttribute]
        public int Machine { get; set; }

        /// <summary>
        /// The EPSet data
        /// </summary>
        public List<HeelToeSetting> EPSet { get; set; }

        /// <summary>
        /// All parameters for this preset
        /// </summary>
        [XmlArray]
        [XmlArrayItem(ElementName = "Parameter")]        
        public List<XmlParameter> Parameters { get; set; }

        public List<Tag> Tags { get; set; }

        
    }




}
