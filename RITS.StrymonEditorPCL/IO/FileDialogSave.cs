using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
namespace RITS.StrymonEditor.IO
{
    /// <summary>
    /// Basic wrapper implementation of <see cref="IFileDialog"/> around SaveFileDialog to allow testing
    /// </summary>
    public class FileDialogSave : IFileDialog
    {
        private SaveFileDialog dialog;
        public FileDialogSave()
        {
            dialog = new SaveFileDialog();
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
