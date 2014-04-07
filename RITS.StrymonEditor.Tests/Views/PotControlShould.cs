using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Text;
using System.Threading.Tasks;
using RITS.StrymonEditor.Views;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace RITS.StrymonEditor.Tests
{
    [TestClass()]
    public class PotControlShould:TestContext<PotControl>
    {
        [TestMethod]
        public void RespectAngleBounds()
        {
            Sut.Angle = 295;
            Assert.AreEqual(290, Sut.Angle);
            Sut.Angle = -20;
            Assert.AreEqual(0, Sut.Angle);
        }

        [TestMethod]
        public void RespondToPageUp()
        {
            // A bit nasty, but simulating PWF events is not easy
            var mockKeyboard = new MockKeyboardDevice();
            
            var privSut = new PrivateObject(Sut);
            privSut.Invoke("UserControl_GotFocus", new object[]{null,null});
            privSut.Invoke("PotControl_KeyUp", new object[] { null, mockKeyboard.CreateKeyEventArgs(Key.PageUp) });

            // Added 10
            Assert.AreEqual(10, Sut.Angle);
        }

        [TestMethod]
        public void RespondToPageDown()
        {
            // A bit nasty, but simulating PWF events is not easy
            var mockKeyboard = new MockKeyboardDevice();
            Sut.Angle = 20;
            var privSut = new PrivateObject(Sut);
            privSut.Invoke("UserControl_GotFocus", new object[] { null, null });
            privSut.Invoke("PotControl_KeyUp", new object[] { null, mockKeyboard.CreateKeyEventArgs(Key.PageDown) });

            // Subtracted 10
            Assert.AreEqual(10, Sut.Angle);
        }

        [TestMethod]
        public void RespondToPageUpWithControl()
        {
            // A bit nasty, but simulating PWF events is not easy
            var mockKeyboard = new MockKeyboardDevice();
            var privSut = new PrivateObject(Sut);
            privSut.Invoke("UserControl_GotFocus", new object[] { null, null });
            privSut.Invoke("PotControl_KeyUp", new object[] { null, mockKeyboard.CreateKeyEventArgs(Key.PageUp, ModifierKeys.Control) });

            // Added 1
            Assert.AreEqual(1, Sut.Angle);
        }

        [TestMethod]
        public void RespondToPageDownWithControl()
        {
            // A bit nasty, but simulating PWF events is not easy

            // A bit nasty, but simulating PWF events is not easy
            var mockKeyboard = new MockKeyboardDevice();
            Sut.Angle = 20;
            var privSut = new PrivateObject(Sut);
            privSut.Invoke("UserControl_GotFocus", new object[] { null, null });
            privSut.Invoke("PotControl_KeyUp", new object[] { null, mockKeyboard.CreateKeyEventArgs(Key.PageDown, ModifierKeys.Control) });

            // Subtracted 1
            Assert.AreEqual(19, Sut.Angle);
        }
    }

    
}
