using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;

namespace RITS.StrymonEditor.Tests
{
    [TestClass]
    public class StrymonPresetShould : TestContext<StrymonPreset>
    {

        [TestMethod]
        public void ReturnCorrectFineValue_Mobius_BPM()
        {
            // Arrange
            Sut = TestHelper.TestMobiusPreset;
            Globals.IsBPMModeActive = true;            
            // Act
            var inc = Sut.FineValue;
            // Assert - 449 millihz in Mobius preset, should be 26.9 bpm
            Assert.AreEqual("26.9", inc);
            Globals.IsBPMModeActive = false;
        }


        [TestMethod]
        public void ReturnCorrectFineValue_Mobius()
        {
            // Arrange
            Sut = TestHelper.TestMobiusPreset;
            // Act
            var inc = Sut.FineValue;
            // Assert - 449 millihz in Mobius preset
            Assert.AreEqual("449", inc);
        }

    }
}
