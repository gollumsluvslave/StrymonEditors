using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RITS.StrymonEditor.Models
{



    [Serializable]
    public class Address
    {
        [XmlAttribute]
        public int Offset { get; set; }
        [XmlAttribute]
        public string ParameterName { get; set; }
    }
}
