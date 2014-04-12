using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using RITS.StrymonEditor.Logging;

namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Helper / Utility class to handle / encapsulate all things relating to SysExm
    /// </summary>
    public static class StrymonSysExUtils
    {
        
        /// <summary>
        /// Loads a <see cref="StrymonPreset"/> instance from a SysEx byte array
        /// </summary>
        /// <param name="syxData"></param>
        /// <returns></returns>
        public static StrymonPreset FromSysExData(byte[] syxData)
        {
            using (RITSLogger logger = new RITSLogger())
            {
                StrymonSysExMessage msg = new StrymonSysExMessage(syxData);
                StrymonPreset preset = new StrymonPreset(msg.StrymonPedal, false);
                // Set Machine
                preset.Machine = msg.StrymonMachine;
                preset.Name = msg.PresetName;
                preset.SourceIndex = msg.PresetIndex;
                // Set all single Byte params / pots
                foreach (var p in preset.AllParameters.Where(x => x.SysExOffset != 0))
                {
                    p.Value = msg.Data[p.SysExOffset];
                    // Special handling for fine/coasre parameter
                    if (p.HasFineControl)
                    {
                        p.FineValue = msg.FineValue;
                    }
                }
                // EP Set HeelToe
                preset.EPSetValues = new List<HeelToeSetting>();
                foreach (var pot in msg.StrymonPedal.Pots.Where(x=>!x.Hide && x.Id >0))
                {
                    var ht = new HeelToeSetting
                        {
                            PotId = pot.Id,
                            HeelValue = msg.GetHeel(pot.Id),
                            ToeValue = msg.GetToe(pot.Id)
                        };
                    preset.EPSetValues.Add(ht);
                }
                // Dynamic parameter pot references
                if (msg.StrymonPedal.Name != StrymonPedal.Timeline_Name)
                {
                    preset.Param1ParameterIndex = msg.DynamicParameterPot1Index;
                    preset.Param2ParameterIndex = msg.DynamicParameterPot2Index;
                }
                return preset;
            }
        }


        /// <summary>
        /// Converts a supplied <see cref="StrymonPreset"/> instance back to a SysEx byte array
        /// </summary>
        /// <param name="preset"></param>
        /// <returns></returns>
        public static byte[] ToSysExData(StrymonPreset preset)
        {
            using (RITSLogger logger = new RITSLogger())
            {
                var sysExMessage = new StrymonSysExMessage(preset.Pedal);
                sysExMessage.StrymonPedal = preset.Pedal;
                sysExMessage.StrymonMachine = preset.Machine;
                sysExMessage.PresetName = preset.Name;
                // Single Byte Params
                foreach (var p in preset.AllParameters.Where(x => x.SysExOffset != 0))
                {
                    sysExMessage.Data[p.SysExOffset] = Convert.ToByte(p.Value);
                    if (p.HasFineControl)
                    {
                        sysExMessage.FineValue = p.FineValue;
                    }
                }
                // HeelToe                
                foreach(var ht in preset.EPSetValues)
                {
                    sysExMessage.SetHeel(ht.PotId, ht.HeelValue);
                    sysExMessage.SetToe(ht.PotId, ht.ToeValue);
                }
                if (preset.Pedal.Name != StrymonPedal.Timeline_Name)
                {
                    sysExMessage.DynamicParameterPot1Index = preset.GetDynamicAssignedParameterIndex(5);
                    sysExMessage.DynamicParameterPot2Index = preset.GetDynamicAssignedParameterIndex(6);
                }

                return sysExMessage.FullMessageData;
            }
        }

        /// <summary>
        /// Internal class that wraps a sysex byte array into a more helpful type
        /// </summary>
        public class StrymonSysExMessage
        {
            private byte[] _data;

            /// <summary>
            ///  .ctor for byte array
            /// </summary>
            /// <param name="data"></param>
            public StrymonSysExMessage(byte[] data)
            {
                Init(data);
            }

            /// <summary>
            /// .ctor for a pedal, using embedded template
            /// </summary>
            /// <param name="pedal"></param> 
            public StrymonSysExMessage(StrymonPedal pedal)
            {
                var resourceName = string.Format("RITS.StrymonEditor.Base_{0}.syx",pedal.Name);
                using (Stream resFilestream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    if (resFilestream == null) return;
                    byte[] ba = new byte[resFilestream.Length];
                    resFilestream.Read(ba, 0, ba.Length);
                    Init(ba);
                }            
            }

            private void Init(byte[] data)
            {
                if (data.Length != Globals.PresetLength) throw new ArgumentOutOfRangeException("Invalid Preset - wrong length!");
                _data = data;
                AddressStart = _data.Skip(5).Take(4).ToArray();
                Data = _data.Skip(9).Take(Globals.PresetDataLength).ToArray();
                Checksum = _data[Globals.PresetChecksumOffset];
            }

            // Static data
            public static byte Start { get { return 0xF0; } }
            public static byte SendRequestByte { get { return 0x12; } }
            public static byte End { get { return 0xF7; } }
            public static byte[] StrymonId { get { return new byte[] { 0x00, 0x01, 0x55 }; } }

            /// <summary>
            /// Address start including pedal id and preset info
            /// </summary>
            public byte[] AddressStart { get; set; }

            /// <summary>
            /// The main preset data
            /// </summary>
            public byte[] Data { get; set; }
            
            /// <summary>
            /// The sysex checksum
            /// </summary>
            public byte Checksum { get; set; }

            /// <summary>
            /// Generator method that returns the instances as a bytearray
            /// </summary>
            public byte[] FullMessageData
            {
                get
                {
                    return SyxGenerator.ToArray();
                }
            }

            /// <summary>
            /// Exposes the <see cref="StrymonPedal"/> Id as useful strongly typed property, encapsulating the sys ex offset details away from the caller
            /// </summary>
            public StrymonPedal StrymonPedal
            {
                get
                {
                    int deviceId = AddressStart[0];
                    switch (deviceId)
                    {
                        case 1:
                            return Globals.SupportedPedals.FirstOrDefault(x => x.Name == StrymonPedal.Timeline_Name);
                        case 2:
                            return Globals.SupportedPedals.FirstOrDefault(x => x.Name == StrymonPedal.Mobius_Name);
                        case 3:
                            return Globals.SupportedPedals.FirstOrDefault(x => x.Name == StrymonPedal.BigSky_Name);
                        default:
                            return null;
                    }
                }
                set
                {
                    var pedal = value;
                    AddressStart[0] = Convert.ToByte(pedal.Id);
                }
            }

            /// <summary>
            /// Exposes the <see cref="StrymonMachine"/> value as useful strongly typed property, encapsulating the sys ex offset details away from the caller
            /// </summary>
            public StrymonMachine StrymonMachine
            {
                get
                {
                    return StrymonPedal.Machines.FirstOrDefault(x => x.Value == Data[0]);
                }
                set
                {
                    var machine = value;
                    Data[0] = Convert.ToByte(machine.Value);
                }
            }

            /// <summary>
            /// Gets / sets the preset name
            /// </summary>
            public string PresetName
            {
                get
                {
                    var nameData = (Data.Skip(Globals.PresetNameOffset).Take(Globals.PresetNameLen)).ToArray();
                    return UTF8Encoding.UTF8.GetString(nameData).Trim();
                }
                set
                {
                    var nameBytes = UTF8Encoding.UTF8.GetBytes(value);
                    for (int i = 0; i < 16; i++)
                    {
                        byte b = (i < nameBytes.Length) ? nameBytes[i] : Convert.ToByte(' ');
                        Data[Globals.PresetNameOffset + i] = b;
                    }
                }
            }

            /// <summary>
            /// Gets / sets the preset index (pedal slot)
            /// </summary>
            public int PresetIndex
            {
                get
                {
                    int multiplier = AddressStart[2];
                    int fine = AddressStart[3];
                    return (multiplier * 128) + fine;
                }
            }

            #region FineValue
            /// <summary>
            /// Gets sets the FineValue, handling pedal differences
            /// </summary>
            public int FineValue
            {
                get 
                {
                    if (StrymonPedal.Name == StrymonPedal.BigSky_Name) return BigSkyFineValue;
                    if (StrymonPedal.Name == StrymonPedal.Mobius_Name) return MobiusFineValue;
                    return TimelineFineValue;
                }
                set
                {
                    if (StrymonPedal.Name == StrymonPedal.BigSky_Name) BigSkyFineValue = value;
                    if (StrymonPedal.Name == StrymonPedal.Mobius_Name) MobiusFineValue = value;
                    if (StrymonPedal.Name == StrymonPedal.Timeline_Name) TimelineFineValue = value;
                }
            }

            private int BigSkyFineValue
            {
                get 
                {
                    int bigmultiplier = Data[65];
                    int smallmultiplier = Data[66];
                    int ms = Data[67];
                    int total = (bigmultiplier * 16384) + (smallmultiplier * 128) + ms;
                    return total;
                }
                set 
                {
                    int fineValue = value;
                    int bigmultiplier = fineValue / 16384;
                    int remainder = fineValue % 16384;
                    int smallmultiplier = remainder / 128;
                    int ms = remainder % 128;
                    Data[65] = Convert.ToByte(bigmultiplier);
                    Data[66] = Convert.ToByte(smallmultiplier);
                    Data[67] = Convert.ToByte(ms);
                }
            }

            private int MobiusFineValue
            {
                get 
                { 
                    var total = TwoByteFineValue;
                    if (total == 0) return 0;
                    double tmp = Convert.ToDouble(1000) / total;
                    return Convert.ToInt32(tmp * 1000);
                }
                set
                {
                    int fineValue = value;
                    int valueToUse = fineValue;
                    if (StrymonPedal.Name == StrymonPedal.Mobius_Name)
                    {
                        valueToUse = 1000000 / fineValue;
                    }
                    TwoByteFineValue = valueToUse;
                }
            }

            private int TimelineFineValue
            {
                get 
                {
                    return TwoByteFineValue;
                }
                set
                {
                    TwoByteFineValue=value;
                }

            }

            private int TwoByteFineValue
            {
                get
                {
                    int multiplier = Data[34];
                    int ms = Data[35];
                    return (multiplier * 128) + ms;
                }
                set
                {
                    int multiplier = value / 128;
                    int sms = value % 128;
                    Data[34] = Convert.ToByte(multiplier);
                    Data[35] = Convert.ToByte(sms);
                }
            }
            #endregion FineValue

            #region EPSetHeelToe
            /// <summary>
            /// Sets the Heel value for a Pot
            /// </summary>
            /// <param name="potId"> the id of the pot</param>
            /// <param name="value">the value of the pot in the heel position</param>
            public void SetHeel(int potId, int value)
            {
                var sysExOffset = GetHeelSysExOffSetForPot(potId);
                Data[sysExOffset] = Convert.ToByte(value);
            }

            /// <summary>
            /// Sets the Toe value for a Pot
            /// </summary>
            /// <param name="potId"> the id of the pot</param>
            /// <param name="value">the value of the pot in the heel position</param>
            public void SetToe(int potId, int value)
            {
                var sysExOffset = GetHeelSysExOffSetForPot(potId) + 1;
                Data[sysExOffset] = Convert.ToByte(value);
            }

            public int GetHeel(int potId)
            {
                return Data[GetHeelSysExOffSetForPot(potId)];
            }

            public int GetToe(int potId)
            {
                return Data[GetHeelSysExOffSetForPot(potId) + 1];
            }
            #endregion

            #region DynamicParameters
            /// <summary>
            /// Gets / Sets the parameter association DynamicPot 1
            /// </summary>
            public int DynamicParameterPot1Index
            {
                get
                {
                    return Data[62];
                }
                set
                {
                    Data[62] = Convert.ToByte(value);
                }
            }

            /// <summary>
            /// Gets / Sets the parameter association DynamicPot 2
            /// </summary>
            public int DynamicParameterPot2Index
            {
                get
                {
                    return Data[63];
                }
                set
                {
                    Data[63] = Convert.ToByte(value);
                }
            }
            #endregion

            // .Syx generator
            private IEnumerable<byte> SyxGenerator
            {
                get
                {
                    yield return Start;
                    foreach (var b in StrymonId)
                    {
                        yield return b;
                    }
                    yield return SendRequestByte;
                    foreach (var b in AddressStart)
                    {
                        yield return b;
                    }
                    foreach (var b in Data)
                    {
                        yield return b;
                    }
                    yield return CalculateChecksum();
                    yield return End;
                }
            }

            // Checksum calculator - thanks John @ Strymon
            // Port from GitHub code
            // https://github.com/strymon/QRtMidi/blob/master/QRtMidiData.cpp 
            private byte CalculateChecksum()
            {
                using (RITSLogger logger = new RITSLogger())
                {
                    uint accum = 0;
                    foreach (var b in Data)
                    {
                        accum += (uint)0x7F & b;
                    }
                    return Convert.ToByte(0x7F & accum);
                }
            }

            // Helper method that returns the Heel SysExOffset for a specifc pot. 
            // involves skipping a couple of bytes for pots greater than 5
            private int GetHeelSysExOffSetForPot(int potId)
            {
                int offSet = (potId - 1) * 2;
                if (potId > 5) offSet = potId * 2;
                return 37 + offSet;
            }

        }

    }


}
