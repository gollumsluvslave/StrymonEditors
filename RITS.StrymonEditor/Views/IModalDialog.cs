using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;

namespace RITS.StrymonEditor.Views
{
    /// <summary>
    /// Very simple interface to allow mocks for the various 'input' dialogs based on the <see cref="Dialog"/> view
    /// </summary>
    public interface IModalDialog
    {
        /// <summary>
        /// Shows the dialog modally
        /// </summary>
        void ShowModal();
    }

    /// <summary>
    /// Implementation of <see cref="IModalDialog"/> for DirectEntry operations
    /// </summary>
    public class DirectEntryDialog : IModalDialog 
    {
        private Dialog dlg;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fineValue"></param>
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
    /// Implementation of <see cref="IModalDialog"/> for Preset rename operations
    /// </summary>
    public class PresetRenameDialog : IModalDialog
    {
        private Dialog dlg;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
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
    /// Implementation of <see cref="IModalDialog"/> for Modal Progress operations
    /// </summary>
    public class ModalProgressBar : IModalDialog
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

    /// <summary>
    /// Wrapper around the <see cref="PedalEditor"/> window to allow unit testing
    /// </summary>
    public class PedalEditorWindow : IModalDialog
    {
        private PedalEditor editor;

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="preset"></param>
        /// <param name="midiManager"></param>
        public PedalEditorWindow(StrymonPreset preset,IStrymonMidiManager midiManager)
        {
            editor = new PedalEditor(preset,midiManager);
        }
        
        /// <inheritdoc/>
        public void ShowModal()
        {
            editor.ShowDialog();
        }
    }
}
