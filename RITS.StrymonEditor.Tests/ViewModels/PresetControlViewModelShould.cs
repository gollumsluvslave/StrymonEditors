using System;
using System.Collections.Generic;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RITS.StrymonEditor.MIDI;
using RITS.StrymonEditor.Messaging;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;
namespace RITS.StrymonEditor.Tests
{


    [TestClass]
    public class PresetControlViewModelShould : TestContext<PresetControlViewModel>
    {

        [TestMethod]
        public void CantExecuteWhenBulkFetching()
        {
            var midiManagerMock = Container.GetMock<IStrymonMidiManager>();
            midiManagerMock.SetupGet(x => x.IsBulkFetching).Returns(true);
            Container.Register<string>("Fetch");
            // Spoof BulkFetch mode
            Container.Register<StrymonPedal>(TestHelper.TimelinePedal);

            Assert.IsFalse(Sut.Execute.CanExecute(null));

        }

        [TestMethod]
        public void ExecuteFetchSuccessfully()
        {
            var midiManagerMock = Container.GetMock<IStrymonMidiManager>();
            var mediatorMock = Container.GetMock<IMediator>();
            Container.Register<string>("Fetch");
            Container.Register<StrymonPedal>(TestHelper.TimelinePedal);
            Sut.Mediator = mediatorMock.Object;            
            Sut.Execute.Execute(null);
            mediatorMock.Verify(x => x.NotifyColleagues(ViewModelMessages.FetchPresetRequested, Sut.PresetIndex), Times.Once());

        }

        [TestMethod]
        public void ExecutePushSuccessfully()
        {
            var midiManagerMock = Container.GetMock<IStrymonMidiManager>();
            var mediatorMock = Container.GetMock<IMediator>();
            Container.Register<string>("Push");
            Container.Register<StrymonPedal>(TestHelper.TimelinePedal);
            Sut.Mediator = mediatorMock.Object;

            Sut.Execute.Execute(null);
            mediatorMock.Verify(x => x.NotifyColleagues(ViewModelMessages.PushPresetRequested, Sut.PresetIndex), Times.Once());

        }

        [TestMethod]
        public void SetPresetCorrectlyWhenLocked()
        {
            var pedal = TestHelper.TimelinePedal;
            Container.Register<string>("Fetch");
            Container.Register<StrymonPedal>(pedal);
            Globals.MachineLocked = true;
            Globals.LockedMachine = 0;
            var priv = new PrivateObject(pedal);
            // Spoof presetdata
            Dictionary<int, StrymonRawPreset> data = priv.GetField("rawPresetData") as Dictionary<int, StrymonRawPreset>;
            if (data != null)
            {
                data.Add(2, new StrymonRawPreset { Index = 2, Data = new byte[] { }, Machine = 0, Name = "test1" });
                data.Add(4, new StrymonRawPreset { Index = 4, Data = new byte[] { }, Machine = 0, Name = "test2" });
                data.Add(6, new StrymonRawPreset { Index = 6, Data = new byte[] { }, Machine = 0, Name = "test3" });
            }
            Sut.PresetIndex=2;
            Assert.AreEqual("01A : test1",Sut.PresetName );
            Globals.MachineLocked = false;
        }
        [TestMethod]
        public void SetPresetCorrectlyWhenLocked_2()
        {
            var pedal = TestHelper.TimelinePedal;
            Container.Register<string>("Fetch");
            Container.Register<StrymonPedal>(pedal);
            Globals.MachineLocked = true;
            Globals.LockedMachine = 0;
            var priv = new PrivateObject(pedal);
            Dictionary<int, StrymonRawPreset> data = priv.GetField("rawPresetData") as Dictionary<int, StrymonRawPreset>;
            if (data != null)
            {
                data.Add(2, new StrymonRawPreset { Index = 2, Data = new byte[] { }, Machine = 0, Name = "test1" });
                data.Add(4, new StrymonRawPreset { Index = 4, Data = new byte[] { }, Machine = 0, Name = "test2" });
                data.Add(6, new StrymonRawPreset { Index = 6, Data = new byte[] { }, Machine = 0, Name = "test3" });
            }
            Sut.PresetIndex = 6;
            Assert.AreEqual("03A : test3", Sut.PresetName);
            Globals.MachineLocked = false;

        }

        [TestMethod]
        public void SetPresetCorrectlyWhenLocked_3()
        {
            var pedal = TestHelper.TimelinePedal;
            Container.Register<string>("Fetch");
            Container.Register<StrymonPedal>(pedal);
            Globals.MachineLocked = true;
            Globals.LockedMachine = 0;
            var priv = new PrivateObject(pedal);
            // Spoof presetdata
            Dictionary<int, StrymonRawPreset> data = priv.GetField("rawPresetData") as Dictionary<int, StrymonRawPreset>;
            if (data != null)
            {
                data.Add(2, new StrymonRawPreset { Index = 2, Data = new byte[] { }, Machine = 0, Name = "test1" });
                data.Add(4, new StrymonRawPreset { Index = 4, Data = new byte[] { }, Machine = 0, Name = "test2" });
                data.Add(6, new StrymonRawPreset { Index = 6, Data = new byte[] { }, Machine = 0, Name = "test3" });
            }
            Sut.PresetIndex = 1; // Set to last
            Assert.AreEqual("03A : test3", Sut.PresetName);
            Globals.MachineLocked = false;

        }
        [TestMethod]
        public void SetPresetCorrectlyWhenLocked_4()
        {
            var pedal = TestHelper.TimelinePedal;
            Container.Register<string>("Fetch");
            Container.Register<StrymonPedal>(pedal);
            Globals.MachineLocked = true;
            Globals.LockedMachine = 0;
            var priv = new PrivateObject(pedal);
            Dictionary<int, StrymonRawPreset> data = priv.GetField("rawPresetData") as Dictionary<int, StrymonRawPreset>;
            if (data != null)
            {
                data.Add(2, new StrymonRawPreset { Index = 2, Data = new byte[] { }, Machine = 0, Name = "test1" });
                data.Add(4, new StrymonRawPreset { Index = 4, Data = new byte[] { }, Machine = 0, Name = "test2" });
                data.Add(6, new StrymonRawPreset { Index = 6, Data = new byte[] { }, Machine = 0, Name = "test3" });
            }
            Sut.PresetIndex = 7; // set to first
            Assert.AreEqual("01A : test1", Sut.PresetName);
            Globals.MachineLocked = false;


        }

        [TestMethod]
        public void SetBigSkyCorrectlyWhenLocked()
        {
            ClearContainer();
            var pedal = TestHelper.BigSkyPedal;
            var midiManagerMock = Container.GetMock<IStrymonMidiManager>();
            Globals.MachineLocked = true;
            Globals.LockedMachine = 0;
            var priv = new PrivateObject(pedal);
            // Spoof presetdata
            Dictionary<int, StrymonRawPreset> data = priv.GetField("rawPresetData") as Dictionary<int, StrymonRawPreset>;
            if (data != null)
            {
                data.Add(0, new StrymonRawPreset { Index = 0, Data = new byte[] { }, Machine = 0, Name = "test1" });
                data.Add(1, new StrymonRawPreset { Index = 1, Data = new byte[] { }, Machine = 0, Name = "test2" });
                data.Add(2, new StrymonRawPreset { Index = 2, Data = new byte[] { }, Machine = 0, Name = "test3" });
            }
            Sut = new PresetControlViewModel("Fetch", pedal, midiManagerMock.Object);
            Assert.AreEqual("00A : test1", Sut.PresetName);
            Sut.PresetIndex = 1;
            Assert.AreEqual("00B : test2", Sut.PresetName);
            Sut.PresetIndex = 2;
            Assert.AreEqual("00C : test3", Sut.PresetName);
            Globals.MachineLocked = false;
        }
    }
}
