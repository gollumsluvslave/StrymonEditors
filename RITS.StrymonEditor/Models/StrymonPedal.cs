using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RITS.StrymonEditor.Logging;
namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Class that represents one of the pedals
    /// </summary>
    [Serializable]
    public class StrymonPedal: NameBase
    {
        /// <summary>
        /// WPF colour information
        /// </summary>
        [XmlAttribute]
        public string Colour { get; set; }

        /// <summary>
        /// List of configured machines
        /// </summary>
        public List<StrymonMachine> Machines { get; set; }

        /// <summary>
        /// List of Control parameters - linked to pots
        /// </summary>
        public List<ParameterDef> ControlParameters { get; set; }

        /// <summary>
        /// List of common parameters shared across ALL machines
        /// </summary>
        public List<ParameterDef> CommonParameters { get; set; }

        /// <summary>
        /// List of parameters that are shared between a few machines
        /// </summary>
        public List<ParameterDef> SharedParameters { get; set; }

        /// <summary>
        /// List of Pots
        /// </summary>
        public List<Pot> Pots { get; set; }

        /// <summary>
        /// Default increment map for the Pedal
        /// </summary>
        public List<Increment> IncrementMap { get; set; }

        /// <summary>
        /// Default PotValueMap for the pedal
        /// </summary>
        public PotValueMap PotValueMap { get; set; }

        public int MidiChannel 
        {
            get
            {
                switch (Name)
                {
                    case Timeline_Name:
                        return Properties.Settings.Default.TimelineMidiChannel;
                    case Mobius_Name:
                        return Properties.Settings.Default.MobiusMidiChannel;
                    case BigSky_Name:
                        return Properties.Settings.Default.BigSkyMidiChannel;
                    default:
                        throw new ArgumentOutOfRangeException(string.Format("Unsupported pedal : {0}", Name));
                }

            }
        }

        private List<ParameterDef> AllParameters
        {
            get { return Machines.SelectMany(x => x.MachineParameters).Union(SharedParameters).ToList(); }
        }

        [XmlIgnore]
        public int Id
        {
            get 
            {
                switch (Name)
                {
                    case Timeline_Name:
                        return Timeline_Id;
                    case Mobius_Name:
                        return Mobius_Id;
                    case BigSky_Name:
                        return BigSky_Id;
                    default:
                        throw new ArgumentOutOfRangeException(string.Format("Unsupported pedal : {0}", Name));
                }
            }
        }



        public int PresetCount
        {
            get
            {
                switch (Name)
                {
                    case BigSky_Name:
                        return 300;
                    default:
                        return 200;
                }
            }
        }


        public int FineIncrement(int fineValue)
        {
            if (Name == Mobius_Name)return 10;
            int inc = 1;
            if (Name == BigSky_Name)
            {
                // Variable??
                if (fineValue < 1000)
                {
                    inc = 1;
                }
                else if (fineValue < 10000)
                {
                    inc = 10;
                }
                else
                {
                    inc = 100;
                }
            }
            return inc;
        }

        /// <summary>
        /// Returns an instance of <see cref="StrymonPedal"/> by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static StrymonPedal GetPedalByName(string name)
        {
            return Globals.SupportedPedals.FirstOrDefault(x => x.Name == name);
        }


        /// <summary>
        /// Returns an instance of <see cref="StrymonPedal"/> by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static StrymonPedal GetPedalById(int id)
        {
            return Globals.SupportedPedals.FirstOrDefault(x => x.Id == id);
        }
        
        #region Constants
        public const string Timeline_Name = "Timeline";
        public const string Mobius_Name = "Mobius";
        public const string BigSky_Name = "BigSky";

        public const int Timeline_Id = 1;
        public const int Mobius_Id = 2;
        public const int BigSky_Id = 3;


        public const int TypeEncoderCC = 19;
        public const int ValueEncoderCC = 20;
        public const int AFootCC = 80;
        public const int BFootCC = 82;
        public const int TapCC = 81;
        public const int C_TapCC = 81;
        public const int ExpressionCC = 100;
        public const int BypassCC = 102;
        public const int HoldCC = 97;
        public const int RemoteTapCC = 93;

        public const int LooperRecord = 87;
        public const int LooperStop = 85;
        public const int LooperPlay = 86;
        public const int LooperUndo = 89;
        public const int LooperRedo = 90;
        public const int LooperReverse = 94;
        public const int LooperFullHalf = 95;
        public const int LooperPrePost = 96;
        #endregion

        private Dictionary<int, string> presetInfo = new Dictionary<int, string>();
        [XmlIgnore]
        public Dictionary<int, string> PresetInfo
        {
            get { return presetInfo; }
        }

        private Dictionary<int, byte[]> presetRawData = new Dictionary<int, byte[]>();
        [XmlIgnore]
        public Dictionary<int, byte[]> PresetRawData
        {
            get { return presetRawData; }
        }

        public void UpdatePresetInfo(StrymonPreset preset)
        {
            if (PresetInfo.ContainsKey(preset.SourceIndex))
            {
                PresetInfo[preset.SourceIndex] = preset.Name;
            }
            else
            {
                PresetInfo.Add(preset.SourceIndex, preset.Name);
            }
        }

        public void UpdatePresetRawData(int index, byte[] data)
        {
            if (PresetRawData.ContainsKey(index))
            {
                PresetRawData[index] = data;
            }
            else
            {
                PresetRawData.Add(index, data);
            }
        }

        public string GetPresetName(int index)
        {
            if (PresetInfo.ContainsKey(index))
            {
                return PresetInfo[index];
            }
            else
            {
                return null;
            }
        }
        public byte[] GetBackupData
        {
            get
            {
                return PresetRawData.OrderBy(x => x.Key).SelectMany(x => x.Value).ToArray();
            }
        }
    }

}
