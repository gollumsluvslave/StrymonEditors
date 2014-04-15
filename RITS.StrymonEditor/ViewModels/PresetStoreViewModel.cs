using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RITS.StrymonEditor.Commands;
using RITS.StrymonEditor.IO;
using RITS.StrymonEditor.Models;
namespace RITS.StrymonEditor.ViewModels
{
    /// <summary>
    /// ViewModel responseible for the <see cref="Views.PresetStoreWindow"/>
    /// </summary>
    public class PresetStoreViewModel:ViewModelBase
    {
        private StrymonPreset uploadPreset;
        public PresetStoreViewModel()
        {
            IsUploadMode = false;
        }

        public IOnlinePresetService OnlineService
        {
            get;
            set;
        }

        public PresetStoreViewModel(StrymonPreset presetToUpload)
        {
            uploadPreset = presetToUpload;
            IsUploadMode = true;
        }
        /// <summary>
        /// The mode of operation - either Upload / Download
        /// </summary>
        private bool isUploadMode;
        public bool IsUploadMode
        {
            get { return isUploadMode; }
            set 
            { 
                isUploadMode = value; 
                OnPropertyChanged("IsUploadMode");
                OnPropertyChanged("IsDownloadMode");
            }
        }

        public bool IsDownloadMode
        {
            get
            {
                return !IsUploadMode;
            }
        }

        private BindableCollection<string> pedals;
        /// <summary>
        /// List of Custom Tags avialbale for search / association
        /// </summary>
        public BindableCollection<string> Pedals
        {
            get
            {
                if (pedals == null) // If caching, use set
                {
                    pedals = new BindableCollection<string>();
                    pedals.Add(StrymonPedal.Timeline_Name);
                    pedals.Add(StrymonPedal.Mobius_Name);
                    pedals.Add(StrymonPedal.BigSky_Name);
                }
                return pedals;
            }
            set
            {
                pedals = value;
                OnPropertyChanged("Pedals");
            }
        }

        private BindableCollection<string> machines;
        /// <summary>
        /// List of Custom Tags avialbale for search / association
        /// </summary>
        public BindableCollection<string> Machines
        {
            get
            {
                if (machines == null) // If caching, use set
                {
                    machines = new BindableCollection<string>();
                    machines.Add("");
                    var pedal = Globals.SupportedPedals.FirstOrDefault(x => x.Name == SelectedPedal);
                    if (pedal != null)
                    {
                        foreach (var m in pedal.Machines)
                        {
                            machines.Add(m.Name);
                        }
                    }
                }
                return machines;
            }
            set
            {
                machines = value;
                OnPropertyChanged("Machines");
            }
        }


        private BindableCollection<string> availableTags;
        /// <summary>
        /// List of Custom Tags avialbale for search / association
        /// </summary>
        public BindableCollection<string> AvailableTags
        {
            get
            {
                if (availableTags == null) // If caching, use set
                {
                    availableTags = new BindableCollection<string>();
                    foreach (var t in OnlineService.GetAvailableTagNames())
                    {
                        availableTags.Add(t);
                    }
                }
                return availableTags;
            }
            set
            {
                availableTags = value;
                OnPropertyChanged("AvailableTags");
            }
        }

        private BindableCollection<Tag> customTags = new BindableCollection<Tag>();
        /// <summary>
        /// List of Custom Tags
        /// </summary>
        public BindableCollection<Tag> CustomTags
        {
            get
            {
                return customTags;
            }
            set
            {
                customTags = value;
                OnPropertyChanged("CustomTags");
            }
        }

        private BindableCollection<PresetMetadata> presets;
        /// <summary>
        /// List of Custom Tags
        /// </summary>
        public BindableCollection<PresetMetadata> Presets
        {
            get
            {
                if (presets == null) presets = new BindableCollection<PresetMetadata>();
                return presets;
            }
            set
            {
                presets = value;
                OnPropertyChanged("Presets");
            }
        }

        private PresetMetadata selectedPreset;
        /// <summary>
        /// The preset that has been selected in the search dialog
        /// </summary>
        public PresetMetadata SelectedPreset
        {
            get { return selectedPreset; }
            set 
            { 
                selectedPreset = value; 
                OnPropertyChanged("SelectedPreset"); 
            }
        }

        private string selectedPedal;
        /// <summary>
        /// The pedal that has been selected in the search dialog
        /// </summary>
        public string SelectedPedal
        {
            get { return selectedPedal; }
            set 
            { 
                selectedPedal = value; 
                OnPropertyChanged("SelectedPedal");
                Machines = null;
                OnPropertyChanged("Machines"); 
            }
        }

        private string selectedMachine;
        /// <summary>
        /// The machine that has been selected in the search dialog
        /// </summary>
        public string SelectedMachine
        {
            get { return selectedMachine; }
            set { selectedMachine = value; OnPropertyChanged("SelectedMachine"); }
        }

        private string tagToAdd;
        /// <summary>
        /// The tag that has been selected
        /// </summary>
        public string TagToAdd
        {
            get { return tagToAdd; }
            set { tagToAdd = value; OnPropertyChanged("TagToAdd"); }
        }
        // Search Results


        // Commands
        
        // SearchCommand
        private RelayCommand searchCommand;
        /// <summary>
        /// Command that searches the online store based on the criteria
        /// </summary>
        public RelayCommand SearchCommand
        {
            get
            {
                if (searchCommand == null)
                {
                    searchCommand = new RelayCommand(new Action(() =>
                    {
                        PerformSearch();
                    }));
                }
                return searchCommand;
            }
        }
        private void PerformSearch()
        {
            // TODO handle selectedpedal and selectedmachine ? add as 'special' tags??
            // Possible issues with string / int values? gotta be consistent
            var results =OnlineService.Search(CustomTags.ToList());
            foreach (var r in results)
            {
                Presets.Add(r);
            }
        }

        // AddTagCommand
        private RelayCommand addTagCommand;
        /// <summary>
        /// Command that adds a tag to the current list to be used for preset association / search depending on mode
        /// </summary>
        public RelayCommand AddTagCommand
        {
            get
            {
                if (addTagCommand == null)
                {
                    addTagCommand = new RelayCommand(new Action(() =>
                    {
                        PerformAddTag();
                    }));
                }
                return addTagCommand;
            }
        }
        private void PerformAddTag()
        {
            // TODO
            // 1. Add tag to CustomTags, get available values for combo
            customTags.Add(new Tag { TagName = TagToAdd, AvailableValues=OnlineService.GetExistingValuesForTag(TagToAdd) });
            OnPropertyChanged("CustomTags");
            // 2. Remove from AvaialableTags
            AvailableTags.Remove(TagToAdd);
        }

        private RelayCommand uploadCommand;
        /// <summary>
        /// Command that uploads the preset to teh store
        /// </summary>
        public RelayCommand UploadCommand
        {
            get
            {
                if (uploadCommand == null)
                {
                    uploadCommand = new RelayCommand(new Action(() =>
                    {
                        PerformUpload();
                    }));
                }
                return uploadCommand;
            }
        }
        private void PerformUpload()
        {
            var id=OnlineService.UploadPreset(uploadPreset.ToXmlPreset(), CustomTags.ToList());
        }


        private RelayCommand downloadCommand;
        /// <summary>
        /// Command that downloads the selected preset to the Editor
        /// </summary>
        public RelayCommand DownloadCommand
        {
            get
            {
                if (downloadCommand == null)
                {
                    downloadCommand = new RelayCommand(new Action(() =>
                    {
                        PerformDownload();
                    }));
                }
                return downloadCommand;
            }
        }
        private void PerformDownload()
        {
            var p = OnlineService.DownloadPreset(SelectedPreset.PresetId);
        }
    }
}
