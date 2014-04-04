using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RITS.StrymonEditor.Models;
namespace RITS.StrymonEditor.Tests.Models
{
    [TestClass]
    public class StymonSysExUtilsShould:TestBase
    {
        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
        }
        [TestMethod]
        public void ReturnCorrectTimelinePresetFromBase()
        {
            var pedal = Globals.SupportedPedals.FirstOrDefault(x=>x.Name == StrymonPedal.Timeline_Name);
            var sysEx = new StrymonSysExUtils.StrymonSysExMessage(pedal);
            var preset = StrymonSysExUtils.FromSysExData(sysEx.FullMessageData);

            Assert.AreEqual(StrymonPedal.Timeline_Name, preset.Pedal.Name);
            Assert.AreEqual("BASE", preset.Name);
            Assert.AreEqual(7, preset.Machine.Value);
            Assert.AreEqual("dTape", preset.Machine.Name);
            var timeParam = AssertParamAndReturn(preset, 0, "Time");
            Assert.AreEqual(30, timeParam.FineValue);
            AssertParamAndReturn(preset, 0, "Mix");
            AssertParamAndReturn(preset, 0, "Repeats");
            AssertParamAndReturn(preset, 0, "Filter");
            AssertParamAndReturn(preset, 0, "Grit");
            AssertParamAndReturn(preset, 0, "Speed");
            AssertParamAndReturn(preset, 0, "Depth");
        }

        private Parameter AssertParamAndReturn(StrymonPreset preset, int expectedValue, string name)
        {
            var param = preset.ControlParameters.FirstOrDefault(x => x.Name == name);
            Assert.IsNotNull(param);
            Assert.AreEqual(expectedValue, param.Value);
            return param;
        }

        [TestMethod]
        public void ReturnCorrectMobiusPresetFromBase()
        {
            var pedal = Globals.SupportedPedals.FirstOrDefault(x => x.Name == StrymonPedal.Mobius_Name);
            var sysEx = new StrymonSysExUtils.StrymonSysExMessage(pedal);
            var preset = StrymonSysExUtils.FromSysExData(sysEx.FullMessageData);

            Assert.AreEqual(StrymonPedal.Mobius_Name, preset.Pedal.Name);
            Assert.AreEqual("CALL THE COPS", preset.Name);
            Assert.AreEqual(7, preset.Machine.Value);
            Assert.AreEqual("Chorus", preset.Machine.Name);
            var speedParam = AssertParamAndReturn(preset, 0, "Speed");
            Assert.AreEqual(449, speedParam.FineValue);
            AssertParamAndReturn(preset, 0, "Depth");
            AssertParamAndReturn(preset, 0, "Level");
        }

        [TestMethod]
        public void ReturnCorrectBigSkyPresetFromBase()
        {
            var pedal = Globals.SupportedPedals.FirstOrDefault(x => x.Name == StrymonPedal.BigSky_Name);
            var sysEx = new StrymonSysExUtils.StrymonSysExMessage(pedal);
            var preset = StrymonSysExUtils.FromSysExData(sysEx.FullMessageData);

            Assert.AreEqual(StrymonPedal.BigSky_Name, preset.Pedal.Name);
            Assert.AreEqual("LIL RINGY", preset.Name);
            Assert.AreEqual(9, preset.Machine.Value);
            Assert.AreEqual("Plate", preset.Machine.Name);
            var speedParam = AssertParamAndReturn(preset, 0, "Decay");
            Assert.AreEqual(16387, speedParam.FineValue);
            AssertParamAndReturn(preset, 0, "Pre-Delay");
            AssertParamAndReturn(preset, 0, "Mix");
        }

        // Need to add more specific preset tests
        // EPSet
        // Various hidden parameters
        // Dynamic Params
        // Hidden pots etc
    }
}
