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
    public class DefaultPotValueConverterShould : TestContext<DefaultPotValueConverter>
    {

        [TestMethod]
        public void RespectAngleToValueRangeBoundaries()
        {
            // Arrange
            Container.Register<int>(127);
            // Act
            var retval = Sut.AngleToValue(300);
            Assert.AreEqual(127, retval);

            retval = Sut.AngleToValue(-20);
            Assert.AreEqual(0, retval);
        }
        
        [TestMethod]
        public void RespectValueToAngleRangeBoundaries()
        {
            // Arrange
            Container.Register<int>(127);
            // Act
            var retval = Sut.ValueToAngle(300);
            Assert.AreEqual(290, retval);

            retval = Sut.AngleToValue(-20);
            Assert.AreEqual(0, retval);
        }
    }
}
