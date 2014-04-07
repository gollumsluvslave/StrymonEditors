using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        [TestMethod]
        public void HandlePedalConnected()
        {
            Sut.Mediator.NotifyColleagues(ViewModelMessages.PedalConnected, TestHelper.TimelinePedal);            
            Assert.AreEqual("Timeline", Sut.ConnectedPedal1);
        }

    }
}
