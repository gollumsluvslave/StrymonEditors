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
        /// Loads a template into a SysExMessage instance 
        /// </summary>
        /// <returns></returns>
        private static StrymonSysExMessage Template(StrymonPedal pedal)
        {
            using (RITSLogger logger = new RITSLogger())
            {
                var resourceName = string.Format("RITS.StrymonEditor.Base_{0}.syx",pedal.Name);
                using (Stream resFilestream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    if (resFilestream == null) return null;
                    byte[] ba = new byte[resFilestream.Length];
                    resFilestream.Read(ba, 0, ba.Length);
                    return new StrymonSysExMessage(ba);
                }
            }
        }

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
                preset.Machine = preset.Pedal.Machines.FirstOrDefault(x => x.Value == msg.Data[0]);
                preset.Name = msg.PresetName;
                preset.SourceIndex = GetPresetIndex(msg.AddressStart);
                // Set all single Byte params / pots
                foreach (var p in preset.AllParameters.Where(x => x.SysExOffset != 0))
                {
                    p.Value = msg.Data[p.SysExOffset];
                    if (p.HasFineControl)
                    {
                        p.FineValue = GetFineValue(msg.Data, msg.StrymonPedal);
                    }
                }
                //// TODO Fine/Coarse Params

                // HeelToe
                preset.EPSetValues = new List<HeelToeSetting>();
                foreach (var pot in msg.StrymonPedal.Pots.Where(x=>!x.Hide && x.Id >0))
                {
                    var ht = new HeelToeSetting
                        {
                            PotId = pot.Id,
                            HeelValue = msg.Data[Globals.GetHeelSysExOffSetForPot(pot.Id)],
                            ToeValue = msg.Data[Globals.GetToeSysExOffSetForPot(pot.Id)]
                        };
                    preset.EPSetValues.Add(ht);
                }
                if (msg.StrymonPedal.Name != StrymonPedal.Timeline_Name)
                {
                    preset.Param1ParameterIndex = msg.Data[62];
                    preset.Param2ParameterIndex = msg.Data[63];
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
                var template = Template(preset.Pedal);
                template.AddressStart[0] = Convert.ToByte(preset.Pedal.Id);
                template.Data[0] = Convert.ToByte(preset.Machine.Value);
                template.PresetName = preset.Name;
                // Single Byte Params
                foreach (var p in preset.AllParameters.Where(x => x.SysExOffset != 0))
                {
                    template.Data[p.SysExOffset] = Convert.ToByte(p.Value);
                    if (p.HasFineControl)
                    {
                        SetFineValue(p.FineValue, template.Data, template.StrymonPedal);
                    }
                }
                // HeelToe                
                foreach(var ht in preset.EPSetValues)
                {
                    template.Data[Globals.GetHeelSysExOffSetForPot(ht.PotId)] = Convert.ToByte(ht.HeelValue);
                    template.Data[Globals.GetToeSysExOffSetForPot(ht.PotId)] = Convert.ToByte(ht.ToeValue);
                }
                if (preset.Pedal.Name != StrymonPedal.Timeline_Name)
                {
                    template.Data[62] = Convert.ToByte(preset.GetDynamicAssignedParameterIndex(5));
                    template.Data[63] = Convert.ToByte(preset.GetDynamicAssignedParameterIndex(6));
                }

                return template.FullMessageData;
            }
        }


        
        
        
        // Helper that extracts byte offset values and converts into the fine value for a fine/coarse parameter
        private static int GetFineValue(byte[] msgData, StrymonPedal pedal)
        {
            // TODO differences by pedal
            if (pedal.Name == StrymonPedal.Timeline_Name || pedal.Name == StrymonPedal.Mobius_Name)
            { 
                int multiplier = msgData[34];
                int ms = msgData[35];
                int total = (multiplier * 128) + ms;
                if (pedal.Name == StrymonPedal.Timeline_Name) return total;
                if (total ==0) return 0;
                double tmp = Convert.ToDouble(1000) / total;
                return Convert.ToInt32(tmp * 1000);
            }
            if (pedal.Name == StrymonPedal.BigSky_Name)
            {
                int bigmultiplier = msgData[65];
                int smallmultiplier = msgData[66];
                int ms = msgData[67];
                int total = (bigmultiplier *16384) + (smallmultiplier * 128) + ms;
                return total;
            }
            return 0;
        }

        private static int GetPresetIndex(byte[] data)
        {
            int multiplier = data[2];
            int fine = data[3];
            return (multiplier * 128) + fine;
        }
        
        // Helper that sets the byte offset values to the supplied fine value
        private static void SetFineValue(int fineValue, byte[] msgData, StrymonPedal pedal)
        {
            // TODO differences by pedal
            if (pedal.Name == StrymonPedal.BigSky_Name)
            {
                int bigmultiplier = fineValue / 16384;
                int remainder = fineValue % 16384;
                int smallmultiplier = remainder / 128;
                int ms = remainder % 128;
                msgData[65] = Convert.ToByte(bigmultiplier);
                msgData[66] = Convert.ToByte(smallmultiplier);
                msgData[67] = Convert.ToByte(ms);
                return;
            }
            int valueToUse = fineValue;
            if (pedal.Name == StrymonPedal.Mobius_Name)
            {
                valueToUse = 1000000 / fineValue;
            }
            int multiplier = valueToUse / 128;
            int sms = valueToUse % 128;
            msgData[34] = Convert.ToByte(multiplier);
            msgData[35] = Convert.ToByte(sms);
            return;
        }

        // Helper

        /// <summary>
        /// Internal class that wraps a sysex byte array into a more helpful type
        /// </summary>
        private class StrymonSysExMessage
        {
            private byte[] _data;

            public StrymonSysExMessage(byte[] data)
            {
                if (data.Length != Globals.PresetLength) throw new ArgumentOutOfRangeException("Invalid Preset - wrong length!");
                _data = data;
                AddressStart = _data.Skip(5).Take(4).ToArray();
                Data = _data.Skip(9).Take(Globals.PresetDataLength).ToArray();
                Checksum = _data[Globals.PresetChecksumOffset];
            }
            // Static data
            public static byte Start { get { return 240; } }
            public static byte SendRequestByte { get { return 18; } }
            public static byte End { get { return 247; } }
            public static byte ManufacturerId { get { return 0; } }
            public static byte DeviceId { get { return 1; } }
            public static byte ModelId { get { return 85; } }

            // Variable data
            public byte[] AddressStart { get; set; }
            public byte[] Data { get; set; }
            public string PresetName
            {
                get
                {
                    var nameData = (Data.Skip(Globals.PresetNameOffset).Take(Globals.PresetNameLen)).ToArray();
                    return UTF8Encoding.UTF8.GetString(nameData);
                }
                set
                {
                    var nameBytes = UTF8Encoding.UTF8.GetBytes(value);
                    for (int i = 0; i < 16; i++)
                    {
                        byte b =(i < nameBytes.Length) ? nameBytes[i] : Convert.ToByte(' ');
                        Data[Globals.PresetNameOffset + i] = b;
                    }
                }
            }
            public byte Checksum { get; set; }
            public byte[] FullMessageData
            {
                get
                {
                    return SyxGenerator.ToArray();
                }
            }
            
            private IEnumerable<byte> SyxGenerator
            {
                get
                {
                    yield return Start;
                    yield return ManufacturerId;
                    yield return DeviceId;
                    yield return ModelId;
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
            }

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
        }

    }


}
