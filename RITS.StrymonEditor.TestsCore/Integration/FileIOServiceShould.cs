using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Moq;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace RITS.StrymonEditor.Tests.Integration
{
    [TestClass]
    public class FileIOServiceShould: TestContext<FileIOService>
    {
        [TestCategory("Integration")]
        [TestMethod]
        public void LoadSyxFromFileSystemSuccessfully()
        {
            var mockFileDialog = Container.GetMock<IFileDialog>();
            mockFileDialog.SetupProperty(x => x.FileName, @"Integration\Data\Base_Timeline.syx");
            mockFileDialog.Setup(x => x.ShowDialog()).Returns(true);
            var preset=Sut.LoadSyxPreset();
            mockFileDialog.Verify(x => x.ShowDialog(), Times.Once());
            Assert.IsNotNull(preset);
            Assert.AreEqual("BASE", preset.Name);
        }
        
        [TestCategory("Integration")]
        [TestMethod]
        public void LoadXmlFromFileSystemSuccessfully()
        {
            var mockFileDialog = Container.GetMock<IFileDialog>();
            mockFileDialog.SetupProperty(x => x.FileName, @"Integration\Data\Black Metallic.xml");
            mockFileDialog.Setup(x => x.ShowDialog()).Returns(true);
            var preset = Sut.LoadXmlPreset();
            mockFileDialog.Verify(x => x.ShowDialog(), Times.Once());
            Assert.IsNotNull(preset);
            Assert.AreEqual("BLACK METALLIC", preset.Name);
        }

        [TestCategory("Integration")]
        [TestMethod]
        public void SaveToSyxSuccessfully()
        {
            var mockFileDialog = Container.GetMock<IFileDialog>();
            var preset = TestHelper.TestTimelinePreset;
            var file = "SaveTest.syx";
            if (File.Exists(file)) File.Delete(file);
            Assert.IsFalse(File.Exists(file));
            mockFileDialog.SetupProperty(x => x.FileName, file);
            mockFileDialog.Setup(x => x.ShowDialog()).Returns(true);
            Sut.SavePresetToSyx(preset);
            mockFileDialog.Verify(x => x.ShowDialog(), Times.Once());
            Assert.IsTrue(File.Exists(file));
        }

        [TestCategory("Integration")]
        [TestMethod]
        public void SaveToXmlSuccessfully()
        {
            var mockFileDialog = Container.GetMock<IFileDialog>();
            var preset = TestHelper.TestTimelinePreset;
            var file = "SaveTest.xml";
            if (File.Exists(file)) File.Delete(file);
            Assert.IsFalse(File.Exists(file));
            mockFileDialog.SetupProperty(x => x.FileName, file);
            mockFileDialog.Setup(x => x.ShowDialog()).Returns(true);
            Sut.SavePresetToXml(preset);
            mockFileDialog.Verify(x => x.ShowDialog(), Times.Once());
            Assert.IsTrue(File.Exists(file));
            
        }
    }
}
