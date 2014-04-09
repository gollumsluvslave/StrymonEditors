using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RITS.StrymonEditor;
using RITS.StrymonEditor.Conversion;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;
namespace RITS.StrymonEditor.Tests
{
    [TestClass]
    public class PotValueConverterFactoryShould : TestBase
    {

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
        }

        Parameter testParameter = new Parameter
        {
            Definition = new ParameterDef { Name = "Test", Range = new Range { MinValue = 0, MaxValue = 10 } }
        };
        Pot testPot = new Pot
        {
            Id=2
        };
        [TestMethod]
        public void ReturnDefaultPotValueConverterCorrectly()
        {
            var retval = PotValueConverterFactory.Create(testParameter,testPot);
            Assert.IsTrue(retval is DefaultPotValueConverter);
        }
        [TestMethod]
        public void ReturnOptionListPotValueConverterCorrectly()
        {
            testParameter.Definition.Range = null;
            testParameter.Definition.OptionList = new List<Option>();
            testParameter.Definition.OptionList.Add(new Option{Name="TestOption", Value=0});
            var retval = PotValueConverterFactory.Create(testParameter, testPot);
            Assert.IsTrue(retval is OptionListPotValueConverter);
        }
    }
}
