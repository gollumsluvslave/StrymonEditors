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
    public class OptionValueLabelConverterShould : TestContext<OptionValueLabelConverter>
    {

        [TestMethod]
        public void ReturnDirectMatchCorrectly()
        {
            var list = new Option[]{ new Option{Name="Test",Value=10}}.ToList();
            Container.Register<List<Option>>(list);
            // Act
            var retval =Sut.ValueToLabel(10);
            Assert.AreEqual("Test", retval);
        }

        [TestMethod]
        public void ReturnMinCorrectly()
        {
            var list = new Option[] { 
                                        new Option { Name = "Test1", Value = 0 }, 
                                        new Option { Name = "Test2", Value = 67 }, 
                                        new Option { Name = "Test3", Value = 127 } 
                                    }.ToList();
            Container.Register<List<Option>>(list);
            // Act
            var retval = Sut.ValueToLabel(-10);
            Assert.AreEqual("Test1", retval);
        }
        
        [TestMethod]
        public void ReturnMaxCorrectly()
        {
            var list = new Option[] { 
                                        new Option { Name = "Test1", Value = 0 }, 
                                        new Option { Name = "Test2", Value = 67 }, 
                                        new Option { Name = "Test3", Value = 127 } 
                                    }.ToList();
            Container.Register<List<Option>>(list);
            // Act
            var retval = Sut.ValueToLabel(300);
            Assert.AreEqual("Test3", retval);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowCorrectlyWhenNoMatch()
        {
            var list = new Option[] { 
                                        new Option { Name = "Test1", Value = 0 }, 
                                        new Option { Name = "Test2", Value = 67 }, 
                                        new Option { Name = "Test3", Value = 127 } 
                                    }.ToList();
            Container.Register<List<Option>>(list);
            // Act
            var retval = Sut.ValueToLabel(13);
            
        }
    }
}
