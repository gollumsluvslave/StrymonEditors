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
    /// Control that represents a dial in the physical pedal
    /// </summary>
    public partial class PotControl : UserControl
    {
        bool handleKeyboard;
        Point captureLocation;
        
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(PotControl), new UIPropertyMetadata(0.0));

        /// <summary>
        /// The angle of the pot
        /// </summary>
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set 
            {
                if (value < 0)value = 0;
                if (value > 290) value = 290;
                SetValue(AngleProperty, value); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public PotControl()
        {
            InitializeComponent();
            this.PreviewMouseWheel += Zoom_MouseWheel;
            this.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
            this.MouseUp += new MouseButtonEventHandler(OnMouseUp);
            this.MouseMove += new MouseEventHandler(OnMouseMove);
            this.PreviewKeyUp+= new KeyEventHandler(PotControl_KeyUp);
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(this);
            captureLocation = Mouse.GetPosition(this);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.Captured == this)
            {
                // Get the current mouse position relative to the volume control
                Point currentLocation = Mouse.GetPosition(this);
                // Get Y delta
                double diffX = (currentLocation.X - captureLocation.X);
                this.Angle = diffX;
            }
        }

        private void Zoom_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            bool handle = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
            // We don't want to 
            double inc = handle ? (e.Delta / 120) : (e.Delta / 10);
            this.Angle += inc;
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            handleKeyboard = true;
        }

        private void PotControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (handleKeyboard)
            {
                int inc = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) > 0 ? 1 : 10;
                if (e.Key == Key.PageUp)
                {
                    this.Angle += inc;
                }
                else if (e.Key == Key.PageDown)
                {
                    this.Angle -= inc;
                }
            }
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            handleKeyboard = false;
        }

        
    }
}
