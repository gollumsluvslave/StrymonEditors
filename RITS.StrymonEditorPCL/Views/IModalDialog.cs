using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RITS.StrymonEditor.MIDI;
using RITS.StrymonEditor.IO;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;

namespace RITS.StrymonEditor.Views
{
    

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

    public class PresetStoreDialog : IModalDialog
    {
        private PresetStoreWindow window;
        public PresetStoreDialog(StrymonPreset preset, bool fromMainWindow)
        {
            window = new PresetStoreWindow(preset, fromMainWindow);
        }

        public void ShowModal()
        {
            window.ShowDialog();
        }

    }
}
