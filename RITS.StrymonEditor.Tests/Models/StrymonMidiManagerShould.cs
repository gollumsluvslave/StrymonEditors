using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Midi;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.Messaging;
namespace RITS.StrymonEditor.Tests.Models
{
    [TestClass]
    public class StrymonMidiManagerShould : TestContext<StrymonMidiManager>
    {
        private Parameter testParam = new Parameter { Definition = new ParameterDef { ControlChangeNo = 1 } };

        #region Initialisation Tests
        [TestMethod]
        public void InitialiseGracefullyWithNoMidiDevices()
        {
            var sut = new StrymonMidiManager(null, null);
            sut.InitMidi();
            Assert.AreEqual(0, sut.ConnectedPedals.Count);

        }

        [TestMethod]
        public void OpenMidiDevicesOnInit()
        {
            // Arrange
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 1);
            // Assert
            midiInMock.Verify(x => x.Open(), Times.Once());
            midiInMock.Verify(x => x.StartReceiving(null,true), Times.Once());
            midiOutMock.Verify(x => x.Open(), Times.Once());            
            midiOutMock.Verify(x => x.SendSysEx(It.IsAny<byte[]>()), Times.Once());
        }

        [TestMethod]
        public void BeClosedWithNoInit()
        {
            // Arrange
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 0);
            // Assert
            midiInMock.Verify(x => x.Open(), Times.Never());
            midiInMock.Verify(x => x.StartReceiving(null,true), Times.Never());
            midiOutMock.Verify(x => x.Open(), Times.Never());
            midiOutMock.Verify(x => x.SendSysEx(It.IsAny<byte[]>()), Times.Never());
        }

        [TestMethod]
        public void SetConnnectedPedalOnValidSysExEvent()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 2);
            // Assert
            Assert.AreEqual(1,Sut.ConnectedPedals.Count);
        }

        [TestMethod]
        public void NotSetConnnectedPedalOnInvalidSysExEvent()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 2, 0x07);
            // Assert
            Assert.AreEqual(0, Sut.ConnectedPedals.Count);
        }

        [TestMethod]
        public void NotSetConnnectedPedalOnInvalidPedalSysExEvent()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 2, 0x00, 0x00);
            // Assert
            Assert.AreEqual(0, Sut.ConnectedPedals.Count);
        }

        [TestMethod]
        public void IgnoreEchoSysIdentityResonse()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 1);
            midiInMock.Raise(x => x.SysEx += null,new SysExMessage(new DeviceBase("dummy"), StrymonMidiManager.IdentifyRequest, new float()));
            // Assert
            Assert.AreEqual(0, Sut.ConnectedPedals.Count);
        }


        [TestMethod]
        public void SetContextPedalCorrectly()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();

            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);

            // Assert
            Assert.IsTrue(Sut.IsConnected);
        }
        #endregion

        #region Dispose Tests
        [TestMethod]
        public void DisposeGracefully()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            // TODO - This test should probably fail as the CC data is not valid!
            Sut.Dispose();
            // Assert
            midiInMock.Verify(x => x.Close(), Times.Once());
            midiInMock.Verify(x => x.StopReceiving(), Times.Once());
            midiOutMock.Verify(x => x.Close(), Times.Once());

        }
        #endregion

        #region CC Send Tests
        [TestMethod]
        public void SendCCOnSynchMachineWhenConnected()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();            
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            // TODO - This test should probably fail as the CC data is not valid!
            Sut.SynchMachine(new StrymonMachine()); 
            // Assert
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [TestMethod]
        public void NotSendCCOnSynchMachineWhenNotConnected()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 2);
            Sut.SynchMachine(new StrymonMachine());
            // Assert
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }

        [TestMethod]
        public void SendCCOnSynchParameterWhenConnected()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            Sut.SynchParameter(testParam);
            // Assert
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        // Tricky to test use a simple direct entry that requires 2 additional CC messages
        [TestMethod]
        public void SendCCOnSynchParameterDirectEntry()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            testParam.Definition.PotId = 1; // Force FineControl
            testParam.DirectEntryChange = true;
            // Use default Timeline fine range
            testParam.Definition.Range = new RITS.StrymonEditor.Models.Range { MinValue = 0, MaxValue = 127 };
            // Use default Timeline fine range
            testParam.Definition.FineRange = new RITS.StrymonEditor.Models.Range { MinValue = 60, MaxValue = 2500 };
            testParam.Value = 0; // Set param coarse value to min
            testParam.FineValue = 62; // Set fine value to 2 greater, to force 2 additional CC messages
            Sut.SynchParameter(testParam);
            testParam.Value = 127; // Set param coarse value to max
            testParam.FineValue = 2499; // Set fine value to 1 less, to force an additional CC messages, with decrease value
            Sut.SynchParameter(testParam);
            
            // Assert
            // 5 sends in total
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(5));
            // 2 send was FineControl and was 0 (increment 1)
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), StrymonPedal.ValueEncoderCC, 0), Times.Exactly(2));
            // 1 send was FineControl and was 1 (deccrement 1)
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), StrymonPedal.ValueEncoderCC, 1), Times.Once());
        }

        // Tricky to test use a simple direct entry that requires one additional CC
        [TestMethod]
        public void SendCCOnSynchParameterFineEncoderLastChange()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            testParam.Definition.PotId = 1; // Force FineControl
            testParam.FineValue = 60;
            testParam.FineValue = 61; // Increase - 0 cc value
            testParam.FineEncoderLastChange = true;
            Sut.SynchParameter(testParam);
            testParam.FineValue = 60; // Decrease - 0 cc value
            testParam.FineEncoderLastChange = true;
            Sut.SynchParameter(testParam);
            // Assert
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), StrymonPedal.ValueEncoderCC, 0), Times.Once());
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), StrymonPedal.ValueEncoderCC, 1), Times.Once());
        }

        [TestMethod]
        public void SendLooperCCs()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            Sut.SendLooperRecord();
            Sut.SendLooperPlay();
            Sut.SendLooperStop();
            Sut.SendLooperUndo();
            Sut.SendLooperRedo();
            Sut.SendLooperReverse();
            Sut.SendLooperFullHalf();
            Sut.SendLooperPrePost();
            // Assert
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(8));
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), StrymonPedal.LooperRecord, It.IsAny<int>()), Times.Once());
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), StrymonPedal.LooperPlay, It.IsAny<int>()), Times.Once());
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), StrymonPedal.LooperStop, It.IsAny<int>()), Times.Once());
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), StrymonPedal.LooperUndo, It.IsAny<int>()), Times.Once());
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), StrymonPedal.LooperRedo, It.IsAny<int>()), Times.Once());
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), StrymonPedal.LooperReverse, It.IsAny<int>()), Times.Once());
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), StrymonPedal.LooperFullHalf, It.IsAny<int>()), Times.Once());
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), StrymonPedal.LooperPrePost, It.IsAny<int>()), Times.Once());
        }

        [TestMethod]
        public void SendVirtualEPCC()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            Sut.SendVirtualEP(0);
            // Assert
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), StrymonPedal.ExpressionCC, It.IsAny<int>()), Times.Once());
        }

        [TestMethod]
        public void SendInfiniteCC()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            Sut.SendInfinite(0);
            // Assert
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), StrymonPedal.HoldCC, It.IsAny<int>()), Times.Once());
        }


        [TestMethod]
        public void NotSendCCOnSynchParameterWhenNotConnected()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 2);
            Sut.SynchParameter(testParam);
            // Assert
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }

        [TestMethod]
        public void NotSendCCOnSynchParameterWhenCCNoIsZero()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            testParam.Definition.ControlChangeNo = 0;
            Sut.SynchParameter(testParam);
            // Assert
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }
        [TestMethod]
        public void NotSendCCOnSynchParameterWhenDisableSendCCIsSetToTrue()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            Sut.DisableControlChangeSends = true;
            Sut.SynchParameter(testParam);
            // Assert
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }
        [TestMethod]
        public void NotSendCCOnSynchParameterWhenSyncModeIsPedalMaster()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            Sut.SyncMode = SyncMode.PedalMaster;
            Sut.SynchParameter(testParam);
            // Assert
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }
        #endregion

        #region CC Receive Tests
        [TestMethod]
        public void ReceiveCCMessageWhenConnected()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            var mediatorMock = Container.GetMock<IMediator>();
            Sut.Mediator = mediatorMock.Object;
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            midiInMock.Raise(x => x.ControlChange += null, new ControlChangeMessage(new DeviceBase("dummy"),Channel.Channel1,1,0,new float()));
            // Assert
            mediatorMock.Verify(x => x.NotifyColleagues(ViewModelMessages.ReceivedCC, It.IsAny<ControlChangeMsg>()), Times.Once());

        }
        [TestMethod]
        public void IgnoreCCEchoMessageWhenConnected()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            var mediatorMock = Container.GetMock<IMediator>();
            Sut.Mediator = mediatorMock.Object;
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            Sut.SynchParameter(testParam);
            midiInMock.Raise(x => x.ControlChange += null, new ControlChangeMessage(new DeviceBase("dummy"), Channel.Channel1, 1, 0, new float()));
            // Assert
            mediatorMock.Verify(x => x.NotifyColleagues(ViewModelMessages.ReceivedCC, It.IsAny<ControlChangeMsg>()), Times.Never());

        }

        [TestMethod]
        public void ReceivePCChange()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            var mediatorMock = Container.GetMock<IMediator>();
            Sut.Mediator = mediatorMock.Object;
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            midiInMock.Raise(x => x.ProgramChange += null, new ProgramChangeMessage(new DeviceBase("dummy"), Channel.Channel1, Instrument.AcousticGrandPiano, new float()));
            // Assert
            var data = TimelineEditBufferFetchData;
            midiOutMock.Verify(x => x.SendSysEx(data), Times.Once());
        }
        #endregion

        #region Fetch Tests
        [TestMethod]
        public void SendSysExForFetchCurrentWhenConnected()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            var mediatorMock = Container.GetMock<IMediator>();
            Sut.Mediator = mediatorMock.Object;
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            Sut.FetchCurrent();
            var presetData = new StrymonSysExUtils.StrymonSysExMessage(Sut.ConnectedPedals.First()).FullMessageData;
            midiInMock.Raise(x => x.SysEx += null, new SysExMessage(new DeviceBase("dummy"),
                               presetData, new float()));
            // Assert
            var data = TimelineEditBufferFetchData;
            midiOutMock.Verify(x => x.SendSysEx(data), Times.Once());
            mediatorMock.Verify(x => x.NotifyColleagues(ViewModelMessages.ReceivedPresetFromPedal, It.IsAny<StrymonPreset>()), Times.Once()); 
        }

        [TestMethod]
        public void NotSendSysExForFetchCurrentWhenNotConnected()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 2);
            Sut.FetchCurrent();
            // Assert
            var data = TimelineEditBufferFetchData;
            midiOutMock.Verify(x => x.SendSysEx(data), Times.Never());
        }

        [TestMethod]
        public void NotSendSysExForFetchCurrentWhenBulkFetching()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            Sut.BulkPedal = Sut.ConnectedPedals.First();
            Sut.FetchCurrent();
            // Assert
            var data = TimelineEditBufferFetchData;
            midiOutMock.Verify(x => x.SendSysEx(data), Times.Never());
        }

        [TestMethod]
        public void SendSysExForFetchByIndexWhenConnected()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            Sut.FetchByIndex(0);
            Sut.FetchByIndex(199); 
            Sut.FetchByIndex(299); // Out of bounds on Timeline, need some tightening up
            // Assert
            // Bloody enums in the MIDI library suck, need to get them out
            // Call One
            midiOutMock.Verify(x => x.SendProgramChange(It.IsAny<Channel>(), Instrument.AcousticGrandPiano), Times.Once()); // Preset 0
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), 0, 0), Times.Once()); // Bank 0
            var data = TimelineFetchRequest(0x00, 0x00);
            midiOutMock.Verify(x => x.SendSysEx(data), Times.Once());
            // Call Two
            midiOutMock.Verify(x => x.SendProgramChange(It.IsAny<Channel>(), Instrument.Clarinet), Times.Once()); // Preset 199
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), 0, 1), Times.Once()); // Bank 1
            var lastPreset = TimelineFetchRequest(0x01, 0x47);
            midiOutMock.Verify(x => x.SendSysEx(lastPreset), Times.Once());
            // Call Three
            midiOutMock.Verify(x => x.SendProgramChange(It.IsAny<Channel>(), Instrument.Contrabass), Times.Once()); // Preset 299
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), 0, 2), Times.Once()); // Bank 2
            var lastBigSkyPreset = TimelineFetchRequest(0x02, 0x2B);
            midiOutMock.Verify(x => x.SendSysEx(lastBigSkyPreset), Times.Once());
        }


        [TestMethod]
        public void SendSysExForFetchByIndexWhenBulkFetching()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            var mediatorMock = Container.GetMock<IMediator>();
            Sut.Mediator = mediatorMock.Object;
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            Sut.BulkPedal = Sut.ConnectedPedals.First();
            Sut.FetchByIndex(1);
            var presetData = new StrymonSysExUtils.StrymonSysExMessage(Sut.ConnectedPedals.First()).FullMessageData;
            midiInMock.Raise(x => x.SysEx += null, new SysExMessage(new DeviceBase("dummy"),
                               presetData, new float()));

            // Assert
            var data = TimelineFetchRequest(0x00, 0x01);
            midiOutMock.Verify(x => x.SendSysEx(data), Times.Once());
            mediatorMock.Verify(x => x.NotifyColleagues(ViewModelMessages.BulkPresetRead, It.IsAny<StrymonPreset>()), Times.Once());
        }


        [TestMethod]        
        public void HandleSysExForFetchByIndexWhenBadResponse()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            var mediatorMock = Container.GetMock<IMediator>();
            Sut.Mediator = mediatorMock.Object;
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            Sut.BulkPedal = Sut.ConnectedPedals.First();
            Sut.FetchByIndex(1);

            var presetData = new StrymonSysExUtils.StrymonSysExMessage(Sut.ConnectedPedals.First()).FullMessageData;
            midiInMock.Raise(x => x.SysEx += null, new SysExMessage(new DeviceBase("dummy"),
                               Enumerable.Repeat((byte)0x20, 8).ToArray(), new float()));

            // Assert
            
        }


        [TestMethod]
        public void NotSendSysExForFetchByIndexWhenNotConnected()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 2);
            Sut.FetchByIndex(0);
            // Assert
            var data = TimelineFetchRequest(0x00, 0x00);
            midiOutMock.Verify(x => x.SendSysEx(data), Times.Never());
            midiOutMock.Verify(x => x.SendProgramChange(It.IsAny<Channel>(), It.IsAny<Instrument>()), Times.Never()); // Preset 299
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never()); // Bank 2
        }

        [TestMethod]
        public void NotSendSysExForFetchByIndexWhenPedalMaster()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 2);
            Sut.SyncMode = SyncMode.PedalMaster;
            Sut.FetchByIndex(0);
            // Assert
            var data = TimelineFetchRequest(0x00, 0x00);
            midiOutMock.Verify(x => x.SendSysEx(data), Times.Never());
            midiOutMock.Verify(x => x.SendProgramChange(It.IsAny<Channel>(), It.IsAny<Instrument>()), Times.Never()); // Preset 299
            midiOutMock.Verify(x => x.SendControlChange(It.IsAny<Channel>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never()); // Bank 2
        }

        #endregion

        #region Push Tests
        [TestMethod]
        public void PushToEditWhenConnected()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            // Bit convoluted
            var presetData =new StrymonSysExUtils.StrymonSysExMessage(Sut.ConnectedPedals.First()).FullMessageData;
            var preset = StrymonSysExUtils.FromSysExData(presetData);
            SetPresetIndex(presetData, 0x01, 0x48);
            Sut.PushToEdit(preset);
            midiOutMock.Verify(x => x.SendSysEx(presetData), Times.Once()); 
        }

        [TestMethod]
        public void NotPushToEditWhenNotConnected()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 2);
            // Bit convoluted
            var presetData = new StrymonSysExUtils.StrymonSysExMessage(Sut.ConnectedPedals.First()).FullMessageData;
            var preset = StrymonSysExUtils.FromSysExData(presetData);
            SetPresetIndex(presetData, 0x01, 0x48);
            Sut.PushToEdit(preset);
            midiOutMock.Verify(x => x.SendSysEx(presetData), Times.Never());
        }

        [TestMethod]
        public void NotPushToEditWhenBulkFetching()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            // Bit convoluted
            var presetData = new StrymonSysExUtils.StrymonSysExMessage(Sut.ConnectedPedals.First()).FullMessageData;
            var preset = StrymonSysExUtils.FromSysExData(presetData);
            SetPresetIndex(presetData, 0x01, 0x48);
            Sut.BulkPedal = Sut.ConnectedPedals.First();
            Sut.PushToEdit(preset);
            midiOutMock.Verify(x => x.SendSysEx(presetData), Times.Never());
        }

        [TestMethod]
        public void PushToIndexWhenConnected()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            // Bit convoluted
            var presetData = new StrymonSysExUtils.StrymonSysExMessage(Sut.ConnectedPedals.First()).FullMessageData;
            var preset = StrymonSysExUtils.FromSysExData(presetData);
            SetPresetIndex(presetData, 0x00, 0x00);
            Sut.PushToIndex(preset,0);
            midiOutMock.Verify(x => x.SendSysEx(presetData), Times.Once());
        }

        [TestMethod]
        public void NotPushToIndexWhenNotConnected()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 2);
            // Bit convoluted
            var presetData = new StrymonSysExUtils.StrymonSysExMessage(Sut.ConnectedPedals.First()).FullMessageData;
            var preset = StrymonSysExUtils.FromSysExData(presetData);
            SetPresetIndex(presetData, 0x00, 0x00);
            Sut.PushToIndex(preset, 0);
            midiOutMock.Verify(x => x.SendSysEx(presetData), Times.Never());
        }

        [TestMethod]
        public void NotPushToIndexWhenBulkFetching()
        {
            // Arrange 
            var midiInMock = Container.GetMock<IInputDevice>();
            var midiOutMock = Container.GetMock<IOutputDevice>();
            // Act
            MidiSetupLevel(midiInMock, midiOutMock, 3);
            // Bit convoluted
            var presetData = new StrymonSysExUtils.StrymonSysExMessage(Sut.ConnectedPedals.First()).FullMessageData;
            var preset = StrymonSysExUtils.FromSysExData(presetData);
            SetPresetIndex(presetData, 0x00, 0x00);
            Sut.BulkPedal = Sut.ConnectedPedals.First();
            Sut.PushToIndex(preset, 0);
            midiOutMock.Verify(x => x.SendSysEx(presetData), Times.Never());
        }
        #endregion

        /// <summary>
        /// Helper method to setup the Sut at various levels of 'completeness'      
        /// </summary>
        private void MidiSetupLevel( Mock<IInputDevice> midiInMock, Mock<IOutputDevice> midiOutMock, int level, byte zeroByte = 0x00, byte pedalId = 0x01)
        {
            // Level 0 - No initialisation at all
            if (level == 0) return;
            // Level 1 - Only InitMidi called - no callbacks received or ContextPedal set
            Sut.InitMidi();
            if (level == 1) return;
            // Level 1 - InitMidi called and valid IdentityCallback received
            midiInMock.Raise(x => x.SysEx += null, new SysExMessage(new DeviceBase("dummy"),
                                            new byte[] { 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x55, 0x12, zeroByte, pedalId, 0xF7 }, new float()));
            if (level == 2) return;
            // Level 3 - Full 'happy path' setup, InitMidi called, IdentityCallback received and matching ContextPedal setup
            Sut.ContextPedal = Sut.ConnectedPedals.First();
        }

        private byte[] TimelineEditBufferFetchData
        {
            get
            {
                return TimelineFetchRequest(0x01,0x48);
            }
        }
        private byte[] TimelineFetchRequest(byte presetSlot1, byte presetSlot2)
        {
            return new byte[] { 0xF0, 0x00, 0x01, 0x55, 0x12, 0x01, 0x63, presetSlot1, presetSlot2, 0xF7 };
        }
        private void SetPresetIndex(byte[] data, byte presetSlot1, byte presetSlot2)
        {
            data[7] = presetSlot1;
            data[8] = presetSlot2;
        }
    }
}
