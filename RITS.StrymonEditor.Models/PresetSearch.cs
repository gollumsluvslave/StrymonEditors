using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.Models
{
    public class PresetSearch
    {
        public int? PedalId { get; set; }
        public int? MachineId { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
