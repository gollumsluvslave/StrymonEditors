using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Windows;
using RITS.StrymonEditor.Serialization;
using RITS.StrymonEditor.Logging;
using RITS.StrymonEditor.Models;
namespace RITS.StrymonEditor
{
    /// <summary>
    /// Utility class for IO operations
    /// </summary>
    public static class IOUtils
    {
        /// <summary>
        /// Load a <see cref="StrymonPreset"/> from an xml file
        /// </summary>
        /// <returns></returns>
        public static StrymonPreset LoadXmlPreset()
        {
            using (RITSLogger logger = new RITSLogger())
            {
                try
                {

                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.DefaultExt = ".xml";
                    dlg.Filter = "Xml Files (.xml)|*.xml";
                    Nullable<bool> result = dlg.ShowDialog();
                    if (result == true)
                    {
                        using (XmlSerializer<StrymonXmlPreset> xs = new XmlSerializer<StrymonXmlPreset>())
                        {
                            var xmlPreset = xs.DeserializeFile(dlg.FileName);
                            var ps =StrymonXmlPreset.FromXmlPreset(xmlPreset);
                            ps.Filename = dlg.FileName;
                            return ps;
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Load a <see cref="StrymonPreset"/> from an .syx file
        /// </summary>
        /// <returns></returns>
        public static StrymonPreset LoadSyxPreset()
        {
            using (RITSLogger logger = new RITSLogger())
            {
                try
                {
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.DefaultExt = ".syx";
                    dlg.Filter = "Sysex Files (.syx)|*.syx";
                    Nullable<bool> result = dlg.ShowDialog();
                    if (result == true)
                    {
                        byte[] presetData = GetBinaryFile(dlg.FileName);
                        return StrymonSysExUtils.FromSysExData(presetData);
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    return null;
                }
            }
        }

        /// <summary>
        /// Save the supplied <see cref="StrymonPreset"/> to an xml file format
        /// </summary>
        /// <param name="preset"></param>
        public static bool SavePresetToXml(StrymonPreset preset)
        {
            using (RITSLogger logger = new RITSLogger())
            {
                try
                {
                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.FileName = preset.Name;
                    dlg.DefaultExt = ".xml";
                    dlg.Filter = "Xml Files (.xml)|*.xml";
                    string filePath = null;
                    Nullable<bool> result = dlg.ShowDialog();
                    if (result == true)
                    {
                        filePath = dlg.FileName;
                        using (XmlSerializer<StrymonXmlPreset> xs = new XmlSerializer<StrymonXmlPreset>())
                        {
                            xs.SerializeToFile(preset.ToXmlPreset(), filePath);
                        }
                        return true;
                    }
                    return false;

                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Save the supplied <see cref="StrymonPreset"/> to the .syx file format
        /// </summary>
        /// <param name="preset"></param>
        public static bool SavePresetToSyx(StrymonPreset preset)
        {
            using (RITSLogger logger = new RITSLogger())
            {
                try
                {
                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.DefaultExt = ".syx";
                    dlg.Filter = "Sysex Files (.syx)|*.syx";
                    string filePath = null;
                    Nullable<bool> result = dlg.ShowDialog();
                    if (result == true)
                    {
                        filePath = dlg.FileName;
                        var syxData = StrymonSysExUtils.ToSysExData(preset);
                        using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            using (BinaryWriter writer = new BinaryWriter(stream))
                            {
                                writer.Write(syxData);
                            }
                        }
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Backup all the presets of the supplied <see cref="StrymonPedal"/> to the .syx file format
        /// </summary>
        /// <param name="preset"></param>
        public static void PedalBackupToSyx(StrymonPedal pedal)
        {
            using (RITSLogger logger = new RITSLogger())
            {
                try
                {
                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.FileName = pedal.Name;
                    dlg.DefaultExt = ".syx";
                    dlg.Filter = "Sysex Files (.syx)|*.syx";
                    string filePath = null;
                    Nullable<bool> result = dlg.ShowDialog();
                    if (result == true)
                    {
                        filePath = dlg.FileName;
                        byte[] fullPedal = pedal.GetBackupData;
                        if (fullPedal.Length != pedal.PresetCount * 650)
                        {
                            throw new ArgumentException("Pedal does not have the correct number of bytes for a backup.");
                        }
                        using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            using (BinaryWriter writer = new BinaryWriter(stream))
                            {
                                writer.Write(fullPedal);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Save the supplied <see cref="StrymonPreset"/> to the .syx file format
        /// </summary>
        /// <param name="preset"></param>
        public static IEnumerable<StrymonPreset> GetPresetBackupDataFromSyx(StrymonPedal pedal)
        {
            using (RITSLogger logger = new RITSLogger())
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = pedal.Name;
                dlg.DefaultExt = ".syx";
                dlg.Filter = "Sysex Files (.syx)|*.syx";
                string filePath = null;
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    filePath = dlg.FileName;
                    byte[] fullBackup = GetBinaryFile(filePath);
                    var size = fullBackup.Length;
                    if (size != pedal.PresetCount * 650)
                    {
                        MessageBox.Show("Selected file does not have the correct number of bytes for a pedal backup file.","Invalid Backup");
                        yield break;
                    }
                    foreach (var presetData in fullBackup.Chunkify(650))
                    {
                        yield return StrymonSysExUtils.FromSysExData(presetData.ToArray());
                    }
                }
            }
        }


        /// <summary>
        /// Save the currently active preset over the file it was originally loaded from
        /// </summary>
        public static bool SavePreset(StrymonPreset preset)
        {
            if (preset.Filename != null)
            {
                using (XmlSerializer<StrymonXmlPreset> xs = new XmlSerializer<StrymonXmlPreset>())
                {
                    xs.SerializeToFile(preset.ToXmlPreset(), preset.Filename);
                }
                return true;
            }
            else
            {
               return SavePresetToXml(preset);
            }
        }


        // Helper method that returns a byte array from a filepath
        // Default encoding here??
        private static byte[] GetBinaryFile(string filePath)
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
    

}
