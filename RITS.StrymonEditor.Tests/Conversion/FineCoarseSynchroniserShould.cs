using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RITS.StrymonEditor;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;

namespace RITS.StrymonEditor.Tests
{
    /// <summary>
    /// Simple test to ensure Fine/Coarse parameter synchronisation is working
    /// </summary>
    [TestClass]
    public class FineCoarseSynchroniserShould:TestBase
    {
        private FineCoarseSynchroniser sut;
        Parameter testParameter = new Parameter
        {
            Definition = new ParameterDef { Name = "Time", 
                                            Range = new Range { MinValue = 0, MaxValue = 127 }, 
                                            FineRange = new Range { MinValue = 60, MaxValue = 2500 } 
                                          },
            Value=0,
            FineValue=60

        };
        Pot testFinePot = new Pot
        {
            Id = 0
        };
        Pot testCoarsePot = new Pot
        {
            Id = 1
        };

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            sut = new FineCoarseSynchroniser(PotViewModelFactory.Create(testCoarsePot, testParameter), PotViewModelFactory.Create(testFinePot, testParameter));
            // SUT needs to be assigned to Global to provide sync
            Globals.FineCoarseSynchroniser = sut;
            // Need an IncrementMap to be applied
            var incList = new List<Increment>();
            incList.Add(new Increment { Value = "10" });
            Globals.PotValueMap = Globals.SupportedPedals.First().PotValueMap;
            Globals.PotValueMap.ApplyFineValueIncrementMap(incList, testParameter.Definition);
        }

        [TestMethod]
        public void SetCoarseValueCorrectly()
        {
            var fineValue = testParameter.FineValue;
            sut.SetCoarseValue(50);
            Assert.AreEqual(50, testParameter.Value);
            Assert.AreNotEqual(fineValue, testParameter.FineValue);
        }
        
        
        [TestMethod]
        public void SetFineValueCorrectly()
        {
            var coarseValue = testParameter.Value;
            sut.SetFineValue(1000);
            Assert.AreEqual(1000, testParameter.FineValue);
            Assert.AreNotEqual(coarseValue, testParameter.Value);
        }
    }
}
