using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RITS.StrymonEditor.ViewModels;

namespace RITS.StrymonEditor.Views
{
    /// <summary>
    /// Very simple interface to allow mocks for the various 'input' dialogs based on the <see cref="Dialog"/> view
    /// </summary>
    public interface IInputDialog
    {
        /// <summary>
        /// Shows the dialog modally
        /// </summary>
        void ShowModal();
    }

    /// <summary>
    /// Implementation of <see cref="IInputDialog"/> for DirectEntry operations
    /// </summary>
    public class DirectEntryDialog : IInputDialog 
    {
        private Dialog dlg;
        public DirectEntryDialog(string fineValue)
        {
            dlg = new Dialog(new DirectEntryViewModel(fineValue));
        }

        /// <inheritdoc/>
        public void ShowModal()
        {
            dlg.ShowDialog();
        }
    }

    /// <summary>
    /// Implementation of <see cref="IInputDialog"/> for Preset rename operations
    /// </summary>
    public class PresetRenameDialog : IInputDialog
    {
        private Dialog dlg;
        public PresetRenameDialog(string name)
        {
            dlg = new Dialog(new PresetRenameViewModel(name));
        }
        /// <inheritdoc/>
        public void ShowModal()
        {
            dlg.ShowDialog();
        }
    }

    /// <summary>
    /// Implementation of <see cref="IInputDialog"/> for Modal Progress operations
    /// </summary>
    public class ModalProgressBar : IInputDialog
    {
        private ModalProgressDialog dlg;
        public ModalProgressBar(ModalProgressDialogViewModel vm)
        {
            dlg = new ModalProgressDialog(vm);
        }

        /// <inheritdoc/>
        public void ShowModal()
        {
            dlg.ShowDialog();
        }
    }
}
