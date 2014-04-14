using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;

namespace RITS.StrymonEditor.Tests
{
    [TestClass]
    public class StrymonPedalShould : TestContext<StrymonPedal>
    {

        [TestMethod]
        public void ReturnCorrectFineIncrementValue_Timeline()
        {
            // Arrange
            Sut.Name=StrymonPedal.Timeline_Name;
            // Act
            var inc = Sut.FineIncrement(500);
            // Assert - Timeline should always return 1 millisecond
            Assert.AreEqual(1, inc);
        }

        [TestMethod]
        public void ReturnCorrectFineIncrementValue_Mobius()
        {
            // Arrange
            Sut.Name = StrymonPedal.Mobius_Name;
            // Act
            var inc = Sut.FineIncrement(500);
            // Assert - Mobius should always return 10 millihz
            Assert.AreEqual(10, inc);
        }

        [TestMethod]
        public void ReturnCorrectFineIncrementValue_BigSky()
        {
            // Arrange
            Sut.Name = StrymonPedal.BigSky_Name;
            // Act
            var inc = Sut.FineIncrement(500);
            // Assert - less than 1000 = 1
            Assert.AreEqual(1, inc);
            inc = Sut.FineIncrement(1500);
            // Assert - less than 10000 = 10
            Assert.AreEqual(10, inc);
            inc = Sut.FineIncrement(15000);
            // Assert - less than 10000 = 100
            Assert.AreEqual(100, inc);
        }

    }
}
