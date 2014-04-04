using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using RITS.StrymonEditor.Logging;
namespace RITS.StrymonEditor.Models
{
    public static class StrymonSysExUtils
    {
        private static StrymonSysExMessage Template()
        {
            using (MCLogger logger = new MCLogger())
            {
                using (Stream resFilestream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RITS.StrymonEditor.Base.syx"))
                {
                    if (resFilestream == null) return null;
                    byte[] ba = new byte[resFilestream.Length];
                    resFilestream.Read(ba, 0, ba.Length);
                    return new StrymonSysExMessage(ba);
                }
            }
        }      
        public static StrymonPreset FromSysExData(byte[] syxData)
        {
            using (MCLogger logger = new MCLogger())
            {
                StrymonSysExMessage msg = new StrymonSysExMessage(syxData);
                StrymonPreset preset = new StrymonPreset(msg.StrymonPedal, false);
                // Set Machine
                preset.Machine = preset.Pedal.Machines.FirstOrDefault(x => x.Value == msg.Data[0]);
                preset.Name = msg.PresetName;
                // Set all single Byte params / pots
                foreach (var p in preset.AllParameters.Where(x => x.SysExOffset != 0))
                {
                    p.Value = msg.Data[p.SysExOffset];
                    if (p.HasFineControl)
                    {
                        p.FineValue = GetFineValue(msg.Data, msg.StrymonPedal);
                    }
                }
                // TODO Fine/Coarse Params

                // Can't do bitwise ops on System.Single (float) - c++ using union typedef?
                // msg.Data is a subset byte array starting from the machine 'Type' 
                //(i.e. not including the first 9 bytes of the Sysex message)
                int time;
                time = (msg.Data[66] & 0x7F);
                time |= (msg.Data[65] & 0x7F) << 7;
                time |= (msg.Data[64] & 0x7F) << 14;
                time |= (msg.Data[63] & 0x7F) << 21;
                time |= (msg.Data[62] & 0xF) << 28;
                float test = (float)time; // Straight cast?? Doubtful!
                Console.WriteLine(test);
                return preset;
            }
        }
        private static int GetFineValue(byte[] msgData, StrymonPedal pedal)
        {
            int multiplier = msgData[34];
            int ms = msgData[35];
            int total = (multiplier * 128) + ms;
            // TODO differences by pedal
            return total;
        }
        private static void SetFineValue(int fineValue, byte[] msgData, StrymonPedal pedal)
        {
            int multiplier = fineValue / 128;
            int ms = fineValue % 128;
            msgData[34] = Convert.ToByte(multiplier);
            msgData[35] = Convert.ToByte(ms);
        }
        public static byte[] ToSysExData(StrymonPreset preset)
        {
            using (MCLogger logger = new MCLogger())
            {
                var template = Template();
                template.AddressStart[0] =Convert.ToByte(GetDeviceIdForPedal(preset.Pedal));
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
                // TODO - more complex params??

                return template.FullMessageData;
            }
        }

        public static int GetDeviceIdForPedal(StrymonPedal pedal)
        {
            switch (pedal.Name)
            {
                case "Timeline":
                    return 1;
                case "Mobius":
                    return 2;
                case "BigSky":
                    return 3;
                default:
                    return 0;
            }
        }

        public static StrymonPedal GetPedalForPedalName(string name)
        {
            return Globals.SupportedPedals.FirstOrDefault(x=>x.Name==name);
        }

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
                    for (int i = 0; i < nameBytes.Length; i++)
                    {
                        Data[Globals.PresetNameOffset+i] = nameBytes[i];
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
                            return Globals.SupportedPedals.FirstOrDefault(x => x.Name == "Timeline");
                        case 2:
                            return Globals.SupportedPedals.FirstOrDefault(x => x.Name == "Mobius");
                        case 3:
                            return Globals.SupportedPedals.FirstOrDefault(x => x.Name == "BigSky");
                        default:
                            return null;
                    }
                }
            }

            private byte CalculateChecksum()
            {
                using (MCLogger logger = new MCLogger())
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
