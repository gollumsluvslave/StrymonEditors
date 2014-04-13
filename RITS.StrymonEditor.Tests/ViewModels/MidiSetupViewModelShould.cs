using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;
using RITS.StrymonEditor.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace RITS.StrymonEditor.Tests
{
    [TestClass()]
    public class MidiSetupViewModelShould : TestContext<MidiSetupViewModel>
    {

        [TestMethod]
        public void UpdateDevicesCorrectly()
        {
            Container.Register<Action>(new Action(() => { }));
            var midiInDevice = Sut.ConfiguredInputDevice;
            Sut.ConfiguredInputDevice = null;
            Assert.IsNull(Sut.ConfiguredInputDevice);
            var midiOutDevice = Sut.ConfiguredOutputDevice;
            Sut.ConfiguredOutputDevice = null;
            Assert.IsNull(Sut.ConfiguredOutputDevice);
            // Reset state to avoid foulding other tests
            Sut.ConfiguredInputDevice = midiInDevice;
            Sut.ConfiguredOutputDevice = midiOutDevice;
        }

        [TestMethod]
        public void UpdateMidiChannelsCorrectly()
        {
            Container.Register<Action>(new Action(() => { }));
            Sut.TimelineMidiChannel = 16;
            Assert.AreEqual(16, Sut.TimelineMidiChannel);
            Sut.MobiusMidiChannel = 16;
            Assert.AreEqual(16, Sut.MobiusMidiChannel);
            Sut.BigSkyMidiChannel = 16;
            Assert.AreEqual(16, Sut.BigSkyMidiChannel);
            // Reset
            Sut.TimelineMidiChannel = 1;
            Sut.MobiusMidiChannel = 2;
            Sut.BigSkyMidiChannel = 4;
        }

        [TestMethod]
        public void UpdateDelaysCorrectly()
        {
            Container.Register<Action>(new Action(() => { }));
            Sut.BulkFetchDelay = 100;
            Assert.AreEqual(100, Sut.BulkFetchDelay);
            Sut.PushChunkSize = 20;
            Assert.AreEqual(20, Sut.PushChunkSize);
            Sut.PushChunkDelay = 10;
            Assert.AreEqual(10, Sut.PushChunkDelay);
            // Reset
            Sut.BulkFetchDelay = 300;
            Sut.PushChunkSize = 0;
            Sut.PushChunkDelay = 0;
        }

        [TestMethod]
        public void ReturnCorrectLists()
        {
            Container.Register<Action>(new Action(() => { }));
            Assert.AreEqual(16, Sut.MidiChannels.Count);
            Assert.AreEqual(8, Sut.BulkFetchDelays.Count);
            Assert.AreEqual(8, Sut.PushChunkSizes.Count);
            Assert.AreEqual(50, Sut.PushChunkDelays.Count);
            foreach(var x in Sut.InputDevices)
            {
            }
            foreach (var x in Sut.OutputDevices)
            {
            }
        }


        [TestMethod]
        public void ReinitMidiOnInputDeviceChangeCorrectly()
        {
            var midiMock = Container.GetMock<IStrymonMidiManager>();            
            Container.Register<Action>(new Action(() => { }));
            var midiInDevice = Sut.ConfiguredInputDevice;
            Sut.ConfiguredInputDevice = null;
            Sut.OKCommand.Execute(null);

            midiMock.Verify(x => x.InitMidi(), Times.Once());

            Sut.ConfiguredInputDevice = midiInDevice;
        }

        [TestMethod]
        public void ReinitMidiOnOutputDeviceChangeCorrectly()
        {
            var midiMock = Container.GetMock<IStrymonMidiManager>();
            Container.Register<Action>(new Action(() => { }));
            var midiOutDevice = Sut.ConfiguredOutputDevice;
            Sut.ConfiguredOutputDevice = null;
            Sut.OKCommand.Execute(null);

            midiMock.Verify(x => x.InitMidi(), Times.Once());
            Sut.ConfiguredOutputDevice = midiOutDevice;
        }

        [TestMethod]
        public void ReinitMidiOnBothDeviceChangeCorrectly()
        {
            var midiMock = Container.GetMock<IStrymonMidiManager>();
            Container.Register<Action>(new Action(() => { }));
            var midiInDevice = Sut.ConfiguredInputDevice;
            var midiOutDevice = Sut.ConfiguredOutputDevice;
            Sut.ConfiguredInputDevice = null;
            Sut.ConfiguredOutputDevice = null;
            Sut.OKCommand.Execute(null);

            midiMock.Verify(x => x.InitMidi(), Times.Once());
            Sut.ConfiguredInputDevice = midiInDevice;
            Sut.ConfiguredOutputDevice = midiOutDevice;
        }
    }
}
