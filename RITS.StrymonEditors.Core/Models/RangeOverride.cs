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
    /// Provision to 'override' a <see cref="Range"/> based on a trigger 
    /// and additionally a new set of increments
    /// </summary>
    [DataContract, XmlSerializerFormat]
    public class RangeOverride
    {
        /// <summary>
        /// Specifies the name of the parameter that will trigger changes to the range
        /// </summary>
        [XmlAttribute]
        public string TriggerParameter { get; set; }
        
        /// <summary>
        /// Specifies the value for the TriggerParameter that this override is relevant for
        /// </summary>
        [XmlAttribute]
        public int Value { get; set; }

        /// <summary>
        /// Absolute 'fine' range that is applicable for this override
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// Subset range that is applicable for the 'coarse' pot in this override
        /// </summary>
        public Range CoarseRange { get; set; }

        /// <summary>
        /// A list of <see cref="Increment"/>s that are used to correctly relate the fine values for the
        /// pot to the coarse values
        /// </summary>
        public List<Increment> IncrementMap { get; set; }
    }
}
