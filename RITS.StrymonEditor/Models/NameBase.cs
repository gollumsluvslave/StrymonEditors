using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// base class for model types that have a name property
    /// </summary>
    [Serializable]
    public class NameBase
    {
        [XmlAttribute]
        public string Name { get; set; }
    }
}
