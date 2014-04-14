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
    public class BooleanToVisibilityConverterShould : TestContext<BooleanToVisibilityConverter>
    {
        [TestMethod]
        public void ReturnCollapsedWhenFalse()
        {
            Visibility retval = (Visibility)Sut.Convert(false, null, null,null);
            Assert.AreEqual(Visibility.Collapsed, retval);
        }
        [TestMethod]
        public void ReturnVisibleWhenTrue()
        {
            Visibility retval = (Visibility)Sut.Convert(true, null, null, null);
            Assert.AreEqual(Visibility.Visible, retval);
        }

        [TestMethod]
        public void ReturnVisibleWhenFalseAndReversed()
        {
            Sut.IsReversed = true;
            Visibility retval = (Visibility)Sut.Convert(false, null, null, null);
            Assert.AreEqual(Visibility.Visible, retval);
        }

        [TestMethod]
        public void ReturnHiddenWhenFalseAndUseHidden()
        {
            Sut.UseHidden = true;
            Visibility retval = (Visibility)Sut.Convert(false, null, null, null);
            Assert.AreEqual(Visibility.Hidden, retval);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ThrowNotImplementedOnConvertBack()
        {
            Sut.ConvertBack(false, null, null, null);
        }
    }

    
}
