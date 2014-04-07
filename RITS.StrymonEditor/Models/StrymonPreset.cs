using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RITS.StrymonEditor.Serialization;
using RITS.StrymonEditor.Conversion;
using RITS.StrymonEditor.Logging;

namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Defines an in memory preset to drive the editor
    /// </summary>
    public class StrymonPreset : NameBase
    {
        #region Constructors
        // Default 
        private Dictionary<string, int> previousParameterValues = new Dictionary<string, int>();
        public StrymonPreset()
        {
        }
        public StrymonPreset(StrymonPedal pedal, bool isNew)
        {
            Param1ParameterIndex = 0;
            Param2ParameterIndex = 1;
            Pedal = pedal;
            if (isNew)
            {
                Machine = Pedal.Machines.First();
            }
        }

        #endregion

        /// <summary>
        /// The pedal this preset is related to
        /// </summary>
        public StrymonPedal Pedal { get; set; }
        public bool IsNew { get { return Name == null; } }
        private StrymonMachine currentMachine;
        private StrymonMachine prevMachine;

        public List<HeelToeSetting> EPSetValues { get; set; }

        public int Param1ParameterIndex 
        { 
            get; 
            set; 
        }
        public int Param2ParameterIndex 
        { 
            get; 
            set; 
        }
        
        /// <summary>
        /// The machine the preset uses
        /// </summary>
        public StrymonMachine Machine 
        {
            get { return currentMachine; }
            set
            {
                prevMachine = currentMachine;
                currentMachine = value;
                RebuildParameters();                
            }
        }

        /// <summary>
        /// The Control Parameters for the preset
        /// </summary>
        public List<Parameter> ControlParameters { get; set; }

        /// <summary>
        /// The hidden parameters for the preset
        /// </summary>
        public List<Parameter> HiddenParameters { get; set; }

        /// <summary>
        /// Helper to expose all parameters - union
        /// </summary>
        public List<Parameter> AllParameters 
        {
            get { return ControlParameters.Union(HiddenParameters).ToList(); }
        }

        /// <summary>
        /// The PotValueMap used for the preset
        /// </summary>
        public PotValueMap PotValueMap
        {
            get
            {
                if (Machine.PotValueMap != null && Machine.PotValueMap.Count > 0)
                {
                    return Machine.PotValueMap;
                }
                return Pedal.PotValueMap;
            }
        }

        private void CachePreviousParameters()
        {
            if (HiddenParameters == null) return;
            previousParameterValues.Clear();
            foreach (var p in HiddenParameters)
            {
                previousParameterValues.Add(p.Name, p.Value);
            }
        }

        private int GetValueFromPreviousParameter(string paramName)
        {
            if (previousParameterValues.ContainsKey(paramName))
            {
                return previousParameterValues[paramName];
            }
            return 0;
        }


        private void RebuildParameters()
        {
            CachePreviousParameters();
            using (RITSLogger logger = new RITSLogger())
            {

                if (prevMachine == null || prevMachine.Value != currentMachine.Value)
                {
                    PotValueMap.Reset();
                    // Tell Fine/Coarse ViewModels to refresh the IncrementMap too
                    
                    int offset = 1;
                    // Only create ControlParameters 1st time
                    if (ControlParameters == null)
                    {
                        ControlParameters = new List<Parameter>();
                        foreach (var p in Pedal.ControlParameters)
                        {
                            p.SysExOffset = offset;
                            Parameter pv = new Parameter { Definition = p, ContextPedalName=Pedal.Name };
                            ControlParameters.Add(pv);
                            if (offset == 5) { offset++; } // Odd offset not used
                            offset++;
                        }
                    }
                    // Hidden & Common Parameters start at offset 17
                    offset=17;

                    HiddenParameters = new List<Parameter>();
                    // Order by index? Serialized order would be best, less code, but more brittle
                    foreach (var p in Machine.MachineParameters)
                    {
                        var def = p;
                        if (p.IsRef)
                        {
                            def = Pedal.SharedParameters.FirstOrDefault(x => x.Name == p.Name);
                            if (def == null)
                            {
                                logger.Warn(string.Format("Orphaned Reference to Shared Parameter : {0}",p.Name));
                            }
                        }
                        def.SysExOffset = offset;
                        Parameter pv = new Parameter { Definition = def, ContextPedalName = Pedal.Name, Value = GetValueFromPreviousParameter(p.Name) };
                        HiddenParameters.Add(pv);
                        offset++;
                    }
                    // Order by index? Serialized order would be best, less code, but more brittle
                    foreach (var p in Pedal.CommonParameters)
                    {
                        p.SysExOffset = offset;
                        Parameter pv = new Parameter { Definition = p, ContextPedalName = Pedal.Name, Value = GetValueFromPreviousParameter(p.Name) };
                        HiddenParameters.Add(pv);
                        offset += p.PostOffset;
                        offset++;
                    }

                }
            }
        }

         
        public StrymonPreset Clone()
        {
            using (XmlSerializer<StrymonPreset> xs = new XmlSerializer<StrymonPreset>())
            {
                return xs.DeserializeString(xs.SerializeToString(this));
            }
        }

        public int GetDynamicAssignedParameterIndex(int potId)
        {
            var p1Pot = HiddenParameters.FirstOrDefault(x => x.DynamicPotIdAssigned == potId);
            return p1Pot.SysExOffset-17; // Touchy Feely
        }

        public string FineValue
        {
            get
            {
                int fineValue = ControlParameters[0].FineValue;
                if (!Globals.IsBPMModeActive)
                    return fineValue.ToString();
                if (Pedal.Name == StrymonPedal.Mobius_Name)
                    return ConversionUtils.ConvertMilliHzToBPM(fineValue).ToString();
                return ConversionUtils.ConvertMillisecondsToBPM(fineValue).ToString();
            }
        }

        /// <summary>
        /// Convert the Preset to a light-weight xml format
        /// </summary>
        /// <returns></returns>
        public StrymonXmlPreset ToXmlPreset()
        {
            var retval = new StrymonXmlPreset { Name = this.Name, Pedal=this.Pedal.Name, Machine=this.Machine.Value, Parameters=new List<XmlParameter>() };
            foreach (var p in AllParameters)
            {
                retval.Parameters.Add(p.ToXmlParameter());
            }
            retval.EPSet = new List<HeelToeSetting>();
            foreach (var ht in EPSetValues)
            {
                var xmlHt = new HeelToeSetting { PotId = ht.PotId, HeelValue = ht.HeelValue, ToeValue = ht.ToeValue };
                retval.EPSet.Add(xmlHt);
            }
            return retval;
        }

        /// <summary>
        /// The filename currently associated with the preset - will be null if created from scratch
        /// </summary>
        public string Filename { get; set; }

        public int SourceIndex { get; set; }
    }




}
