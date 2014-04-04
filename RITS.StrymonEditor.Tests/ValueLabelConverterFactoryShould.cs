using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RITS.StrymonEditor;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;
namespace RITS.StrymonEditor.Tests
{
    [TestClass]
    public class ValueLabelConverterFactoryShould : TestBase
    {

        [TestInitialize]
        public override void  Setup()
        {
            base.Setup();
        }

        Parameter testParameter = new Parameter
        {
            Definition = new ParameterDef { Name = "Test", Range = new Range { MinValue = 0, MaxValue = 10 } }
        };
        [TestMethod]
        public void ReturnEchoValueLabelConverterCorrectly()
        {
            var retval = ValueLabelConverterFactory.Create(testParameter);
            Assert.IsTrue(retval is EchoValueLabelConverter);
        }
        [TestMethod]
        public void ReturnBoostValueLabelConverterCorrectly()
        {
            testParameter.Definition.Name = "Boost";
            var retval = ValueLabelConverterFactory.Create(testParameter);
            Assert.IsTrue(retval is BoostValueLabelConverter);
        }
        [TestMethod]
        public void ReturnFineValueLabelConverterCorrectly()
        {
            testParameter.Definition.Name = "Test";
            testParameter.Definition.PotId = 1;
            var retval = ValueLabelConverterFactory.Create(testParameter);
            Assert.IsTrue(retval is FineValueLabelConverter);
        }
    }
}
