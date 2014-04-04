using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RITS.StrymonEditor.Models
{

    [Serializable]
    public class Option:NameBase
    {
        [XmlAttribute]
        public double Value { get; set; }
    }



}
