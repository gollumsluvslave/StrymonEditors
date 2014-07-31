using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RITS.StrymonEditor.Conversion;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;

namespace RITS.StrymonEditor.Tests
{
    [TestClass]
    public class BPMConversionShould:TestBase
    {

        [TestInitialize]
        public override void Setup() 
        {
            base.Setup();
        }

        [TestMethod]
        public void ConvertSimpleMillisecondsCorrectly()
        {
            Assert.AreEqual(60, ConversionUtils.ConvertMillisecondsToBPM(1000));
        }
        
        [TestMethod]
        public void ConvertDoubleMillisecondsCorrectly()
        {
            Assert.AreEqual(30, ConversionUtils.ConvertMillisecondsToBPM(2000));
        }

        [TestMethod]
        public void ConvertHalfMillisecondsCorrectly()
        {
            Assert.AreEqual(120, ConversionUtils.ConvertMillisecondsToBPM(500));
        }      

        [TestMethod]
        public void ConvertSimpleHerzCorrectly()
        {
            Assert.AreEqual(60, ConversionUtils.ConvertMilliHzToBPM(1000));
        }
        [TestMethod]
        public void Convert3HerzCorrectly()
        {
            Assert.AreEqual(180, ConversionUtils.ConvertMilliHzToBPM(3000));
        }

        [TestMethod]
        public void ConvertBPMToMillisecondsCorrectly()
        {
            Assert.AreEqual(500, ConversionUtils.ConvertBPMToMilliseconds(120));
        }
        [TestMethod]
        public void ConvertBPMToMilliHzCorrectly()
        {
            Assert.AreEqual(3000, ConversionUtils.ConvertBPMToMilliHz(180));
        }
    }
}
