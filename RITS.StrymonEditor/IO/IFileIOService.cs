using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RITS.StrymonEditor.Models;

namespace RITS.StrymonEditor.IO
{
    /// <summary>
    /// Interface that defines the varios IO operations needed for the editor
    /// </summary>
    public interface IFileIOService
    {
        StrymonPreset LoadXmlPreset();
        StrymonPreset LoadSyxPreset();
        bool SavePresetToXml(StrymonPreset preset);
        bool SavePresetToSyx(StrymonPreset preset);
        bool SavePreset(StrymonPreset preset);
        void PedalBackupToSyx(StrymonPedal pedal);
        IEnumerable<StrymonPreset> GetPresetBackupDataFromSyx(StrymonPedal pedal);
    }
}
