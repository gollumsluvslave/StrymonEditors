using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading.Tasks;
using RITS.StrymonEditor.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace RITS.StrymonEditor.Tests
{
    [TestClass()]
    public class BindableCollectionShould
    {
        private BindableCollection<string> sut = new BindableCollection<string>();
        [TestInitialize]
        public void Setup()
        {
            sut.CollectionChanged += DummyHandler;
        }
        private void DummyHandler(object sender, NotifyCollectionChangedEventArgs args)
        {
        }

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
        public void InsertCorrectly()
        {
            // Arrange
            var currentCount = sut.Count;
            // Act
            sut.Insert(0,"Test");
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
        public void RemoveCorrectly()
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

        [TestMethod()]
        public void RemoveAtCorrectly()
        {
            // Arrange
            string testString = "Test";
            sut.Add(testString);
            var currentCount = sut.Count;
            // Act
            sut.RemoveAt(0);
            // Assert
            Assert.AreEqual(currentCount - 1, sut.Count);
        }
        
        [TestMethod()]
        public void EvaluateIndexOfCorrectly()
        {
            string testString = "Test";
            sut.Add(testString);
            var currentCount = sut.Count;
            Assert.AreEqual(0,sut.IndexOf(testString));

        }

        [TestMethod()]
        public void MiscCodeCoverageTests()
        {
            string testString = "Test";
            sut.Add(testString);
            foreach (var x in sut)
            {
                //
            }
            sut[0] = "new";
            var r = sut.IsReadOnly;

            sut.CopyTo(new string[]{"blah"},0);

            sut.Remove(null);
            sut.RemoveAt(76);
        }

        [TestMethod]
        public void UseDispatcher()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, e) => RunTestsInBackgroundThread();
            worker.RunWorkerAsync();
            DispatcherUtil.DoEvents();
        }

        private void RunTestsInBackgroundThread()
        {
            RemoveCorrectly();
            RemoveAtCorrectly();
            AddCorrectly();
            InsertCorrectly();
            ClearCorrectly();

        }
        
    }
}
