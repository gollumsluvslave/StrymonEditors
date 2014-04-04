using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor
{
    /// <summary>
    /// Message types that trigger comms between view models
    /// </summary>
    public enum ViewModelMessages
    {
        Shutdown,
        MachineSelected,
        ParameterChanged,
        LCDUpdate,
        ActivePresetDirty,
        EPSetStarted,
        EPSetHeelComplete,
        EPSetComplete,
        EPSetCancelled,
        RequestPedalClose,
        ReceivedPresetFromPedal,
        ReceivedCC,
        FetchPresetRequested,
        PushPresetRequested,
        BulkPresetRead,
        PedalConnected,
        MIDIConnectionComplete,
        BulkLoadStarted,
        BulkLoadComplete,
        PresetRenamed,
        DirectEntryValueEntered
    }

    /// <summary>
    /// Colleague that can register to messages from the Mediator
    /// Credit to Marlon Grech for the initial implementation and idea - made some tweaks here and there
    /// http://marlongrech.wordpress.com/2008/03/20/more-than-just-mvc-for-wpf/
    /// Look at updating to v2 with WeakAction etc
    /// </summary>
    public interface IColleague
    {
        /// <summary>
        /// Gets or sets the mediator for this controller
        /// </summary>
        Mediator Mediator { get; }

    }

    public class KeyEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, object> keyExtractor;

        public KeyEqualityComparer(Func<T, object> keyExtractor)
        {
            this.keyExtractor = keyExtractor;
        }

        public bool Equals(T x, T y)
        {
            return this.keyExtractor(x).Equals(this.keyExtractor(y));
        }

        public int GetHashCode(T obj)
        {
            return this.keyExtractor(obj).GetHashCode();
        }
    } 

}
