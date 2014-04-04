using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;
namespace RITS.StrymonEditor.Tests
{

    public class TestBase
    {
        public virtual void Setup()
        {
            Globals.Init();
            //Globals.ActivePreset = new StrymonPreset(Globals.SupportedPedals[2], true);
        }
    }

    [TestClass]
    public class ParameterViewModelShould : TestBase
    {
        Parameter testParameter = new Parameter
        {
            Definition = new ParameterDef { Name = "Test", Range = new Range { MinValue = 0, MaxValue = 10 } }
        };
        ParameterViewModel sut;
        
        [TestInitialize]
        public override void Setup() 
        {
            base.Setup();
            sut = new ParameterViewModel(testParameter);
        }

        [TestMethod]
        public void WrapParameterCorrectly()
        {
            Assert.AreEqual(testParameter.Name,sut.Name);
            Assert.IsNotNull(sut.Definition);
            Assert.IsNotNull(sut.Definition.Range);
        }

        [TestMethod]
        public void ReturnCorrectRange()
        {
            Assert.AreEqual(0,sut.Definition.Range.MinValue);
            Assert.AreEqual(10, sut.Definition.Range.MaxValue);            
        }

        [TestMethod]
        public void ReturnDefaultValue()
        {
            Assert.AreEqual(0, sut.Value);            
        }

        [TestMethod]
        public void AssignPotCorrectly()
        {
            var potVM = new PotViewModel(new Pot { Id = 1 }, testParameter);
            sut.AssignToDynamicPot(potVM);
            Assert.IsNotNull(sut.LinkedPot);
            Assert.AreEqual(potVM.Value, sut.Value);
        }
    }
}
