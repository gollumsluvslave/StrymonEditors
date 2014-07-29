using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RITS.StrymonEditor.Conversion;
using RITS.StrymonEditor.Logging;

namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Defines an in memory preset to drive the editor
    /// </summary>
    public class StrymonPreset : NameBase
    {
        private StrymonMachine currentMachine;
        private StrymonMachine prevMachine;

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
        

        /// <summary>
        /// Flag that indicates whether this preset is new and has not been sourced from a pre-existing file
        /// </summary>
        public bool IsNew { get { return Name == null; } }

        /// <summary>
        /// List of <see cref="HeelToeSetting"/> values used to store the EPSet information
        /// </summary>
        public List<HeelToeSetting> EPSetValues { get; set; }


        /// <summary>
        /// The index of the parameter currently assigned to the dynamic 'Param1' <see cref="Pot"/>
        /// Only relevant for Mobius and BigSky presets
        /// </summary>
        public int Param1ParameterIndex { get; set; }

        /// <summary>
        /// The index of the parameter currently assigned to the dynamic 'Param2' <see cref="Pot"/>
        /// Only relevant for Mobius and BigSky presets
        /// </summary>
        public int Param2ParameterIndex { get; set; }
        
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

        /// <summary>
        /// Return the SysEx 'index' for the supplied pot id
        /// NB this is based on an offset of 17
        /// </summary>
        /// <param name="potId"></param>
        /// <returns></returns>
        public int GetDynamicAssignedParameterIndex(int potId)
        {
            var p1Pot = HiddenParameters.FirstOrDefault(x => x.DynamicPotIdAssigned == potId);
            return p1Pot.SysExOffset-17; // Touchy Feely
        }

        /// <summary>
        /// Returns the current 'fine' value for this preset - it is unique at any given time
        /// Currently only used to populate the DirecTEntry dialog
        /// </summary>
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
        /// Convert the Preset to a light-weight <see cref="StrymonXmlPreset"/> format
        /// </summary>
        /// <returns></returns>
        public StrymonXmlPreset ToXmlPreset()
        {
            var retval = new StrymonXmlPreset { Name = this.Name, Pedal=this.Pedal.Id, Machine=this.Machine.Value, Parameters=new List<XmlParameter>() };
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
        /// Helper method that converts the xml representation into a usable <see cref="StrymonPreset"/>
        /// </summary>
        /// <param name="xmlPreset"></param>
        /// <returns></returns>
        public static StrymonPreset FromXmlPreset(StrymonXmlPreset xmlPreset)
        {
            var pedal = StrymonPedal.GetPedalById(xmlPreset.Pedal);
            StrymonPreset preset = new StrymonPreset(pedal, false);
            // Set Machine
            preset.Name = xmlPreset.Name;
            preset.Machine = pedal.Machines.FirstOrDefault(x => x.Value == xmlPreset.Machine);
            // Single Byte Params
            foreach (var p in preset.AllParameters.Where(x => x.SysExOffset != 0))
            {
                // get parameter from preset 1st
                var xmlParameter = xmlPreset.Parameters.FirstOrDefault(x => x.Name == p.Name);
                if (xmlParameter != null)
                {
                    p.Value = xmlParameter.Value;
                    if (p.HasFineControl)
                    {
                        p.FineValue = xmlParameter.FineValue;
                    }
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

        /// <summary>
        /// The filename currently associated with the preset - will be null if created from scratch
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// The index position for this preset in the pedal
        /// </summary>
        public int SourceIndex { get; set; }


        // Helper that caches the previous values of the hidden parameters
        // to allow them to be reset on a change of machine
        private void CachePreviousParameters()
        {
            if (HiddenParameters == null) return;
            previousParameterValues.Clear();
            foreach (var p in HiddenParameters)
            {
                previousParameterValues.Add(p.Name, p.Value);
            }
        }

        // Helper that returns the previous value for a named parameter
        private int GetValueFromPreviousParameter(string paramName)
        {
            if (previousParameterValues.ContainsKey(paramName))
            {
                return previousParameterValues[paramName];
            }
            return 0;
        }

        // Helper that rebuilds the parameters for this preset
        private void RebuildParameters()
        {
            CachePreviousParameters();
            using (ILogger logger= NativeHooks.Current.CreateLogger())
            {

                if (prevMachine == null || prevMachine.Value != currentMachine.Value)
                {
                    PotValueMap.Reset();
                    // Tell Fine/Coarse ViewModels to refresh the IncrementMap too

                    // Only create ControlParameters 1st time
                    if (ControlParameters == null)
                    {
                        ControlParameters = new List<Parameter>();
                        foreach (var p in Pedal.ControlParameters)
                        {
                            
                            Parameter pv = new Parameter { Definition = p, ContextPedalName = Pedal.Name };
                            ControlParameters.Add(pv);
                        }
                    }
                    // Hidden & Common Parameters start at offset 17
                    int offset = 17;

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
                                logger.Warn(string.Format("Orphaned Reference to Shared Parameter : {0}", p.Name));
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

                    // Set the DynamicPotId - VM will overwrite this
                    var param1 = HiddenParameters[Param1ParameterIndex];
                    param1.DynamicPotIdAssigned = 5;
                    var param2 = HiddenParameters[Param2ParameterIndex];
                    param2.DynamicPotIdAssigned = 6;

                }
            }
        }
    }




}
