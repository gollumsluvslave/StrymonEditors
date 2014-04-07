using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RITS.StrymonEditor.Serialization;
using RITS.StrymonEditor.Logging;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;
namespace RITS.StrymonEditor
{
    public enum SyncMode
    {
        TwoWay = 0,
        EditorMaster,
        PedalMaster
    }

    /// <summary>
    /// Globals - shared variables and code
    /// Avoid abusing, prefer to use decouple messaging using the Mediator and ViewModels
    /// </summary>
    public static class Globals
    {
        public const int PresetNameOffset = 623;
        public const int PresetNameLen = 16;
        public const int PresetLength = 650;
        public const int PresetDataOffset = 9;
        public const int PresetDataLength = 639;
        public const int PresetChecksumOffset = 648;
        public static bool SynchInProgress;
        public static List<StrymonPedal> SupportedPedals { get; set; }

        //private static StrymonPreset _activePreset; 
        //public static StrymonPreset ActivePreset 
        //{
        //    get { return _activePreset; }
        //    set 
        //    { 
        //        _activePreset = value;
        //    }
        //}

        public static FineCoarseSynchroniser FineCoarseSynchroniser
        {
            get;
            set;
        }


        public static PotValueMap PotValueMap 
        {
            get;
            set;
        }

        public static bool IsBPMModeActive 
        {
            get { return Properties.Settings.Default.BPMMode; }
            set { Properties.Settings.Default.BPMMode = value; }
        }

        /// <summary>
        /// Main initilisation method - loads all definition xmls, and sets the list of SupportedPedals
        /// </summary>
        public static void Init()
        {
            // TODO : Initialize MIDI here?? Create StrymonMidiManager as Singleton instance
            using (RITSLogger logger = new RITSLogger())
            {
                SupportedPedals = new List<StrymonPedal>();
                foreach (string pedalFolder in Directory.GetDirectories("Pedals"))
                {
                    StrymonPedal current = null;
                    string pedalName = Path.GetFileNameWithoutExtension(pedalFolder);
                    string pedalDefPath = Path.Combine(pedalFolder, pedalName + ".xml");
                    if (File.Exists(pedalDefPath))
                    {
                        logger.Debug(string.Format("Deserializing: {0}",pedalDefPath));
                        using (XmlSerializer<StrymonPedal> xs = new XmlSerializer<StrymonPedal>())
                        {
                            current = xs.DeserializeFile(pedalDefPath);
                        }
                        string machineFolder = Path.Combine(pedalFolder, "Machines");
                        foreach (var machinePath in Directory.GetFiles(machineFolder, "*.xml"))
                        {
                            logger.Debug(string.Format("Deserializing: {0}", machinePath));
                            using (XmlSerializer<StrymonMachine> xs = new XmlSerializer<StrymonMachine>())
                            {
                                var machine = xs.DeserializeFile(machinePath);
                                logger.Debug(string.Format("Adding Machine: {0}", machine.Name));
                                current.Machines.Add(machine);
                            }

                        }
                        SupportedPedals.Add(current);
                    }
                }
            }
        }

        #region IEnumerable<T> Extenions
        public static IEnumerable<IEnumerable<T>> Chunkify<T>(this IEnumerable<T> enumerable,
                                                      int chunkSize)
        {
            if (chunkSize < 1) throw new ArgumentException("chunkSize must be positive");

            using (var enumerator = enumerable.GetEnumerator())
                while (enumerator.MoveNext())
                    yield return enumerator.GetChunk(chunkSize);
        }

        private static IEnumerable<T> GetChunk<T>(this IEnumerator<T> enumerator,
                                                  int chunkSize)
        {
            do yield return enumerator.Current;
            while (--chunkSize > 0 && enumerator.MoveNext());
        }
        #endregion

    }
}
