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
    /// Interaction logic for PresetControl.xaml
    /// </summary>
    public partial class PresetControl : UserControl
    {
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(string), typeof(PresetControl), new UIPropertyMetadata("Fetch"));

        public static readonly DependencyProperty PresetIndexProperty =
            DependencyProperty.Register("PresetIndex", typeof(int), typeof(PresetControl), new UIPropertyMetadata(0));

        public static readonly DependencyProperty PresetNameProperty =
            DependencyProperty.Register("PresetName", typeof(string), typeof(PresetControl), new UIPropertyMetadata("New"));

        public static readonly DependencyProperty PresetIsEnabledProperty =
            DependencyProperty.Register("PresetIsEnabled", typeof(bool), typeof(PresetControl), new UIPropertyMetadata(false));

        public PresetControl()
        {
            InitializeComponent();
            this.PreviewMouseWheel += Zoom_MouseWheel;

            
        }

        public string Mode
        {
            get { return (string)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public string PresetName
        {
            get { return (string)GetValue(PresetNameProperty); }
            set 
            {
                SetValue(PresetNameProperty, value); 
            }
        }

        public int PresetIndex
        {
            get { return (int)GetValue(PresetIndexProperty); }
            set 
            {
                SetValue(PresetIndexProperty, value);
            }
        }

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
