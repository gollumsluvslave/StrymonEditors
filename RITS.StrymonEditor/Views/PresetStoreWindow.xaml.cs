using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;

namespace RITS.StrymonEditor.Views
{
    /// <summary>
    /// Interaction logic for PresetStoreWindow.xaml
    /// </summary>
    public partial class PresetStoreWindow : Window
    {
        private StrymonPreset uploadPreset;
        private bool fromMainWindow;
        public PresetStoreWindow(StrymonPreset preset, bool fromMainWindow)
        {
            this.uploadPreset=preset;
            this.fromMainWindow = fromMainWindow;
            InitializeComponent();
        }

        private PresetStoreViewModel viewModel;
        /// <summary>
        /// Exposes the <see cref="PresetStoreViewModel"/> viewmodel for databinding
        /// </summary>
        public PresetStoreViewModel ViewModel
        {
            get
            {
                if (viewModel == null)
                {
                    viewModel = uploadPreset == null ? new PresetStoreViewModel(fromMainWindow) : new PresetStoreViewModel(uploadPreset);
                    viewModel.OnlineService = new IO.StubOnlineService(); // TODO swap out later
                    viewModel.Close = this.Close;
                }
                return viewModel;
            }
        }        

    }
}
