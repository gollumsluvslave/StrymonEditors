using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RITS.StrymonEditor;
using RITS.StrymonEditor.Conversion;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;
namespace RITS.StrymonEditor.Tests
{
    [TestClass]
    public class FineCoarseValueConverterFactoryShould : TestBase
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
        [TestMethod]
        public void ReturnDefualtFineValueLabelConverterCorrectly()
        {
            var retval = FineCoarseValueConverterFactory.Create(testParameter.Definition);
            Assert.IsTrue(retval is DefaultFineCoarseValueConverter);
        }
    }
}
