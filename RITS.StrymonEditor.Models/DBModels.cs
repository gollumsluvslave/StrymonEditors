using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.OData.Client;

namespace RITS.StrymonEditor.Models
{
    [Key("PresetId")]
    public class DBPreset
    {
        public virtual int PresetId { get; set; }
        public virtual string Name { get; set; }
        public virtual int PedalId { get; set; }
        public virtual int MachineId { get; set; }
        public virtual IList<DBPresetTag> PresetTags { get; set; }
        public virtual IList<DBParameter> Parameters { get; set; }
    }

    [Key("PresetTagId")]
    public class DBPresetTag
    {
        public virtual int PresetTagId { get; set; }
        public virtual DBPreset Preset { get; set; }
        public virtual DBTag Tag { get; set; }
        public virtual string Value { get; set; }
    }

    [Key("TagId")]
    public class DBTag
    {
        public virtual int TagId { get; set; }
        public virtual string TagName { get; set; }
    }

    [Key("ParameterId")]
    public class DBParameter
    {
        public virtual int ParameterId { get; set; }
        public virtual int PresetId { get; set; }
        public virtual string Name { get; set; }
        public virtual int Value { get; set; }
    }
}