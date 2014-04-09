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
    /// Interaction logic for ModalProgressDialog.xaml
    /// </summary>
    public partial class ModalProgressDialog : Window
    {
        public ModalProgressDialog(ModalProgressDialogViewModel vm)
        {
            vm.Close = CloseMe;
            ViewModel = vm;
            InitializeComponent();

        }

        public ModalProgressDialogViewModel ViewModel
        {
            get;
            private set;
        }

        private void CloseMe()
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            ViewModel.Start();
        }
    }
}
