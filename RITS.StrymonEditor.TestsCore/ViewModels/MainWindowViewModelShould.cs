using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using RITS.StrymonEditor.MIDI;
using RITS.StrymonEditor.IO;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.Views;
using RITS.StrymonEditor.ViewModels;
using RITS.StrymonEditor.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace RITS.StrymonEditor.Tests
{
    [TestClass()]
    public class MainWindowViewModelShould:TestContext<MainWindowViewModel>
    {

        [TestMethod]
        public void DisposeGracefully()
        {
            Sut.Dispose();
        }

        [TestMethod]
        public void ReturnCorrectMenus()
        {
            Assert.AreEqual(3, Sut.EditorMenu.Count);
            Sut.Dispose();
        }
        
        [TestMethod]
        public void HandleNullPresetRead()
        {
            Sut.PBMax = 100;
            Sut.PBStatus = null;
            Sut.PBValue = -1;
            Sut.Mediator.NotifyColleagues(ViewModelMessages.BulkPresetRead, null);
            Assert.AreEqual(0, Sut.PBValue);
            Assert.AreEqual("Fetch Failed : 0", Sut.PBStatus);
            Sut.Dispose();
        }
        [TestMethod]
        public void HandlePresetRead()
        {
            Sut.PBMax = 100;
            Sut.PBStatus = null;
            Sut.PBValue = -1;
            Sut.Mediator.NotifyColleagues(ViewModelMessages.BulkPresetRead, TestHelper.TestTimelinePreset);
            Assert.AreEqual(0, Sut.PBValue);
            Assert.AreEqual("Fetched : Timeline(199)", Sut.PBStatus);
            Sut.Dispose();
        }

        [TestMethod]
        public void HandlePedalConnected()
        {
            Sut.Mediator.NotifyColleagues(ViewModelMessages.PedalConnected, TestHelper.TimelinePedal);            
            Assert.AreEqual("Timeline", Sut.ConnectedPedal1);
            Sut.Dispose();
        }

        [TestMethod]
        public void HandleMIDIConenctionComplete()
        {
            var midiMock = Container.GetMock<IStrymonMidiManager>();
            midiMock.Setup(x=>x.ConnectedPedals).Returns(new StrymonPedal[]{TestHelper.TimelinePedal}.ToList());
            Sut.Mediator.NotifyColleagues(ViewModelMessages.MIDIConnectionComplete, TestHelper.TimelinePedal);
            midiMock.Verify(x => x.FetchByIndex(It.IsAny<int>()), Times.Exactly(200));
            Sut.Dispose();
        }

        [TestMethod]
        public void ExecuteNewTimelineCommand()
        {
            // Arrange
            var editorMock=Container.GetMock<IModalDialog>();
            Sut.EditorWindow = editorMock.Object;
            // Act  
            Sut.NewTimelinePresetCommand.Execute(TestHelper.TimelinePedal); 
            //Assert
            editorMock.Verify(x => x.ShowModal(), Times.Once());
            Sut.Dispose();
        }

        [TestMethod]
        public void ExecuteNewMobiusCommand()
        {
            // Arrange
            var editorMock = Container.GetMock<IModalDialog>();
            Sut.EditorWindow = editorMock.Object;
            // Act
            Sut.NewTimelinePresetCommand.Execute(TestHelper.MobiusPedal);
            //Assert
            editorMock.Verify(x => x.ShowModal(), Times.Once());
            Sut.Dispose();
        }

        [TestMethod]
        public void ExecuteNewBigSkyCommand()
        {
            // Arrange
            var editorMock = Container.GetMock<IModalDialog>();
            Sut.EditorWindow = editorMock.Object;
            // Act
            Sut.NewTimelinePresetCommand.Execute(TestHelper.BigSkyPedal); // Tests for null?
            //Assert
            editorMock.Verify(x => x.ShowModal(), Times.Once());
            Sut.Dispose();
        }

        [TestMethod]
        public void ExecuteLoadSyxCommand()
        {
            // Arrange
            var editorMock = Container.GetMock<IModalDialog>();
            var ioMock = Container.GetMock<IFileIOService>();
            ioMock.Setup(x => x.LoadSyxPreset()).Returns(TestHelper.TestMobiusPreset);
            Sut.EditorWindow = editorMock.Object;
            Sut.FileIOService = ioMock.Object;
            // Act
            Sut.LoadSyxCommand.Execute(null); 
            //Assert
            ioMock.Verify(x => x.LoadSyxPreset(), Times.Once());
            editorMock.Verify(x => x.ShowModal(), Times.Once());
            
            Sut.Dispose();
        }

        [TestMethod]
        public void ExecuteLoadXmlCommand()
        {
            // Arrange
            var editorMock = Container.GetMock<IModalDialog>();
            var ioMock = Container.GetMock<IFileIOService>();
            ioMock.Setup(x => x.LoadXmlPreset()).Returns(TestHelper.TestMobiusPreset);
            Sut.EditorWindow = editorMock.Object;
            Sut.FileIOService = ioMock.Object;
            // Act
            Sut.LoadXmlCommand.Execute(null); 
            //Assert
            ioMock.Verify(x => x.LoadXmlPreset(), Times.Once());
            editorMock.Verify(x => x.ShowModal(), Times.Once());

            Sut.Dispose();
        }

    }
}
