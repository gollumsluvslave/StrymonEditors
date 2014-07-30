using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OData.Client;
using RITS.StrymonEditor.Models;
namespace StrymonPresetStoreClient
{
    public class StrymonStoreClient
    {
        private Default.Container dc;
        private List<DBTag> tagCache;
        private List<DBPresetTag> presetTagCache;
        public StrymonStoreClient(string serviceRoot)
        {
            try
            {
                dc = new Default.Container(new Uri(serviceRoot));
                dc.MergeOption = MergeOption.NoTracking;
                RefreshCache();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
            }
            
        }

        public void RefreshCache()
        {
            tagCache = dc.Tags.ToList();
            presetTagCache = dc.PresetTags.Expand(x => x.Tag).Expand(x => x.Preset).ToList();
        }



        public List<string> GetTags()
        {
            return tagCache.Select(x => x.TagName).ToList();
        }

        public List<string> GetValuesForTag(string name)
        {
            var tag = tagCache.FirstOrDefault(x => x.TagName == name);
            if (tag == null)
            {
                return new List<string>();
            }
            var s=presetTagCache.Where(x => x.Tag.TagId == tag.TagId).Select(x => x.Value).Distinct().ToList();
            return s;
        }

        public void UploadPreset(StrymonXmlPreset preset)
        {

            dc.Execute(new Uri("UploadPreset", UriKind.Relative), "POST",  new BodyOperationParameter("uploadPreset", preset));
        }

        public DBPreset DownloadPreset(int id)
        {
            return dc.Presets.Expand(x=>x.Parameters).Expand("PresetTags($expand=Tag)")
                             .Where(x => x.PresetId == id).Single();
        }

        public List<PresetMetadata> Search(PresetSearch search)
        {
            var x = dc.Execute<PresetMetadata>(new Uri("SearchForPresets", UriKind.Relative), "POST", false, new BodyOperationParameter("criteria", search)).ToList();
            return x;
        }


        public Default.Container Context
        {
            get
            {
                return dc;
            }
        }


    }

    

    public class PresetTagComparer : IEqualityComparer<DBPresetTag>
    {
        public bool Equals(DBPresetTag first, DBPresetTag second)
        {
            return (first.Tag.TagName == second.Tag.TagName && first.Value==second.Value);
        }

        public int GetHashCode(DBPresetTag tag)
        {
            return tag.PresetTagId;
        }
    }
}
