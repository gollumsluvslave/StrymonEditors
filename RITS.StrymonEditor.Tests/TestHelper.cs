using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RITS.StrymonEditor.Models;
namespace RITS.StrymonEditor.Tests
{
    public static class TestHelper
    {
        public static StrymonPedal TimelinePedal
        {
            get
            {
                return Globals.SupportedPedals.First(x => x.Name == StrymonPedal.Timeline_Name);
            }
        }

        public static StrymonPedal MobiusPedal
        {
            get
            {
                return Globals.SupportedPedals.First(x => x.Name == StrymonPedal.Mobius_Name);
            }
        }

        public static StrymonPedal BigSkyPedal
        {
            get
            {
                return Globals.SupportedPedals.First(x => x.Name == StrymonPedal.BigSky_Name);
            }
        }

        public static StrymonPreset TestTimelinePreset
        {
            get
            {
                return StrymonSysExUtils.FromSysExData(new StrymonSysExUtils.StrymonSysExMessage(TimelinePedal).FullMessageData);
            }
        }
        public static StrymonPreset NewTimelinePreset
        {
            get
            {
                return new StrymonPreset(TimelinePedal, true);
            }
        }

        public static StrymonPreset TestMobiusPreset
        {
            get
            {
                return StrymonSysExUtils.FromSysExData(new StrymonSysExUtils.StrymonSysExMessage(MobiusPedal).FullMessageData);
            }
        }
        public static StrymonPreset NewMobiusPreset
        {
            get
            {
                return new StrymonPreset(MobiusPedal, true);
            }
        }
        public static StrymonPreset TestBigSkyPreset
        {
            get
            {
                return StrymonSysExUtils.FromSysExData(new StrymonSysExUtils.StrymonSysExMessage(BigSkyPedal).FullMessageData);
            }
        }
        public static StrymonPreset NewBigSkyPreset
        {
            get
            {
                return new StrymonPreset(BigSkyPedal, true);
            }
        }

        
    }
}
