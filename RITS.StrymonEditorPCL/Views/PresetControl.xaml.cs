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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RITS.StrymonEditor.Views
{
    /// <summary>
    /// PresetControl that represents a view into a pedal in terms of the presets
    /// Displays the name of the preset with buttons to cycle through the list
    /// and scroll-wheel behaviour also
    /// </summary>
    public partial class PresetControl : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(string), typeof(PresetControl), new UIPropertyMetadata("Fetch"));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PresetIndexProperty =
            DependencyProperty.Register("PresetIndex", typeof(int), typeof(PresetControl), new UIPropertyMetadata(0));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PresetNameProperty =
            DependencyProperty.Register("PresetName", typeof(string), typeof(PresetControl), new UIPropertyMetadata("New"));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PresetIsEnabledProperty =
            DependencyProperty.Register("PresetIsEnabled", typeof(bool), typeof(PresetControl), new UIPropertyMetadata(false));

        /// <summary>
        /// 
        /// </summary>
        public PresetControl()
        {
            InitializeComponent();
            this.PreviewMouseWheel += Zoom_MouseWheel;

            
        }

        /// <summary>
        /// Mode of operation
        /// </summary>
        public string Mode
        {
            get { return (string)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        /// <summary>
        /// Current preset name
        /// </summary>
        public string PresetName
        {
            get { return (string)GetValue(PresetNameProperty); }
            set 
            {
                SetValue(PresetNameProperty, value); 
            }
        }

        /// <summary>
        /// Current preset index
        /// </summary>
        public int PresetIndex
        {
            get { return (int)GetValue(PresetIndexProperty); }
            set 
            {
                SetValue(PresetIndexProperty, value);
            }
        }

        /// <summary>
        /// Whether preset is enabled
        /// </summary>
        public bool PresetIsEnabled
        {
            get { return (bool)GetValue(PresetIsEnabledProperty); }
            set
            {
                SetValue(PresetIsEnabledProperty, value);
            }
        }



        private void Zoom_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var vm = DataContext as ViewModels.PresetControlViewModel;
            if (e.Delta > 0)
            {
                vm.PresetIndex++;            
            }
            else
            {
                vm.PresetIndex--;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ViewModels.PresetControlViewModel;
            vm.PresetIndex--;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ViewModels.PresetControlViewModel;
            vm.PresetIndex++;
        }


    }
}
