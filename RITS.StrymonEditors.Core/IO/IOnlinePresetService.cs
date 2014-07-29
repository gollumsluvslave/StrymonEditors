using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RITS.StrymonEditor.Models;
namespace RITS.StrymonEditor.IO
{
    public interface IOnlinePresetService
    {
        List<string> GetAvailableTagNames();
        List<string> GetExistingValuesForTag(string tagName);
        StrymonXmlPreset DownloadPreset(int presetId);
        List<PresetMetadata> Search(PresetSearch search);
        int UploadPreset(StrymonXmlPreset preset, List<Tag> metadata);
    }
}
