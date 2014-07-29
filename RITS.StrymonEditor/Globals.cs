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
    /// <summary>
    /// Synhronization modes for how the editor and pedal are synced
    /// </summary>
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
        /// <summary>
        /// Sysex Offset for the Presetname
        /// </summary>
        public const int PresetNameOffset = 623;

        /// <summary>
        /// Length of the preset  name
        /// </summary>
        public const int PresetNameLen = 16;
        public const int PresetLength = 650;

        /// <summary>
        /// Sysex offset for the actual Data
        /// </summary>
        public const int PresetDataOffset = 9;

        /// <summary>
        /// The length of the data portion of the sysex preset
        /// </summary>
        public const int PresetDataLength = 639;

        /// <summary>
        /// SysEx offset for the checksum
        /// </summary>
        public const int PresetChecksumOffset = 648;

        /// <summary>
        /// List of supported pedals based on xml definition load
        /// </summary>
        public static List<StrymonPedal> SupportedPedals { get; set; }

        /// <summary>
        /// <see cref="FineCoarseSynchroniser"/> to be used for fine / coasre synchonisation opeartions
        /// 
        /// TODO remove from globals
        /// 
        /// </summary>
        public static FineCoarseSynchroniser FineCoarseSynchroniser
        {
            get;
            set;
        }

        /// <summary>
        /// <see cref="PotValueMap"/> set of data used to drive pot curves
        /// 
        /// TODO remove from Globals
        /// 
        /// </summary>
        public static PotValueMap PotValueMap 
        {
            get;
            set;
        }

        /// <summary>
        /// Global flag on that indicates BPMmode is active
        /// </summary>
        public static bool IsBPMModeActive 
        {
            get { return Properties.Settings.Default.BPMMode; }
            set { Properties.Settings.Default.BPMMode = value; }
        }

        /// <summary>
        /// Global flag on that indicates BPMmode is active
        /// </summary>
        private static bool machineLocked;
        public static bool MachineLocked
        {
            get { return machineLocked; }
            set { machineLocked = value; }
        }

        /// <summary>
        /// Global flag on that indicates BPMmode is active
        /// </summary>
        private static int lockedMachine;
        public static int LockedMachine
        {
            get { return lockedMachine; }
            set { lockedMachine = value; }
        }

        private static StrymonPresetStoreClient.StrymonStoreClient _client;
        public static StrymonPresetStoreClient.StrymonStoreClient OnlineClient
        {
            get
            {
                if (_client == null) _client = new StrymonPresetStoreClient.StrymonStoreClient(Properties.Settings.Default.OnlineService);
                return _client;
            }
        }

        public static bool IsPedalViewLoading {get; set;}

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

        /// <summary>
        /// Chunks the supplied generic IEnumerable into chunks
        /// NB the chunksize should be an exact match
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Chunkify<T>(this IEnumerable<T> enumerable,
                                                      int chunkSize)
        {
            if (chunkSize < 1) throw new ArgumentException("chunkSize must be positive");

            using (var enumerator = enumerable.GetEnumerator())
                while (enumerator.MoveNext())
                    yield return enumerator.GetChunk(chunkSize);
        }

        /// helper that retrieves a chunk
        private static IEnumerable<T> GetChunk<T>(this IEnumerator<T> enumerator,
                                                  int chunkSize)
        {
            do yield return enumerator.Current;
            while (--chunkSize > 0 && enumerator.MoveNext());
        }
        #endregion

    }
}
