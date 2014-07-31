using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RITS.StrymonEditor.Logging;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;
using RITS.StrymonEditor.Serialization;
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
        // TODO - Properties / application settings
        public static bool IsBPMModeActive 
        {
            get { return NativeHooks.Current.BPMMode; }
            set { NativeHooks.Current.BPMMode = value; }
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

        //private static StrymonPresetStoreClient.StrymonStoreClient _client;
        //public static StrymonPresetStoreClient.StrymonStoreClient OnlineClient
        //{
        //    get
        //    {
        //        if (_client == null) _client = new StrymonPresetStoreClient.StrymonStoreClient(Properties.Settings.Default.OnlineService);
        //        return _client;
        //    }
        //}

        public static bool IsPedalViewLoading {get; set;}

        /// <summary>
        /// Main initilisation method - loads all definition xmls, and sets the list of SupportedPedals
        /// </summary>
        public static void Init()
        {
            LoadSupportedPedals();
            
        }

        private static void LoadSupportedPedals()
        {
            var pedals = new List<StrymonPedal>();
            var resourceFiles = Assembly.GetExecutingAssembly().GetManifestResourceNames().ToList();
            var pedalFiles = resourceFiles.Where(x => !x.Contains("Machines") && !x.Contains("Base_"));
            var machineFiles = resourceFiles.Where(x => x.Contains("Machines")).ToList();
            foreach (var p in pedalFiles)
            {
                StrymonPedal pedal = null;
                using (XmlStreamSerializer<StrymonPedal> xs = new XmlStreamSerializer<StrymonPedal>())
                {
                    pedal = xs.DeserializeStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(p));
                }
                foreach(var m in machineFiles.Where(x=>x.Contains(string.Format(".{0}.Machines",pedal.Name))))
                {
                    StrymonMachine machine = null;
                    using (XmlStreamSerializer<StrymonMachine> xs = new XmlStreamSerializer<StrymonMachine>())
                    {
                        machine = xs.DeserializeStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(m));
                    }
                    pedal.Machines.Add(machine);
                }
                pedals.Add(pedal);
            }
            SupportedPedals = pedals;
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
