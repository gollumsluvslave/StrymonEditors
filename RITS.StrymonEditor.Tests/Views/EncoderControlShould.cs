using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using RITS.StrymonEditor.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RITS.StrymonEditor.Models;
using RITS.StrymonEditor.ViewModels;
namespace RITS.StrymonEditor.Tests
{
    [TestClass]
    public class EncoderControlShould:TestContext<EncoderControl>
    {

        [TestMethod]
        public void RespondToPageUp()
        {
            // A bit nasty, but simulating PWF events is not easy
            var mockKeyboard = new MockKeyboardDevice();
            // Need a datacontext for this control - TODO would be better to keep clean
            var fpVM = new FinePotViewModel(TestHelper.TimelinePedal.Pots.First(), new Parameter{Definition= TestHelper.TimelinePedal.ControlParameters.First()});
            fpVM.ContextPedal=TestHelper.TimelinePedal;
            Sut.DataContext = fpVM;
            
            var privSut = new PrivateObject(Sut);
            privSut.Invoke("UserControl_GotFocus", new object[] { null, null });
            privSut.Invoke("PotControl_KeyUp", new object[] { null, mockKeyboard.CreateKeyEventArgs(Key.PageUp) });

            // Added 1
            Assert.AreEqual(1, Sut.Data);
        }

        [TestMethod]
        public void RespondToPageDown()
        {
            // A bit nasty, but simulating PWF events is not easy
            var mockKeyboard = new MockKeyboardDevice();
            // Need a datacontext for this control - TODO would be better to keep clean
            var fpVM = new FinePotViewModel(TestHelper.TimelinePedal.Pots.First(), new Parameter { Definition = TestHelper.TimelinePedal.ControlParameters.First() });
            fpVM.ContextPedal = TestHelper.TimelinePedal;
            Sut.DataContext = fpVM;
            Sut.Data = 20;
            var privSut = new PrivateObject(Sut);
            privSut.Invoke("UserControl_GotFocus", new object[] { null, null });
            privSut.Invoke("PotControl_KeyUp", new object[] { null, mockKeyboard.CreateKeyEventArgs(Key.PageDown) });

            // Subtracted 1
            Assert.AreEqual(19, Sut.Data);
        }


    }
}
