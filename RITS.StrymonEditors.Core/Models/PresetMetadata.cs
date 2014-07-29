using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Simplified data exchange type to keep data sizes down when searching
    /// </summary>
    public class PresetMetadata
    {
        public int PresetId { get; set; }
        public string PresetName { get; set; }
        public int PedalId { get; set; }
        public int MachineId { get; set; }
        public string PedalName { get; set; }
        public string MachineName { get; set; }

        public string Author { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
