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
namespace RITS.StrymonEditor.Views
{
    /// <summary>
    /// Simple single text field data entry dialog
    /// </summary>
    public partial class Dialog : Window
    {
        DialogViewModel vm;
        public Dialog(DialogViewModel dataContext)
        {
            vm = dataContext;
            vm.CloseAction = this.Close;
            DataContext = dataContext;
            InitializeComponent();
            InputBox.PreviewTextInput += InputBox_PreviewTextInput;
            InputBox.Focus();
        }

        private void InputBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (vm.InputInvalid(e.Text)) e.Handled = true;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            button1.Focus();
        }
    }
}
