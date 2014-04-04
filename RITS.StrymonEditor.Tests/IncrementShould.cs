using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;

namespace RITS.StrymonEditor.Tests
{
    [TestClass]
    public class IncrementShould : TestBase
    {

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
        }

        [TestMethod]
        public void ReturnCorrectIncrementForValue()
        {
            Increment inc = new Increment { Value = "1", End = 10 };
            int index=0;
            var result = inc.GetIncrementValue(ref index);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ReturnCorrectIncrementsForAlternatingValues()
        {
            Increment inc = new Increment { Value = "1,2", End = 10 };
            int index = 0;
            var result = inc.GetIncrementValue(ref index);
            Assert.AreEqual(1, result);
            index++;
            result = inc.GetIncrementValue(ref index);
            Assert.AreEqual(2, result);
            index++;
            result = inc.GetIncrementValue(ref index);
            Assert.AreEqual(1, result);
            index++;
            result = inc.GetIncrementValue(ref index);
            Assert.AreEqual(2, result);
        }
    }
}
