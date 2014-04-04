using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Provision to 'override' a <see cref="Range"/> based on a trigger 
    /// and additionally a new set of increments
    /// </summary>
    [Serializable]
    public class RangeOverride
    {
        [XmlAttribute]
        public string TriggerParameter { get; set; }
        [XmlAttribute]
        public int Value { get; set; }

        public Range Range { get; set; }

        public Range CoarseRange { get; set; }

        public List<Increment> IncrementMap { get; set; }
    }
}
