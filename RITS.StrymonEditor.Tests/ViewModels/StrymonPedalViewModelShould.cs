using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RITS.StrymonEditor.Views;
using RITS.StrymonEditor.IO;
using RITS.StrymonEditor.Messaging;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;
namespace RITS.StrymonEditor.Tests
{
    [TestClass()]
    public class StrymonPedalViewModelShould:TestContext<StrymonPedalViewModel>
    {
        

        [TestMethod]
        public void DisposeGracefully()
        {
            // Arrange
            var midiManagerMock = Container.GetMock<IStrymonMidiManager>();
            var mediatorMock = Container.GetMock<IMediator>();
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            Sut.Mediator = mediatorMock.Object;            
            // Act
            Sut.Dispose();
            // Assert
            midiManagerMock.VerifySet(x=>x.ContextPedal=null, Times.Once());
            midiManagerMock.Verify(x => x.Dispose(), Times.Never());
            mediatorMock.Verify(x => x.UnRegister(It.IsAny<ViewModelMessages>(), It.IsAny<Action<object>>()), Times.Exactly(11));
            Sut.Dispose();
        }

        [TestMethod]
        public void SetLCDValueCorrectly()
        {
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            Sut.ShouldNotifyOn(x => x.LCDValue).
                    When(x => x.LCDValue = "Test");
            Assert.AreEqual("Test", Sut.LCDValue);
            Sut.Dispose();
        }
        [TestMethod]
        public void SetIsDirtyCorrectly()
        {
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            // NB this is making multiple property sets here
            Sut.ShouldNotifyOn(x => x.IsDirty).
                    When(x => x.IsDirty=false);
            Sut.ShouldNotifyOn(x => x.Title).
                    When(x => x.IsDirty = false);
            Assert.IsFalse(Sut.IsDirty);
            Sut.Dispose();
        }


        [TestMethod]
        public void RenameCorrect()
        {
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;

            Sut.Mediator.NotifyColleagues(ViewModelMessages.PresetRenamed, "CHANGED");

            Assert.AreEqual("CHANGED",Sut.ActivePreset.Name);
            Assert.IsTrue(Sut.IsDirty);
            Sut.Dispose();
        }

        [TestMethod]
        public void SetActivePresetCorrectly()
        {
            // Arrange & Act
            var setAction = new Action<StrymonPedalViewModel>(x => x.ActivePreset = TestHelper.TestTimelinePreset);
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            // NB this is making multiple property sets here
            Sut.ShouldNotifyOn(x => x.ActivePreset).
                    When(setAction);
            Sut.ShouldNotifyOn(x => x.ActivePedal).
                    When(setAction);
            Sut.ShouldNotifyOn(x => x.Image).
                    When(setAction);

            // Assert
            Assert.AreEqual(TestHelper.TestTimelinePreset.Machine.Value, Sut.ActiveMachine.Value);
            Sut.Dispose();

        }

        [TestMethod]
        public void SetActivePresetCorrectlyWithNewPreset()
        {
            // Arrange & Act
            var setAction = new Action<StrymonPedalViewModel>(x => x.ActivePreset = TestHelper.NewTimelinePreset);
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            // NB this is making multiple property sets here
            Sut.ShouldNotifyOn(x => x.ActivePreset).
                    When(setAction);
            Sut.ShouldNotifyOn(x => x.ActivePedal).
                    When(setAction);
            Sut.ShouldNotifyOn(x => x.Image).
                    When(setAction);

            // Assert
            Assert.AreEqual("NEW", Sut.ActivePreset.Name);
            Sut.Dispose();

        }

        [TestMethod]
        public void SetActivePresetMobiusCorrectly()
        {
            // Arrange & Act
            var preset = TestHelper.TestMobiusPreset;
            var setAction = new Action<StrymonPedalViewModel>(x => x.ActivePreset = preset);
            Container.Register<StrymonPreset>(preset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            // NB this is making multiple property sets here
            Sut.ShouldNotifyOn(x => x.ActivePreset).
                    When(setAction);
            Sut.PotControls.First();
            // Assert
            Assert.AreEqual(preset.Machine.Value, Sut.ActiveMachine.Value);
            Sut.Dispose();

        }

        [TestMethod]
        public void SetActiveMachineCorrectly()
        {
            // Arrange & Act
            var setAction = new Action<StrymonPedalViewModel>(x => x.ActiveMachine = new StrymonMachineViewModel(TestHelper.TestTimelinePreset.Machine));
            var midiManagerMock = Container.GetMock<IStrymonMidiManager>();
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            Sut.ShouldNotifyOn(x => x.ActiveMachine).
                    When(setAction);

            Assert.AreEqual(TestHelper.TestTimelinePreset.Machine.Value, Sut.ActiveMachine.Value);
            midiManagerMock.Verify(x => x.SynchMachine(TestHelper.TestTimelinePreset.Machine));
            Sut.Dispose();
        }

        [TestMethod]
        public void ChangeActiveMachineCorrectly()
        {
            // Arrange & Act
            var newMachine = TestHelper.TimelinePedal.Machines.FirstOrDefault(x=>x.Name=="Digital");
            var setAction = new Action<StrymonPedalViewModel>(x => x.ActiveMachine = new StrymonMachineViewModel(newMachine));
            var midiManagerMock = Container.GetMock<IStrymonMidiManager>();
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            Sut.ShouldNotifyOn(x => x.ActiveMachine).
                    When(setAction);

            Assert.AreEqual(newMachine.Value, Sut.ActiveMachine.Value);
            midiManagerMock.Verify(x => x.SynchMachine(newMachine));
            Sut.Dispose();
        }


        [TestMethod]
        public void ReturnCorrectMenus()
        {
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;

            Assert.AreEqual(5, Sut.EditorMenu.Count);
            Assert.AreEqual(2, Sut.LCDMenu.Count);
        }


        [TestMethod]
        public void ExecuteEPSetWizardInSuccessionSuccessfully()
        {
            // Arrange
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            // Act & Assert
            Sut.EPSetCommand.Execute(null);
            Assert.AreEqual("EP SET : Setting Heel...", Sut.EPSetMode);
            Sut.EPSetCommand.Execute(null);
            Assert.AreEqual("EP SET : Setting Toe...", Sut.EPSetMode);
            Sut.EPSetCommand.Execute(null);
            Assert.AreEqual(null, Sut.EPSetMode);
            Sut.Dispose();
        }

        [TestMethod]
        public void PreviewEPSetHeelSuccessfully()
        {
            // Arrange
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            // Act & Assert
            Sut.PreviewEPSetHeelCommand.Execute(null);
            Assert.AreEqual("EP SET : Previewing Heel...", Sut.EPSetMode);
            Sut.PreviewEPSetHeelCommand.Execute(null);
            Assert.AreEqual(null, Sut.EPSetMode);
            Sut.Dispose();
        }

        [TestMethod]
        public void PreviewEPSetToeSuccessfully()
        {
            // Arrange
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            // Act & Assert
            Sut.PreviewEPSetToeCommand.Execute(null);
            Assert.AreEqual("EP SET : Previewing Toe...", Sut.EPSetMode);
            Sut.PreviewEPSetToeCommand.Execute(null);
            Assert.AreEqual(null, Sut.EPSetMode);
            Sut.Dispose();
        }

        [TestMethod]
        public void ChangeSyncModeSuccessfully()
        {
            // Arrange
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var midiManagerMock = Container.GetMock<IStrymonMidiManager>();
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            // Act 
            // Simulate check of menu (PedalMaster) - bit brittle having to get data from Sut but not sure how else to manage these kind of tests
            var syncModeVm = Sut.EditorMenu[3].Children[0].Children[1];
            syncModeVm.IsChecked = true;
            Sut.SyncModeChanged.Execute(syncModeVm);
            // Assert
            Assert.IsFalse(syncModeVm.IsEnabled);
            Assert.AreEqual(SyncMode.EditorMaster, Sut.SyncMode);
            midiManagerMock.VerifySet(x => x.SyncMode=SyncMode.EditorMaster);
            Sut.Dispose();
        }

        [TestMethod]
        public void HandleReceiveCCChangedOk()
        {
            // Arrange
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            var EPParamViewModel = Sut.HiddenParameters.FirstOrDefault(x => x.Name == "EP");
            Assert.AreEqual(0, EPParamViewModel.Value);
            // Act 
            // Simulate EP On
            // How to raise EP Set on?? Just use the mediator - no mock, real one
            Sut.Mediator.NotifyColleagues(ViewModelMessages.ReceivedCC, new ControlChangeMsg { ControlChangeNo = 60, Value = 1 });            
            // Assert
            Assert.AreEqual(1, EPParamViewModel.Value);
            Sut.Dispose();
        }

        [TestMethod]
        public void HandleFineParameterChangedWithRangeTrigger()
        {
            var preset = TestHelper.TestTimelinePreset;
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var param = preset.HiddenParameters.First(x => x.Name == "TapeSpeed");
            var oldFineRange = preset.ControlParameters.First().Definition.FineRange;
            param.Value = 1;
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            Sut.Mediator.NotifyColleagues(ViewModelMessages.ParameterChanged, param);
            var newFineRange = preset.ControlParameters.First().Definition.FineRange;
            Assert.AreNotEqual(oldFineRange.MinValue, newFineRange.MinValue);
            Assert.AreNotEqual(oldFineRange.MaxValue, newFineRange.MaxValue);
            Sut.Dispose();
        }

        [TestMethod]
        public void ExecuteLoadSyxCommandWithNoCorrectly()
        {
            // Arrange
            var dialogMock = Container.GetMock<IMessageDialog>();
            dialogMock.Setup(x => x.ShowYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            var fileMock = Container.GetMock<IFileIOService>();
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            Sut.FileIOService = fileMock.Object;
            Sut.MessageDialog = dialogMock.Object;
            // Act
            Sut.LoadSyxCommand.Execute(null);
            // Assert
            dialogMock.Verify(x => x.ShowYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            fileMock.Verify(x => x.LoadSyxPreset(), Times.Once());
            Assert.IsNotNull(Sut.ActivePreset);
            Sut.Dispose();
        }


        [TestMethod]
        public void ExecuteLoadSyxCommandWithNoCorrectlyWhenDirty()
        {
            // Arrange
            var dialogMock = Container.GetMock<IMessageDialog>();
            dialogMock.Setup(x=>x.ShowYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            var fileMock = Container.GetMock<IFileIOService>();
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            Sut.FileIOService = fileMock.Object;
            Sut.MessageDialog = dialogMock.Object;
            Sut.ActivePreset.Name = "MAKEDIRTY";
            // Act
            Sut.LoadSyxCommand.Execute(null);
            // Assert
            dialogMock.Verify(x => x.ShowYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            fileMock.Verify(x => x.LoadSyxPreset(), Times.Never());
            Sut.Dispose();
        }

        [TestMethod]
        public void ExecuteLoadSyxCommandWithYesCorrectlyWhenDirty()
        {
            // Arrange
            var preset = TestHelper.TestTimelinePreset;
            preset.Name="CHANGED";
            var dialogMock = Container.GetMock<IMessageDialog>();
            dialogMock.Setup(x => x.ShowYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            var fileMock = Container.GetMock<IFileIOService>();
            fileMock.Setup(x => x.LoadSyxPreset()).Returns(preset);
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            Sut.FileIOService = fileMock.Object;
            Sut.MessageDialog = dialogMock.Object;
            Sut.ActivePreset.Name = "MAKEDIRTY";
            // Act
            Sut.LoadSyxCommand.Execute(null);
            // Assert
            dialogMock.Verify(x => x.ShowYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            fileMock.Verify(x => x.LoadSyxPreset(), Times.Once());
            // Updated the property            
            Assert.AreEqual("CHANGED", Sut.ActivePreset.Name);
            Sut.Dispose();
        }

        [TestMethod]
        public void ExecuteLoadXmlCommandWithNoCorrectly()
        {
            // Arrange
            var dialogMock = Container.GetMock<IMessageDialog>();
            dialogMock.Setup(x => x.ShowYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            var fileMock = Container.GetMock<IFileIOService>();
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            Sut.FileIOService = fileMock.Object;
            Sut.MessageDialog = dialogMock.Object;
            // Act
            Sut.LoadXmlCommand.Execute(null);
            // Assert
            dialogMock.Verify(x => x.ShowYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            fileMock.Verify(x => x.LoadXmlPreset(), Times.Once());
            Assert.IsNotNull(Sut.ActivePreset);
            Sut.Dispose();
        }


        [TestMethod]
        public void ExecuteLoadXmlCommandWithNoCorrectlyWhenDirty()
        {
            // Arrange
            var dialogMock = Container.GetMock<IMessageDialog>();
            dialogMock.Setup(x => x.ShowYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            var fileMock = Container.GetMock<IFileIOService>();
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            Sut.FileIOService = fileMock.Object;
            Sut.MessageDialog = dialogMock.Object;
            Sut.ActivePreset.Name = "MAKEDIRTY";
            // Act
            Sut.LoadXmlCommand.Execute(null);
            // Assert
            dialogMock.Verify(x => x.ShowYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            fileMock.Verify(x => x.LoadXmlPreset(), Times.Never());
            Sut.Dispose();
        }

        [TestMethod]
        public void ExecuteLoadXmlCommandWithYesCorrectlyWhenDirty()
        {
            // Arrange
            var preset = TestHelper.TestTimelinePreset;
            preset.Name = "CHANGED";
            var dialogMock = Container.GetMock<IMessageDialog>();
            dialogMock.Setup(x => x.ShowYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            var fileMock = Container.GetMock<IFileIOService>();
            fileMock.Setup(x => x.LoadXmlPreset()).Returns(preset);
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            Sut.FileIOService = fileMock.Object;
            Sut.MessageDialog = dialogMock.Object;
            Sut.ActivePreset.Name = "MAKEDIRTY";
            // Act
            Sut.LoadXmlCommand.Execute(null);
            // Assert
            dialogMock.Verify(x => x.ShowYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            fileMock.Verify(x => x.LoadXmlPreset(), Times.Once());
            // Updated the property            
            Assert.AreEqual("CHANGED", Sut.ActivePreset.Name);
            Sut.Dispose();
        }

        [TestMethod]
        public void SetBPMModeCorrectly() 
        {
            // Arrange
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            var bpmMode = Sut.EditorMenu[1].Children[0];
            bpmMode.IsChecked = true;
            // Act
            Sut.BPMModeCommand.Execute(bpmMode);
            // Assert
            Assert.IsTrue(Globals.IsBPMModeActive);
            Sut.Dispose();
        }

        [TestMethod]
        public void UnSetBPMModeCorrectly()
        {
            // Arrange
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            var bpmMode = Sut.EditorMenu[1].Children[0];
            bpmMode.IsChecked = false;
            // Act
            Sut.BPMModeCommand.Execute(bpmMode);
            // Assert
            Assert.IsTrue(!Globals.IsBPMModeActive);
            Sut.Dispose();
        }

        [TestMethod]
        public void SetBPMModeCorrectlyViaKeyCommand()
        {
            // Arrange
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            // Act
            Sut.BPMModeCommand.Execute(null);
            // Assert
            Assert.IsTrue(Globals.IsBPMModeActive);
            Sut.Dispose();
        }

        [TestMethod]
        public void UnSetBPMModeCorrectlyViaKeyCommand()
        {
            // Arrange
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            // simulate set to true
            var bpmMode = Sut.EditorMenu[1].Children[0];
            bpmMode.IsChecked = true;
            // Act
            Sut.BPMModeCommand.Execute(null);
            // Assert
            Assert.IsTrue(!Globals.IsBPMModeActive);
            Sut.Dispose();
        }

        [TestMethod]
        public void ShowEditedStatusInTitleWhenDirty()
        {
            // Arrange
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            //
            Sut.ActivePreset.Name = "MAKEDIRTY";
            //
            Assert.IsTrue(Sut.Title.Contains("*EDITED*"));
            Sut.Dispose();
        }
        [TestMethod]
        public void NotShowEditedStatusInTitleWhenNotDirty()
        {
            // Arrange
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            //
            Assert.IsFalse(Sut.Title.Contains("*EDITED*"));
            Sut.Dispose();
        }

        [TestMethod]
        public void ExecuteRenameCommand()
        {
            // Arrange
            var mockDialog = Container.GetMock<IInputDialog>();
            
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            Sut.RenameDialog = mockDialog.Object;
            // Act
            Sut.RenameCommand.Execute(null);
            // Assert
            mockDialog.Verify(x => x.ShowModal(), Times.Once());
            Sut.Dispose();
        }

        [TestMethod]
        public void ExecuteDirectEntryCommand()
        {
            // Arrange
            var mockDialog = Container.GetMock<IInputDialog>();

            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            Sut.DirectEntryDialog = mockDialog.Object;
            // Act
            Sut.DirectFineEntryCommand.Execute(null);
            // Assert
            mockDialog.Verify(x => x.ShowModal(), Times.Once());
            Sut.Dispose();
        }

        [TestMethod]
        public void ExecuteLooperCommands()
        {
            // Arrange
            var midiManagerMock = Container.GetMock<IStrymonMidiManager>();
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            // Act
            Sut.LoopPlay.Execute(null);
            Sut.LoopRecord.Execute(null);
            Sut.LoopStop.Execute(null);
            Sut.LoopUndo.Execute(null);
            Sut.LoopRedo.Execute(null);
            Sut.LoopFullHalf.Execute(null);
            Sut.LoopReverse.Execute(null);
            Sut.LoopPrePost.Execute(null);
            // Assert
            midiManagerMock.Verify(x => x.SendLooperPlay(), Times.Once());
            midiManagerMock.Verify(x => x.SendLooperRecord(), Times.Once());
            midiManagerMock.Verify(x => x.SendLooperStop(), Times.Once());
            midiManagerMock.Verify(x => x.SendLooperUndo(), Times.Once());
            midiManagerMock.Verify(x => x.SendLooperRedo(), Times.Once());
            midiManagerMock.Verify(x => x.SendLooperPrePost(), Times.Once());
            midiManagerMock.Verify(x => x.SendLooperReverse(), Times.Once());
            midiManagerMock.Verify(x => x.SendLooperFullHalf(), Times.Once());
            Sut.Dispose();
        }

        [TestMethod]
        public void ExecuteExpressionPedalCommand()
        {
            // Arrange
            var midiManagerMock = Container.GetMock<IStrymonMidiManager>();
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            // Act
            Sut.VirtualExpressionPedalValue = 10;
            // Assert
            midiManagerMock.Verify(x => x.SendVirtualEP(10), Times.Once());
            Sut.Dispose();
        }
        
        [TestMethod]
        public void ExecuteInifiniteHoldCommand()
        {
            // Arrange
            var midiManagerMock = Container.GetMock<IStrymonMidiManager>();
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            // Act
            Sut.ToggleInfinite.Execute(null);
            Sut.ToggleInfinite.Execute(null);
            // Assert
            midiManagerMock.Verify(x => x.SendInfinite(127), Times.Once());
            midiManagerMock.Verify(x => x.SendInfinite(0), Times.Once());
            Sut.Dispose();
        }

        [TestMethod]
        public void HandleDirectEntryMessage()
        {

            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var dialogMock = Container.GetMock<IMessageDialog>();
            Sut.MessageDialog = dialogMock.Object;
            // Need to force some get behaviour here - WPF would execute these gets after onpropertychanged events
            Sut.HiddenParameters.First();
            Sut.PotControls.First();
            // Act
            Sut.Mediator.NotifyColleagues(ViewModelMessages.DirectEntryValueEntered, 100);
            // Assert
            Assert.AreEqual(100, Sut.Encoder.LinkedParameter.FineValue);
            Sut.Dispose();

        }

        [TestMethod]
        public void HandleFetchPresetRequestMessageWhenDirty()
        {

            var midiManagerMock = Container.GetMock<IStrymonMidiManager>();
            var mockDialog = Container.GetMock<IMessageDialog>();
            mockDialog.Setup(x => x.ShowYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            Sut.MessageDialog = mockDialog.Object;
            Sut.ActivePreset.Name = "MAKEDIRTY";
            // Act
            Sut.Mediator.NotifyColleagues(ViewModelMessages.FetchPresetRequested, 1);
            // Assert
            mockDialog.Verify(x => x.ShowYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            midiManagerMock.Verify(x => x.FetchByIndex(1));
            Sut.Dispose();

        }

        
        [TestMethod]
        public void HandlePushPresetRequestMessageWithYesResponse()
        {

            var midiManagerMock = Container.GetMock<IStrymonMidiManager>();
            var mockDialog = Container.GetMock<IMessageDialog>();
            mockDialog.Setup(x => x.ShowYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            Sut.MessageDialog = mockDialog.Object;
            // Act
            Sut.Mediator.NotifyColleagues(ViewModelMessages.PushPresetRequested, 1);
            // Assert
            mockDialog.Verify(x => x.ShowYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            midiManagerMock.Verify(x => x.PushToIndex(It.IsAny<StrymonPreset>(),1), Times.Once());
            Sut.Dispose();
        }
        
        [TestMethod]
        public void HandlePushPresetRequestMessageWithNoResponse()
        {

            var midiManagerMock = Container.GetMock<IStrymonMidiManager>();
            var mockDialog = Container.GetMock<IMessageDialog>();
            mockDialog.Setup(x => x.ShowYesNo(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            Sut.MessageDialog = mockDialog.Object;
            // Act
            Sut.Mediator.NotifyColleagues(ViewModelMessages.PushPresetRequested, 1);
            // Assert
            mockDialog.Verify(x => x.ShowYesNo(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            midiManagerMock.Verify(x => x.PushToIndex(TestHelper.TestTimelinePreset, 1), Times.Never());
            Sut.Dispose();
        }

        [TestMethod]
        public void ExecuteSaveSyxCommandCorrectly()
        {
            // Arrange
            var mockFileService = Container.GetMock<IFileIOService>();
            mockFileService.Setup(x => x.SavePresetToSyx(It.IsAny<StrymonPreset>())).Returns(true);
            var fileMock = Container.GetMock<IFileIOService>();
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            Sut.FileIOService = mockFileService.Object;
            Sut.ActivePreset.Name = "MAKEDIRTY";
            // Act
            Sut.SaveSyxCommand.Execute(null);
            // Assert
            mockFileService.Verify(x => x.SavePresetToSyx(It.IsAny<StrymonPreset>()), Times.Once());
            Assert.IsFalse(Sut.IsDirty);
            Sut.Dispose();
        }

        [TestMethod]
        public void ExecuteSaveXmlCommandCorrectly()
        {
            // Arrange
            var mockFileService = Container.GetMock<IFileIOService>();
            mockFileService.Setup(x => x.SavePresetToXml(It.IsAny<StrymonPreset>())).Returns(true);
            var fileMock = Container.GetMock<IFileIOService>();
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            Sut.FileIOService = mockFileService.Object;
            Sut.ActivePreset.Name = "MAKEDIRTY";
            // Act
            Sut.SaveXmlCommand.Execute(null);
            // Assert
            mockFileService.Verify(x => x.SavePresetToXml(It.IsAny<StrymonPreset>()), Times.Once());
            Assert.IsFalse(Sut.IsDirty);
            Sut.Dispose();
        }
    }
}
