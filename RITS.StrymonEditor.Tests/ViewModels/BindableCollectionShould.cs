using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RITS.StrymonEditor.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace RITS.StrymonEditor.Tests
{
    [TestClass()]
    public class BindableCollectionShould
    {
        private BindableCollection<string> sut = new BindableCollection<string>();

        [TestMethod()]
        public void AddCorrectly()
        {
            // Arrange
            var currentCount =sut.Count;
            // Act
            sut.Add("Test");
            // Assert
            Assert.AreEqual(currentCount + 1, sut.Count);
        }

        [TestMethod()]
        public void ClearCorrectly()
        {
            // Arrange
            sut.Add("Test");
            sut.Add("Test2");
            sut.Add("Test3");
            // Act
            sut.Clear();
            // Assert
            Assert.AreEqual(0, sut.Count);
        }

        [TestMethod()]
        public void EvaluateContainsCorrectly()
        {
            // Arrange
            string testString = "Test";
            sut.Add(testString);
            // Act
            var result = sut.Contains(testString);
            // Assert
            Assert.IsTrue(result);
        }


        [TestMethod()]
        public void RemoveTest()
        {
            // Arrange
            string testString = "Test";
            sut.Add(testString);
            var currentCount = sut.Count;
            // Act
            sut.Remove(testString);
            // Assert
            Assert.AreEqual(currentCount - 1, sut.Count);
        }

        
    }
}
