using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using RITS.StrymonEditor.Serialization;
using RITS.StrymonEditor.Logging;
using RITS.StrymonEditor.Models;

namespace RITS.StrymonEditor.IO
{
    /// <summary>
    /// Implementation of <see cref="IFileIOService" to allow for greater unit test coverage/>
    /// </summary>
    public class FileIOService:IFileIOService
    {
        private IFileDialog openDialog;
        private IFileDialog saveDialog;
        private IMessageDialog messageDialog;
        public FileIOService(IFileDialog openDialog, IFileDialog saveDialog, IMessageDialog messageDialog)
        {
            this.openDialog = openDialog;
            this.saveDialog = saveDialog;
            this.messageDialog = messageDialog;
        }
        /// <summary>
        /// Load a <see cref="StrymonPreset"/> from an xml file
        /// </summary>
        /// <returns></returns>
        public StrymonPreset LoadXmlPreset()
        {
            using (RITSLogger logger = new RITSLogger())
            {
                try
                {
                    openDialog.DefaultExt = ".xml";
                    openDialog.Filter = "Xml Files (.xml)|*.xml";
                    Nullable<bool> result = openDialog.ShowDialog();
                    if (result == true)
                    {
                        using (XmlSerializer<StrymonXmlPreset> xs = new XmlSerializer<StrymonXmlPreset>())
                        {
                            var xmlPreset = xs.DeserializeFile(openDialog.FileName);
                            var ps = StrymonXmlPreset.FromXmlPreset(xmlPreset);
                            ps.Filename = openDialog.FileName;
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
        /// Load a <see cref="StrymonPreset"/> from a .syx SysEx file
        /// </summary>
        /// <returns></returns>
        public StrymonPreset LoadSyxPreset()
        {
            using (RITSLogger logger = new RITSLogger())
            {
                try
                {
                    openDialog.DefaultExt = ".syx";
                    openDialog.Filter = "Sysex Files (.syx)|*.syx";
                    Nullable<bool> result = openDialog.ShowDialog();
                    if (result == true)
                    {
                        byte[] presetData = GetBinaryFile(openDialog.FileName);
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
        /// Save the supplied <see cref="StrymonPreset"/> to an xml representation 
        /// </summary>
        /// <param name="preset"></param>
        /// <returns></returns>
        public bool SavePresetToXml(StrymonPreset preset)
        {
            using (RITSLogger logger = new RITSLogger())
            {
                try
                {
                    if(saveDialog.FileName==null)saveDialog.FileName=  preset.Name;
                    saveDialog.DefaultExt = ".xml";
                    saveDialog.Filter = "Xml Files (.xml)|*.xml";
                    string filePath = null;
                    Nullable<bool> result = saveDialog.ShowDialog();
                    if (result == true)
                    {
                        filePath = saveDialog.FileName;
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
        /// Save the supplied <see cref="StrymonPreset"/> to a .syx SysEx representation
        /// </summary>
        /// <param name="preset"></param>
        /// <returns></returns>
        public bool SavePresetToSyx(StrymonPreset preset)
        {
            using (RITSLogger logger = new RITSLogger())
            {
                try
                {
                    if (saveDialog.FileName == null) saveDialog.FileName = preset.Name;
                    saveDialog.DefaultExt = ".syx";
                    saveDialog.Filter = "Sysex Files (.syx)|*.syx";
                    string filePath = null;
                    Nullable<bool> result = saveDialog.ShowDialog();
                    if (result == true)
                    {
                        filePath = saveDialog.FileName;
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
        /// Save the supplied <see cref="StrymonPreset"/> 
        /// </summary>
        /// <param name="preset"></param>
        /// <returns></returns>
        public bool SavePreset(StrymonPreset preset)
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

        /// <summary>
        /// Backup all presets of the supplied <see cref="StrymonPedal"/> to a single .syx file
        /// </summary>
        /// <param name="preset"></param>
        /// <returns></returns>
        public void PedalBackupToSyx(StrymonPedal pedal)
        {
            using (RITSLogger logger = new RITSLogger())
            {
                try
                {
                    
                    saveDialog.FileName = pedal.Name;
                    saveDialog.DefaultExt = ".syx";
                    saveDialog.Filter = "Sysex Files (.syx)|*.syx";
                    string filePath = null;
                    Nullable<bool> result = saveDialog.ShowDialog();
                    if (result == true)
                    {
                        filePath = saveDialog.FileName;
                        byte[] fullPedal = pedal.GetBackupData;
                        if (fullPedal.Length != pedal.PresetCount * 650)
                        {
                            messageDialog.ShowError("Pedal does not have the correct number of bytes for a backup.", "Invalid Backup");
                            return;
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
        /// Enumerate all the presets for the supplied <see cref="StrymonPedal"/> from a .syx backup
        /// </summary>
        /// <param name="pedal"></param>
        /// <returns></returns>
        public IEnumerable<StrymonPreset> GetPresetBackupDataFromSyx(StrymonPedal pedal)
        {
            using (RITSLogger logger = new RITSLogger())
            {
                
                openDialog.DefaultExt = ".syx";
                openDialog.Filter = "Sysex Files (.syx)|*.syx";
                string filePath = null;
                Nullable<bool> result = openDialog.ShowDialog();
                if (result == true)
                {
                    filePath = openDialog.FileName;
                    byte[] fullBackup = GetBinaryFile(filePath);
                    var size = fullBackup.Length;
                    if (size != pedal.PresetCount * 650)
                    {
                        messageDialog.ShowError("Selected file does not have the correct number of bytes for a pedal backup file.", "Invalid Backup");
                        yield break;
                    }
                    foreach (var presetData in fullBackup.Chunkify(650))
                    {
                        yield return StrymonSysExUtils.FromSysExData(presetData.ToArray());
                    }
                }
            }
        }

        // Helper method that returns a byte array from a filepath
        // Default encoding here??
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
}
