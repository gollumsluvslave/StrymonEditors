using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

using System.Data.Entity.Core;
using System.Data.Entity.ModelConfiguration.Conventions;

using System.Web.Mvc;
using System.Web.Http.Controllers;
using RITS.StrymonEditor.Models;
namespace PresetStore.Data
{
    public class PresetStoreContext : DbContext
    {
        public DbSet<DBPreset> Presets { get; set; }
        public DbSet<DBPresetTag> PresetTags { get; set; }
        public DbSet<DBTag> Tags { get; set; }
        public DbSet<DBParameter> Parameters { get; set; }

        public PresetStoreContext()
            : base("name=PresetStoreContext")
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DBPreset>().HasKey<int>(x => x.PresetId).ToTable("Presets");
            modelBuilder.Entity<DBPreset>().Property(x => x.Name).IsRequired().HasMaxLength(16);
            
            modelBuilder.Entity<DBTag>().HasKey<int>(x => x.TagId).ToTable("Tags");
            modelBuilder.Entity<DBParameter>().HasKey<int>(x => x.ParameterId).ToTable("Parameters");
            modelBuilder.Entity<DBPresetTag>().HasKey<int>(x => x.PresetTagId).ToTable("PresetTags");
        }

        public List<PresetMetadata> Search(PresetSearch search)
        {
            List<PresetMetadata> retval = new List<PresetMetadata>();
            IQueryable<DBPreset> query = Presets.Include(x=>x.PresetTags.Select(s=>s.Tag));
            if (search.MachineId.HasValue) query = query.Where(x => x.MachineId == search.MachineId);
            if (search.PedalId.HasValue) query = query.Where(x => x.PedalId == search.PedalId);
            foreach(var t in search.Tags)
            {
                query=query.Where(x=>  x.PresetTags.Select(p=>p.Tag.TagName).Contains(t.TagName));
                query = query.Where(x => x.PresetTags.Select(p => p.Value).Contains(t.Value));    
            }
            foreach (var p in query)
            {
                var pt = new PresetMetadata { PresetId = p.PresetId, PedalId=p.PedalId,MachineId=p.MachineId,PresetName = p.Name, Tags=new List<Tag>() };
                foreach (var t in p.PresetTags)
                {
                    if (t.Tag.TagName == "Author")
                    {
                        pt.Author = t.Value;
                    }
                    else
                    {
                        pt.Tags.Add(new Tag { TagName = t.Tag.TagName, Value = t.Value, AvailableValues = new List<string>() });
                    }
                }
                retval.Add(pt);
            }
            return retval;
        }

        public void UploadPreset(StrymonXmlPreset uploadPreset)
        {
            var dbP = Presets.Create();
            dbP.MachineId = uploadPreset.Machine;
            dbP.PedalId = uploadPreset.Pedal;
            dbP.Name = uploadPreset.Name;
            dbP.Parameters=new List<DBParameter>();
            foreach (var p in uploadPreset.Parameters)
            {
                var dbParam = Parameters.Create();
                dbParam.Name = p.Name;
                dbParam.Value = p.FineValue == 0 ? p.Value : p.FineValue;
                dbP.Parameters.Add(dbParam);
            }
            int i = 0;
            foreach (var p in uploadPreset.EPSet)
            {
                var h = Parameters.Create();
                h.Name = string.Format("EPSet_Heel_{0}",i);
                h.Value = p.HeelValue;
                dbP.Parameters.Add(h);
                var t = Parameters.Create();
                t.Name = string.Format("EPSet_Toe_{0}", i);
                t.Value = p.ToeValue;
                dbP.Parameters.Add(h);
                i++;
            } 
            dbP.PresetTags = new List<DBPresetTag>();
            foreach (var t in uploadPreset.Tags)
            {
                var tag = Tags.FirstOrDefault(x => x.TagName == t.TagName);
                var dbPresetTag = PresetTags.Create();
                dbPresetTag.Tag = tag;
                dbPresetTag.Value = t.Value;
                dbP.PresetTags.Add(dbPresetTag);
            }
            Presets.Add(dbP);
            SaveChanges();
        }

    }
    public class PresetTagComparer : IEqualityComparer<DBPresetTag>
    {
        public bool Equals(DBPresetTag first, DBPresetTag second)
        {
            return (first.Tag.TagName == second.Tag.TagName && first.Value == second.Value);
        }

        public int GetHashCode(DBPresetTag tag)
        {
            return tag.PresetTagId;
        }
    }

}