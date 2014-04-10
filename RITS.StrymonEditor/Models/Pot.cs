using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Defines a 'Pot' or dial on the Strymon pedals
    /// </summary>
    [Serializable]
    public class Pot
    {
        /// <summary>
        /// The numeric 'Id' for the pot
        /// </summary>
        [XmlAttribute]
        public int Id { get; set; }

        /// <summary>
        /// The default textual label for the pot
        /// </summary>
        [XmlAttribute]
        public string DefaultLabel { get; set; }

        /// <summary>
        /// The label for the pot
        /// </summary>
        [XmlAttribute]
        public string Label { get; set; }

        /// <summary>
        /// Value that allows fine adjustment of where this pot is positioned on the editor canvas with respect to the horizontal axis
        /// This was required as the Background images for the 3 pedals are all slightly different
        /// </summary>
        [XmlAttribute]
        public int Left { get; set; }

        /// <summary>
        /// Value that allows fine adjustment of where this pot is positioned on the editor canvas with respect to the vertical axis
        /// This was required as the Background images for the 3 pedals are all slightly different
        /// </summary>
        [XmlAttribute]
        public int Top { get; set; }

        /// <summary>
        /// Flag that indicates this pot shoul not be visible in the UI
        /// Required to support Mobius where pots 5 and 8 are not present
        /// </summary>
        [XmlAttribute]
        public bool Hide { get; set; }

        /// <summary>
        /// Flag that indicates the pot is a 'dynamic' pot that can be assigned to different parameters
        /// Only supported in Mobius and BigSky and currently fixed to pots 6 and 7
        /// </summary>
        [XmlAttribute]
        public bool IsDynamic { get; set; }

        /// <summary>
        /// Returns whether the pot is the fine encoder (Id=0)
        /// </summary>
        [XmlIgnore]
        public bool IsFine
        {
            get { return Id == 0; }
        }

        /// <summary>
        /// Returns whether the pot is the coarse pot (Id=1)
        /// </summary>
        [XmlIgnore]
        public bool IsCoarse
        {
            get { return Id == 1; }
        }

        /// <summary>
        /// Specifies overrides to the valid ranges for a pot with a specific parameter
        /// This is only necessary/relevant for the Fine/Coarse pots
        /// </summary>
        [XmlElement(ElementName="RangeOverride")]
        public List<RangeOverride> RangeOverrides { get; set; }
    }

    
}
