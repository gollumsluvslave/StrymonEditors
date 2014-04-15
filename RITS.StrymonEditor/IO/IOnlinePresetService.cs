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
        List<PresetMetadata> Search(List<Tag> searchCriteria); 
        int UploadPreset(StrymonXmlPreset preset, List<Tag> metadata);
    }

    public class StubOnlineService : IOnlinePresetService
    {
        public List<string> GetAvailableTagNames()
        {
            return new List<string>{"Artist","Song"};
        }
        public List<string> GetExistingValuesForTag(string tagName)
        {
            if (tagName == "Artist")
            {
                return new List<string> { "Van Halen", "U2" };
            }
            else
            {
                return new List<string> { "Where the Streets", "Bad", "Jump" };
            }
        }

        public StrymonXmlPreset DownloadPreset(int presetId)
        {
            var sem = new StrymonSysExUtils.StrymonSysExMessage(StrymonPedal.GetPedalByName(StrymonPedal.Timeline_Name));
            return StrymonSysExUtils.FromSysExData(sem.FullMessageData).ToXmlPreset();
        }

        // Dummy that returns a singel preset that macthes the supplied search
        public List<PresetMetadata> Search(List<Tag> searchCriteria)
        {
            var retval = new List<PresetMetadata>();
            var p1 = new PresetMetadata { PresetId = 1, PresetName = "BIG DELAY", Author = "Ritchie", Url = "TODO", Tags = new List<Tag>() };
            var p2 = new PresetMetadata { PresetId = 2, PresetName = "SMALL DELAY", Author = "John", Url = "TODO", Tags = new List<Tag>() };
            foreach (var t in searchCriteria)
            {
                p1.Tags.Add(t);
                p2.Tags.Add(t);
            }
            retval.Add(p1);
            retval.Add(p2);
            return retval;
        }

        public int UploadPreset(StrymonXmlPreset preset, List<Tag> metadata)
        {
            return 0;
        }
    }
}
