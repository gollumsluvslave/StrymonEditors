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
    /// Represents a name/value pair metadata type tag
    /// </summary>
    public class Tag
    {
        public Tag()
        {
            AvailableValues = new List<string>();
        }
        public string TagName { get; set; }
        public string Value { get; set; }

        public List<string> AvailableValues
        {
            get;
            set;
        }
    }


}
