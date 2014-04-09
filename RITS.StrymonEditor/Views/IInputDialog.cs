using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RITS.StrymonEditor.ViewModels;

namespace RITS.StrymonEditor.Views
{
    public interface IInputDialog
    {
        void ShowModal();
    }

    public class DirectEntryDialog : IInputDialog 
    {
        private Dialog dlg;
        public DirectEntryDialog(string fineValue)
        {
            dlg = new Dialog(new DirectEntryViewModel(fineValue));
        }
        public void ShowModal()
        {
            dlg.ShowDialog();
        }
    }

    public class PresetRenameDialog : IInputDialog
    {
        private Dialog dlg;
        public PresetRenameDialog(string name)
        {
            dlg = new Dialog(new PresetRenameViewModel(name));
        }
        public void ShowModal()
        {
            dlg.ShowDialog();
        }
    }

    public class ModalProgressBar : IInputDialog
    {
        private ModalProgressDialog dlg;
        public ModalProgressBar(ModalProgressDialogViewModel vm)
        {
            dlg = new ModalProgressDialog(vm);
        }

        public void ShowModal()
        {
            dlg.ShowDialog();
        }
    }
}
