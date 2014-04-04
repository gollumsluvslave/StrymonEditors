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
using RITS.StrymonEditor.ViewModels;

namespace RITS.StrymonEditor.Views
{
    /// <summary>
    /// Interaction logic for VolumeControl.xaml
    /// </summary>
    public partial class EncoderControl : UserControl
    {
        // Using a DependencyProperty backing store for Angle.
        public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register("Data", typeof(int), typeof(EncoderControl), new UIPropertyMetadata(0));
        private bool handleKeyboard=false;
    public int Data
    {
        get { return (int)GetValue(DataProperty); }
        set { SetValue(DataProperty, value); }
    }

    public EncoderControl()
    {
        InitializeComponent();
        this.PreviewMouseWheel += Zoom_MouseWheel;
        this.PreviewKeyUp += new KeyEventHandler(PotControl_KeyUp);

    }

    private void PotControl_KeyUp(object sender, KeyEventArgs e)
    {
        if (handleKeyboard)
        {
            var vm = DataContext as PotViewModel;
            int inc = vm.ContextPedal.FineIncrement(Data);
            if (e.Key == Key.PageUp)
            {
                // This data needs to be dynamic by pedal

                this.Data += inc;
            }
            else if (e.Key == Key.PageDown)
            {
                this.Data -= inc;
            }
        }
    }


    private void UserControl_GotFocus(object sender, RoutedEventArgs e)
    {
        handleKeyboard = true;
    }

    private void UserControl_LostFocus(object sender, RoutedEventArgs e)
    {
        handleKeyboard = false;
    }

    private void Zoom_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        bool handle = (Keyboard.Modifiers & ModifierKeys.Control) > 0;

        var vm = DataContext as PotViewModel;
        int inc = vm.ContextPedal.FineIncrement(Data);
        if (e.Delta < 0) inc = -inc;
        this.Data += inc;

    }


    }
}
