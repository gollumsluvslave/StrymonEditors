using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RITS.StrymonEditor;
using RITS.StrymonEditor.Conversion;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;
namespace RITS.StrymonEditor.Tests
{
    [TestClass]
    public class OptionListPotValueConverterShould : TestContext<OptionListPotValueConverter>
    {

        [TestMethod]
        public void RespectMaxAngleToValue()
        {
            // Arrange
            Container.Register<int>(12); // register max
            // Act
            var retval = Sut.AngleToValue(300);
            Assert.AreEqual(12, retval);
        }

        [TestMethod]
        public void RespectMaxValueToAngle()
        {
            // Arrange
            Container.Register<int>(12); // register max
            // Act
            var retval = Sut.ValueToAngle(14);
            Assert.AreEqual(290, retval);
        }

        [TestMethod]
        public void ReturnZeroForNegativeData()
        {
            // Arrange
            Container.Register<int>(12); // register max
            // Act
            var retval = Sut.ValueToAngle(-14);
            Assert.AreEqual(0, retval);
            retval = Sut.AngleToValue(-14);
            Assert.AreEqual(0, retval);
        }
    }
}
