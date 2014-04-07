using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
        }

        [TestMethod]
        public void SetLCDValueCorrectly()
        {
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            Sut.ShouldNotifyOn(x => x.LCDValue).
                    When(x => x.LCDValue = "Test");
            Assert.AreEqual("Test", Sut.LCDValue);
        }
        [TestMethod]
        public void SetIsDirtyCorrectly()
        {
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            // NB this is making multiple property sets here
            Sut.ShouldNotifyOn(x => x.IsDirty).
                    When(x => x.IsDirty=false);
            Sut.ShouldNotifyOn(x => x.Title).
                    When(x => x.IsDirty = false);
            Assert.IsFalse(Sut.IsDirty);
        }


        [TestMethod]
        public void RenameCorrect()
        {
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);

            Sut.Mediator.NotifyColleagues(ViewModelMessages.PresetRenamed, "CHANGED");

            Assert.AreEqual("CHANGED",Sut.ActivePreset.Name);
            Assert.IsTrue(Sut.IsDirty);
        }

        [TestMethod]
        public void SetActivePresetCorrectly()
        {
            // Arrange & Act
            var setAction = new Action<StrymonPedalViewModel>(x => x.ActivePreset = TestHelper.TestTimelinePreset);
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            // NB this is making multiple property sets here
            Sut.ShouldNotifyOn(x => x.ActivePreset).
                    When(setAction);
            Sut.ShouldNotifyOn(x => x.ActivePedal).
                    When(setAction);
            Sut.ShouldNotifyOn(x => x.Image).
                    When(setAction);

            // Assert
            Assert.AreEqual(TestHelper.TestTimelinePreset.Machine.Value, Sut.ActiveMachine.Value);

        }

        [TestMethod]
        public void SetActivePresetCorrectlyWithNewPreset()
        {
            // Arrange & Act
            var setAction = new Action<StrymonPedalViewModel>(x => x.ActivePreset = TestHelper.NewTimelinePreset);
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            // NB this is making multiple property sets here
            Sut.ShouldNotifyOn(x => x.ActivePreset).
                    When(setAction);
            Sut.ShouldNotifyOn(x => x.ActivePedal).
                    When(setAction);
            Sut.ShouldNotifyOn(x => x.Image).
                    When(setAction);

            // Assert
            Assert.AreEqual("NEW", Sut.ActivePreset.Name);

        }

        [TestMethod]
        public void SetActivePresetMobiusCorrectly()
        {
            // Arrange & Act
            var preset = TestHelper.TestMobiusPreset;
            var setAction = new Action<StrymonPedalViewModel>(x => x.ActivePreset = preset);
            Container.Register<StrymonPreset>(preset);
            // NB this is making multiple property sets here
            Sut.ShouldNotifyOn(x => x.ActivePreset).
                    When(setAction);

            // Assert
            Assert.AreEqual(preset.Machine.Value, Sut.ActiveMachine.Value);

        }

        [TestMethod]
        public void SetActiveMachineCorrectly()
        {
            // Arrange & Act
            var setAction = new Action<StrymonPedalViewModel>(x => x.ActiveMachine = new StrymonMachineViewModel(TestHelper.TestTimelinePreset.Machine));
            var midiManagerMock = Container.GetMock<IStrymonMidiManager>();
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            Sut.ShouldNotifyOn(x => x.ActiveMachine).
                    When(setAction);

            Assert.AreEqual(TestHelper.TestTimelinePreset.Machine.Value, Sut.ActiveMachine.Value);
            midiManagerMock.Verify(x => x.SynchMachine(TestHelper.TestTimelinePreset.Machine));
        }

        [TestMethod]
        public void ReturnCorrectMenus()
        {
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);

            Assert.AreEqual(5, Sut.EditorMenu.Count);
            Assert.AreEqual(2, Sut.LCDMenu.Count);
        }


        [TestMethod]
        public void ExecuteEPSetWizardInSuccessionSuccessfully()
        {
            // Arrange
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            // Act & Assert
            Sut.EPSetCommand.Execute(null);
            Assert.AreEqual("EP SET : Setting Heel...", Sut.EPSetMode);
            Sut.EPSetCommand.Execute(null);
            Assert.AreEqual("EP SET : Setting Toe...", Sut.EPSetMode);
            Sut.EPSetCommand.Execute(null);
            Assert.AreEqual(null, Sut.EPSetMode);
        }

        [TestMethod]
        public void PreviewEPSetHeelSuccessfully()
        {
            // Arrange
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            // Act & Assert
            Sut.PreviewEPSetHeelCommand.Execute(null);
            Assert.AreEqual("EP SET : Previewing Heel...", Sut.EPSetMode);
            Sut.PreviewEPSetHeelCommand.Execute(null);
            Assert.AreEqual(null, Sut.EPSetMode);            
        }

        [TestMethod]
        public void PreviewEPSetToeSuccessfully()
        {
            // Arrange
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            // Act & Assert
            Sut.PreviewEPSetToeCommand.Execute(null);
            Assert.AreEqual("EP SET : Previewing Toe...", Sut.EPSetMode);
            Sut.PreviewEPSetToeCommand.Execute(null);
            Assert.AreEqual(null, Sut.EPSetMode);
        }

        [TestMethod]
        public void ChangeSyncModeSuccessfully()
        {
            // Arrange
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var midiManagerMock = Container.GetMock<IStrymonMidiManager>();
            // Act 
            // Simulate check of menu (PedalMaster) - bit brittle having to get data from Sut but not sure how else to manage these kind of tests
            var syncModeVm = Sut.EditorMenu[3].Children[0].Children[1];
            syncModeVm.IsChecked = true;
            Sut.SyncModeChanged.Execute(syncModeVm);
            // Assert
            Assert.IsFalse(syncModeVm.IsEnabled);
            Assert.AreEqual(SyncMode.EditorMaster, Sut.SyncMode);
            midiManagerMock.VerifySet(x => x.SyncMode=SyncMode.EditorMaster);
        }

        [TestMethod]
        public void HandleReceiveCCChangedOk()
        {
            // Arrange
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var EPParamViewModel = Sut.HiddenParameters.FirstOrDefault(x => x.Name == "EP");
            Assert.AreEqual(0, EPParamViewModel.Value);
            // Act 
            // Simulate EP On
            // How to raise EP Set on?? Just use the mediator - no mock, real one
            Sut.Mediator.NotifyColleagues(ViewModelMessages.ReceivedCC, new ControlChangeMsg { ControlChangeNo = 60, Value = 1 });            
            // Assert
            Assert.AreEqual(1, EPParamViewModel.Value);
        }

        [TestMethod]
        public void HandleFineParameterChangedWithRangeTrigger()
        {
            var preset = TestHelper.TestTimelinePreset;
            Container.Register<StrymonPreset>(TestHelper.TestTimelinePreset);
            var param = preset.HiddenParameters.First(x => x.Name == "TapeSpeed");
            var oldFineRange = preset.ControlParameters.First().Definition.FineRange;
            param.Value = 1;
            Sut.Mediator.NotifyColleagues(ViewModelMessages.ParameterChanged, param);
            var newFineRange = preset.ControlParameters.First().Definition.FineRange;
            Assert.AreNotEqual(oldFineRange.MinValue, newFineRange.MinValue);
            Assert.AreNotEqual(oldFineRange.MaxValue, newFineRange.MaxValue);
        }


    }
}
