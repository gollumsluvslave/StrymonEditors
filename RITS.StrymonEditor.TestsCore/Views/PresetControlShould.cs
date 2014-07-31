using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Text;
using System.Threading.Tasks;
using RITS.StrymonEditor.MIDI;
using RITS.StrymonEditor.Views;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace RITS.StrymonEditor.Tests
{
    [TestClass()]
    public class PresetControlShould:TestContext<PresetControl>
    {
        [TestMethod]
        public void IncrementPresetCorrectly()
        {
            var mockMidi = Container.GetMock<IStrymonMidiManager>();
            var vm = new PresetControlViewModel("Fetch", TestHelper.TimelinePedal, mockMidi.Object);

            Sut.DataContext = vm;
            var privSut = new PrivateObject(Sut);
            privSut.Invoke("Button_Click_1", new object[] { null, null });

            Assert.AreEqual(1, vm.PresetIndex);
        }

        [TestMethod]
        public void DecrementPresetCorrectly()
        {
            var mockMidi = Container.GetMock<IStrymonMidiManager>();
            var vm = new PresetControlViewModel("Fetch", TestHelper.TimelinePedal, mockMidi.Object);

            Sut.DataContext = vm;
            var privSut = new PrivateObject(Sut);
            privSut.Invoke("Button_Click", new object[] { null, null });

            Assert.AreEqual(199, vm.PresetIndex);
        }

        [TestMethod]
        public void GetAndSetDependencyPropertiesCorrectly()
        {
            Sut.PresetIsEnabled = false;
            Assert.IsFalse(Sut.PresetIsEnabled);
            Sut.PresetName = "TestPreset";
            Assert.AreEqual("TestPreset", Sut.PresetName);
            Sut.Mode = "Fetch";
            Assert.AreEqual("Fetch", Sut.Mode);
            
        }
    }

    
}
