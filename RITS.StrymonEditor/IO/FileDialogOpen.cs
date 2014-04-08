using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
namespace RITS.StrymonEditor.IO
{
    /// <summary>
    /// Basic wrapper implementation of <see cref="IFileDialog"/> around OpenFileDialog to allow testing
    /// </summary>
    public class FileDialogOpen : IFileDialog
    {
        private OpenFileDialog dialog;
        public FileDialogOpen()
        {
            dialog = new OpenFileDialog();
        }
        
        public string DefaultExt 
        {
            get { return dialog.DefaultExt; }
            set { dialog.DefaultExt = value; }
        }
        public string Filter
        {
            get { return dialog.Filter; }
            set { dialog.Filter = value; }
        }
        public string FileName
        {
            get { return dialog.FileName; }
            set { dialog.FileName = value; }
        }

        public bool? ShowDialog()
        {
            return dialog.ShowDialog();
        }
    }

    
}
