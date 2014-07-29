using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Non enumeration based type to encapsulate a MIDI CC Message
    /// TODO - should be UInt not Int
    /// TODO - add validation
    /// </summary>
    public class ControlChangeMsg
    {
        /// <summary>
        /// The CC number
        /// </summary>
        public int ControlChangeNo { get; set; }

        /// <summary>
        /// The value to be sent
        /// </summary>
        public int Value { get; set; }
    }
}
