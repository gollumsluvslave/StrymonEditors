using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Defines an increment to be used for finevalue control up to a specified end value
    /// </summary>
    [Serializable]
    public class Increment
    {
        /// <summary>
        /// The terminating value for this increment
        /// </summary>
        [XmlAttribute]
        public int End { get; set; }

        /// <summary>
        /// The value or defintion of the increment
        /// </summary>
        [XmlAttribute]
        public string Value { get; set; }

        /// <summary>
        /// Handles returning increment value 
        /// - this needs to support some string parsing for alternating increments
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetIncrementValue(ref int index)
        {
            if (IncrementIndexMap.Count <= index) index = 0;
                return IncrementIndexMap[index];
        }


        // Helper method that uses regex to build a map for the specified value
        // This allows shortcut definitions to be specified
        List<int> map;
        private List<int> IncrementIndexMap
        {
            get
            {
                if (map == null)
                {
                    map = new List<int>();
                    string[] x = Value.Split(",".ToCharArray());                    
                    foreach (var s in x)
                    {
                        var regEx = @"(\d+)\((\d+)\)";
                        if (Regex.IsMatch(s, regEx))
                        {
                            var m = Regex.Match(Value, regEx);
                            var mult = Convert.ToInt32(m.Groups[1].Value);
                            var number = Convert.ToInt32(m.Groups[2].Value);
                            for (int i = 1; i <= mult; i++)
                            {
                                map.Add(number);
                            }
                        }
                        else
                        {
                            map.Add(Convert.ToInt32(s));
                        }
                    }
                }
                return map;
            }
        }
    }
}
