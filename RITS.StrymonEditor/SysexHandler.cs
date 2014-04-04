using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RITS.StrymonEditor
{
    public class SysExParser
    {
        public SysExMessage Parse(string filePath)
        {
            byte[] presetData = GetBinaryFile(filePath);
            return new SysExMessage(presetData);
        }
        private byte[] GetBinaryFile(string filePath)
        {
            long fileSize = new FileInfo(filePath).Length;
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    return reader.ReadBytes((int)fileSize);
                }
            }
        }

        
    }
    public class SysExMessage
    {
        private byte[] _data;
        public SysExMessage(byte[] data)
        {
            if (data.Length != Globals.PresetLength) throw new ArgumentOutOfRangeException("Invalid Preset - wrong length!");
            _data = data;
            ManufacturerId = _data[1];
            DeviceId = _data[2];
            ModelId = _data[3];
            AddressStart = _data.Skip(5).Take(4).ToArray();
            Data = _data.Skip(9).Take(Globals.PresetDataLength).ToArray();
            Checksum = _data[Globals.PresetChecksumOffset];
        }
        // Midi Start for Sys Ex
        public byte Start { get { return 240; } }
        public byte ManufacturerId { get; set; }
        public byte DeviceId { get; set; }
        public byte ModelId { get; set; }
        public byte SendRequestByte { get { return 18; } }
        public byte[] AddressStart { get; set; }
        public byte[] Data { get; set; }
        public byte Checksum { get; set; }
        public byte CalcChecksum { get { return CalculateChecksum(); } }
        public byte End { get { return 247; } }

        private byte CalculateChecksum() 
        {
            uint accum = 0;
            foreach (var b in Data)
            {
                accum += (uint) 0x7F & b;
            }
            return Convert.ToByte(0x7F & accum);
        }

        public void SaveAs(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(Start);
                    writer.Write(ManufacturerId);
                    writer.Write(DeviceId);
                    writer.Write(ModelId);
                    writer.Write(SendRequestByte);
                    writer.Write(AddressStart);
                    writer.Write(Data);
                    writer.Write(Checksum);
                    writer.Write(End);
                }
            }
        }
    }


}
