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
using RITS.StrymonEditor.ViewModels;
using RITS.StrymonEditor.IO;

namespace RITS.StrymonEditor.Views
{
    /// <summary>
    /// Dialog that displays a modal window and progressbar to display a longrunning operation that prohibits
    /// other UI interaction - currently only used for restoring a Pedal backup.
    /// </summary>
    public partial class ModalProgressDialog : Window
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vm"></param>
        public ModalProgressDialog(ModalProgressDialogViewModel vm)
        {
            vm.Close = this.Close;
            ViewModel = vm;
            InitializeComponent();

        }

        /// <summary>
        /// 
        /// </summary>
        public ModalProgressDialogViewModel ViewModel
        {
            get;
            private set;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            ViewModel.Start();
        }
    }
}
