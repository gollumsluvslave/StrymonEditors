using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RITS.StrymonEditor.Models;

using StrymonPresetStoreClient;
using StrymonPresetStoreClient.Default;
using Microsoft.OData.Client;

namespace RITS.StrymonEditor.IO
{
    public class StubOnlineService : IOnlinePresetService
    {
        private StrymonStoreClient client = PresetStoreHooks.OnlineClient;
        public StubOnlineService()
        {
            client.RefreshCache();
        }
        public List<string> GetAvailableTagNames()
        {

            return client.GetTags();
        }
        public List<string> GetExistingValuesForTag(string tagName)
        {
            return client.GetValuesForTag(tagName);
        }

        public StrymonXmlPreset DownloadPreset(int presetId)
        {
            var preset = client.DownloadPreset(presetId);
            return FromStorePreset(preset);
        }

        // Dummy that returns a singel preset that macthes the supplied search
        public List<PresetMetadata> Search(PresetSearch search)
        {
            return client.Search(search);
        }

        public int UploadPreset(StrymonXmlPreset preset, List<Tag> metadata)
        {
            preset.Tags = metadata;
            preset.Tags.Add(new Tag { TagName = "Author", Value = Properties.Settings.Default.OnlineHandle });
            client.UploadPreset(preset);
            return 0;
        }

        private StrymonXmlPreset FromStorePreset(DBPreset preset)
        {
            var retval = new StrymonXmlPreset();
            retval.Machine = preset.MachineId;
            retval.Name = preset.Name;
            retval.Pedal = preset.PedalId;
            retval.Parameters = new List<XmlParameter>();
            var ps = preset.Parameters.ToList();
            foreach (var p in ps.Where(x => !x.Name.StartsWith("EPSet_")))
            {
                var xp = new XmlParameter();
                xp.Name = p.Name;
                if (p.Name.EndsWith("_Fine"))
                {
                    var pname = p.Name.Substring(0, p.Name.IndexOf('_'));
                    var xParam = retval.Parameters.FirstOrDefault(x => x.Name == pname);
                    if (xParam != null)
                    {
                        xParam.FineValue = p.Value;
                    }
                }
                else
                {
                    xp.Value = p.Value;
                    retval.Parameters.Add(xp);
                }

            }
            retval.EPSet = new List<HeelToeSetting>();
            // Heels first
            foreach (var p in ps.Where(x => x.Name.StartsWith("EPSet_Heel_")))
            {
                var hts = new HeelToeSetting();
                hts.PotId = Convert.ToInt32(p.Name.Split("_".ToCharArray())[2]);
                hts.HeelValue = p.Value;
                retval.EPSet.Add(hts);
            }
            // Toes
            foreach (var p in ps.Where(x => x.Name.StartsWith("EPSet_Toe_")))
            {
                var hts = retval.EPSet.First(x => x.PotId == Convert.ToInt32(p.Name.Split("_".ToCharArray())[2]));
                hts.ToeValue = p.Value;
            }
            return retval;
        }




    }
}
