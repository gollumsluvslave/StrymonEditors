using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using RITS.StrymonEditor.Logging;

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
        public string Pedal { get; set; }

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

        /// <summary>
        /// Helper method that converts the xml representation into a usable <see cref="StrymonPreset"/>
        /// </summary>
        /// <param name="xmlPreset"></param>
        /// <returns></returns>
        public static StrymonPreset FromXmlPreset(StrymonXmlPreset xmlPreset)
        {
            using (RITSLogger logger = new RITSLogger())
            {
                var pedal = StrymonPedal.GetPedalByName(xmlPreset.Pedal);
                StrymonPreset preset = new StrymonPreset(pedal, false);
                // Set Machine
                preset.Name = xmlPreset.Name;
                preset.Machine = pedal.Machines.FirstOrDefault(x => x.Value == xmlPreset.Machine);
                // Single Byte Params
                foreach (var p in preset.AllParameters.Where(x => x.SysExOffset != 0))
                {
                    // get parameter from preset 1st
                    var xmlParameter = xmlPreset.Parameters.FirstOrDefault(x=>x.Name==p.Name);
                    if(xmlParameter != null)
                    {
                        p.Value = xmlParameter.Value;
                    }                     
                }
                preset.EPSetValues = new List<HeelToeSetting>();
                foreach (var ht in xmlPreset.EPSet)
                {
                    var xmlHt = new HeelToeSetting { PotId = ht.PotId, HeelValue = ht.HeelValue, ToeValue = ht.ToeValue };
                    preset.EPSetValues.Add(xmlHt);
                }
                return preset;
            }
        }
         
    }




}
