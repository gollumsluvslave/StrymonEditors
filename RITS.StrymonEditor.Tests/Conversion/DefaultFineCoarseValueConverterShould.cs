using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RITS.StrymonEditor;
using RITS.StrymonEditor.Conversion;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;
namespace RITS.StrymonEditor.Tests
{
    [TestClass]
    public class DefaultFineCoarseValueConverterShould : TestContext<DefaultFineCoarseValueConverter>
    {

        [TestMethod]
        public void RespectRangeBoundaries()
        {
            // Arrange
            var paramDef = TestHelper.TimelinePedal.ControlParameters.FirstOrDefault(x => x.PotId == 1);
            var min = paramDef.FineRange.MinValue;
            var max = paramDef.FineRange.MaxValue;
            Container.Register<ParameterDef>(paramDef);
            // Act
            var retval = Sut.FineToFine(min-10);
            Assert.AreEqual(min, retval);
            retval = Sut.FineToFine(max + 10);
            Assert.AreEqual(max, retval);
        }

        [TestMethod]
        public void ReturnCorrectCoarseToFineForBoundaries()
        {
            // Arrange
            var paramDef = TestHelper.TimelinePedal.ControlParameters.FirstOrDefault(x => x.PotId == 1);
            var fineMin = paramDef.FineRange.MinValue;
            var fineMax = paramDef.FineRange.MaxValue;
            var coarseMin = paramDef.Range.MinValue;
            var coarseMax = paramDef.Range.MaxValue;
            Container.Register<ParameterDef>(paramDef);
            // Act
            var retval = Sut.CoarseToFine(coarseMin);
            Assert.AreEqual(fineMin, retval);
            retval = Sut.CoarseToFine(coarseMax);
            Assert.AreEqual(fineMax, retval);
        }

        [TestMethod]
        public void ReturnCorrectFineToCoarseForBoundaries()
        {
            // Arrange
            var paramDef = TestHelper.TimelinePedal.ControlParameters.FirstOrDefault(x => x.PotId == 1);
            var fineMin = paramDef.FineRange.MinValue;
            var fineMax = paramDef.FineRange.MaxValue;
            var coarseMin = paramDef.Range.MinValue;
            var coarseMax = paramDef.Range.MaxValue;
            Container.Register<ParameterDef>(paramDef);
            // Act
            var retval = Sut.FineToCoarse(fineMin);
            Assert.AreEqual(coarseMin, retval);
            retval = Sut.FineToCoarse(fineMax);
            Assert.AreEqual(coarseMax, retval);
        }

        [TestMethod]
        public void ReturnCorrectCoarseToFineForBoundaryBreach()
        {
            // Arrange
            var paramDef = TestHelper.TimelinePedal.ControlParameters.FirstOrDefault(x => x.PotId == 1);
            var fineMin = paramDef.FineRange.MinValue;
            var fineMax = paramDef.FineRange.MaxValue;
            var coarseMin = paramDef.Range.MinValue;
            var coarseMax = paramDef.Range.MaxValue;
            Container.Register<ParameterDef>(paramDef);
            // Act
            var retval = Sut.CoarseToFine(coarseMin - 10);
            Assert.AreEqual(fineMin, retval);
            retval = Sut.CoarseToFine(coarseMax + 10);
            Assert.AreEqual(fineMax, retval);
        }

        [TestMethod]
        public void ReturnCorrectFineToCoarseForBoundaryBreach()
        {
            // Arrange
            var paramDef = TestHelper.TimelinePedal.ControlParameters.FirstOrDefault(x => x.PotId == 1);
            var fineMin = paramDef.FineRange.MinValue;
            var fineMax = paramDef.FineRange.MaxValue;
            var coarseMin = paramDef.Range.MinValue;
            var coarseMax = paramDef.Range.MaxValue;
            Container.Register<ParameterDef>(paramDef);
            // Act
            var retval = Sut.FineToCoarse(fineMin - 10);
            Assert.AreEqual(coarseMin, retval);
            retval = Sut.FineToCoarse(fineMax + 10);
            Assert.AreEqual(coarseMax, retval);
        }
    }
}
