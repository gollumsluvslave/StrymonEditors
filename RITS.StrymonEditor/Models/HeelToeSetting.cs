using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
namespace RITS.StrymonEditor.Models
{
    [Serializable]
    public class HeelToeSetting
    {
        [XmlAttribute]
        public int PotId { get; set; }
        [XmlAttribute]
        public int HeelValue { get; set; }
        [XmlAttribute]
        public int ToeValue { get; set; }

    }
}
