using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RITS.StrymonEditor;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;
namespace RITS.StrymonEditor.Tests
{
    [TestClass]
    public class PotViewModelFactoryShould : TestBase
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
        Pot testPot = new Pot
        {
            Id = 2
        };
        [TestMethod]
        public void ReturnDefaultViewModelForStandardPot()
        {
            var retval = PotViewModelFactory.Create(testPot,testParameter);
            Assert.IsTrue(retval is PotViewModel);
            Assert.IsFalse(retval is DynamicPotViewModel);
            Assert.IsFalse(retval is CoarsePotViewModel);
            Assert.IsFalse(retval is FinePotViewModel);
        }
        
        [TestMethod]
        public void ReturnFineViewModelForFinePot()
        {
            testPot.Id = 0;
            var retval = PotViewModelFactory.Create(testPot, testParameter);
            Assert.IsTrue(retval is FinePotViewModel);
        }
        
        [TestMethod]
        public void ReturnCoarseViewModelForCoarsePot()
        {
            testPot.Id = 1;
            var retval = PotViewModelFactory.Create(testPot, testParameter);
            Assert.IsTrue(retval is CoarsePotViewModel);
        }
        
        [TestMethod]
        public void ReturnDynamicViewModelForDynamicPot()
        {
            testPot.IsDynamic = true;
            var retval = PotViewModelFactory.Create(testPot, testParameter);
            Assert.IsTrue(retval is DynamicPotViewModel);
        }
    }
}
