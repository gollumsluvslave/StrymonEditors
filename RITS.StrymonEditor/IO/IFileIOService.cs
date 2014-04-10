using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RITS.StrymonEditor.Models;

namespace RITS.StrymonEditor.IO
{
    /// <summary>
    /// Interface that defines the various IO operations needed for the editor
    /// </summary>
    public interface IFileIOService
    {
        /// <summary>
        /// Load a <see cref="StrymonPreset"/> from a previously saved xml file
        /// </summary>
        /// <returns></returns>
        StrymonPreset LoadXmlPreset();

        /// <summary>
        /// Load a <see cref="StrymonPreset"/> from a .syx SysEx file
        /// </summary>
        /// <returns></returns>
        StrymonPreset LoadSyxPreset();

        /// <summary>
        /// Save the supplied <see cref="StrymonPreset"/> to an xml representation 
        /// </summary>
        /// <param name="preset"></param>
        /// <returns></returns>
        bool SavePresetToXml(StrymonPreset preset);

        /// <summary>
        /// Save the supplied <see cref="StrymonPreset"/> to a .syx SysEx representation
        /// </summary>
        /// <param name="preset"></param>
        /// <returns></returns>
        bool SavePresetToSyx(StrymonPreset preset);

        /// <summary>
        /// Save the supplied <see cref="StrymonPreset"/> 
        /// </summary>
        /// <param name="preset"></param>
        /// <returns></returns>
        bool SavePreset(StrymonPreset preset);

        /// <summary>
        /// Backup all presets of the supplied <see cref="StrymonPedal"/> to a single .syx file
        /// </summary>
        /// <param name="preset"></param>
        /// <returns></returns>
        void PedalBackupToSyx(StrymonPedal pedal);

        /// <summary>
        /// Enumerate all the presets for the supplied <see cref="StrymonPedal"/> from a .syx backup
        /// </summary>
        /// <param name="pedal"></param>
        /// <returns></returns>
        IEnumerable<StrymonPreset> GetPresetBackupDataFromSyx(StrymonPedal pedal);
    }
}
