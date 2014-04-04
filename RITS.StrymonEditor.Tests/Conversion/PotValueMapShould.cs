using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RITS.StrymonEditor;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;

namespace RITS.StrymonEditor.Tests
{
    [TestClass]
    public class PotValueMapShould:TestBase
    {
        Parameter testParameter = new Parameter
        {
            Definition = new ParameterDef
            {
                Name = "Time",
                Range = new Range { MinValue = 0, MaxValue = 127 },
                FineRange = new Range { MinValue = 60, MaxValue = 2500 }
            },
            Value = 0,
            FineValue = 60

        };
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
            var incList = new List<Increment>();
            incList.Add(new Increment { Value = "10" });
            Globals.PotValueMap = Globals.SupportedPedals.First().PotValueMap;
            Globals.PotValueMap.ApplyFineValueIncrementMap(incList, testParameter.Definition);

        }

        [TestMethod]
        public void Have128ValuesInLookup()
        {
            Assert.AreEqual(128, Globals.PotValueMap.LookupMap.Count);
        }
    }
}
