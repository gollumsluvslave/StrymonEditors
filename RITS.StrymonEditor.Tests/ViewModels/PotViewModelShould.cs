using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;
namespace RITS.StrymonEditor.Tests
{


    [TestClass]
    public class PotViewModelShould : TestBase
    {
        Parameter testParameter = new Parameter
        {
            Definition = new ParameterDef { Name = "Test", Range = new Range { MinValue = 0, MaxValue = 10 } }
        };
        Pot testPot = new Pot
        {
            Id = 2
        };

        PotViewModel sut;
        
        [TestInitialize]
        public override void Setup() 
        {
            base.Setup();
            sut = PotViewModelFactory.Create(testPot, testParameter);
        }

        [TestMethod]
        public void WrapParameterCorrectly()
        {
            Assert.AreEqual(testParameter.Value,sut.Value);
            Assert.IsNotNull(sut.LinkedParameter);
            Assert.AreEqual("Test 0", sut.ValueLabel);
        }


        [TestMethod]
        public void ReturnNullLabelAsDefault()
        {
            Assert.IsNull(sut.Label);
        }

        [TestMethod]
        public void ReturnCorrectLabelIfOverridden()
        {
            sut.Label = "TestOverride";
            Assert.AreEqual("TestOverride",sut.Label);
        }

        [TestMethod]
        public void AcceptSimpleParameterChangeCorrectly()
        {
            var newParam = new Parameter
            {
                Definition = new ParameterDef { Name = "New", Range = new Range { MinValue = 0, MaxValue = 100 } },
                Value=20
            };
            sut.LinkedParameter = newParam;
            Assert.AreEqual(20,sut.Value);
            Assert.IsNull(sut.Label); // Label should still be null as it is not explicitly set 
            Assert.AreEqual("New 20", sut.ValueLabel);
        }

    }
}
