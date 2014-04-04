using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            Assert.AreEqual(60, Globals.ConvertMillisecondsToBPM(1000));
        }
        
        [TestMethod]
        public void ConvertDoubleMillisecondsCorrectly()
        {
            Assert.AreEqual(30, Globals.ConvertMillisecondsToBPM(2000));
        }

        [TestMethod]
        public void ConvertHalfMillisecondsCorrectly()
        {
            Assert.AreEqual(120, Globals.ConvertMillisecondsToBPM(500));
        }      

        [TestMethod]
        public void ConvertSimpleHerzCorrectly()
        {
            Assert.AreEqual(60, Globals.ConvertMilliHzToBPM(1000));
        }
        [TestMethod]
        public void Convert3HerzCorrectly()
        {
            Assert.AreEqual(180, Globals.ConvertMilliHzToBPM(3000));
        }
    }
}
