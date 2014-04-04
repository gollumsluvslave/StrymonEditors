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
    /// Interaction logic for TimelineView.xaml
    /// </summary>
    public partial class EditorView : UserControl
    {
        private Point dragStartPoint;
        private StrymonPedalViewModel vm;
        private ParameterViewModel dragParam;
        private PotViewModel dropPot;
        public EditorView()
        {
            InitializeComponent();
            vm = this.DataContext as StrymonPedalViewModel;
        }
        
        private void View_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            Image targetItem = e.OriginalSource as Image;
            if (targetItem != null)
            {
                dropPot = targetItem.DataContext as PotViewModel;
                if (dropPot != null && dropPot.IsDynamicControlPot)
                {
                    e.Effects = DragDropEffects.Link;
                }
            }
        }

        private void View_PreviewDrop(object sender, DragEventArgs e)
        {
            if (dragParam == null || dropPot == null) return;
            // TODO move to PedalViewmodel, ensure previous link fully broken
            dragParam.AssignToDynamicPot(dropPot);
        }

        //event handler for the mouse down of the listbox. 
        //This will start the drag drop operation
        private void View_MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            dragParam = null;
            dropPot = null;
            var fe = e.OriginalSource as TextBlock;
            if (fe ==null) return;
            try
            {
                StackPanel paramStackPanel = WPFUtils.VisualUpwardSearch<StackPanel>(e.OriginalSource as DependencyObject) as StackPanel;
                dragParam = paramStackPanel.DataContext as ParameterViewModel;
                if (dragParam != null)
                {
                    dragStartPoint = e.GetPosition(null);
                }
            }
            catch { }
        }

        //event handler for the mouse down of the listbox. 
        //This will start the drag drop operation
        private void View_MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (dragParam == null) return;
            try
            {
                if (IsDragging(dragStartPoint, e))
                {
                    DragDrop.DoDragDrop(e.OriginalSource as DependencyObject, dragParam, DragDropEffects.Link);
                }                
            }
            catch
            {
                e.Handled = true;
            }
        }

        private bool IsDragging(Point dragStartPoint, MouseEventArgs e)
        {
            var diff = e.GetPosition(null) - dragStartPoint;
            return e.LeftButton == MouseButtonState.Pressed && (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance);
        }

        private StackPanel TargetFromDragEventArgs(DragEventArgs e)
        {
            FrameworkElement tmp = e.OriginalSource as FrameworkElement;

            while (tmp != null && !(tmp is StackPanel))
            {
                tmp = tmp.Parent as FrameworkElement;
            }

            return tmp as StackPanel;
        }

        private void UserControl_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void UserControl_PreviewDragLeave(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }    }
}
