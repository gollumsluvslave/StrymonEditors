using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace RITS.StrymonEditor.IO
{
    /// <summary>
    /// Basic wrapper implementation of <see cref="IMessageDialog"/> around MessageBox to allow testing
    /// </summary>
    public class MessageDialog : IMessageDialog 
    {        
        public MessageDialog()
        {
        }
        public bool ShowInfo(string message, string caption)
        {
            if (MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK) return true;
            return false;
        }
        
        public bool ShowYesNo(string message, string caption)
        {
            if (MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) return true;
            return false;
        }
        
        public bool ShowError(string message, string caption)
        {
            if (MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK) return true;
            return false;
        }
    }
}
