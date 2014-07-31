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
    public class BoostValueLabelConverterShould : TestContext<BoostValueLabelConverter>
    {

        [TestMethod]
        public void RespectBoostBoundaries()
        {
            // Arrange
            var minBoost = "-3.0 db";
            var maxBoost = "+3.0 db";
            // Act
            var retval = Sut.ValueToLabel(-10);
            Assert.AreEqual(minBoost, retval);
            retval = Sut.ValueToLabel(120);
            Assert.AreEqual(maxBoost, retval);
            retval = Sut.ValueToLabel(0);
            Assert.AreEqual(minBoost, retval);
            retval = Sut.ValueToLabel(60);
            Assert.AreEqual(maxBoost, retval);
        }

        [TestMethod]
        public void ReturnCorrectValue()
        {
            // Act
            var retval = Sut.ValueToLabel(30);
            Assert.AreEqual("0.0 db", retval);
        }

    }
}
