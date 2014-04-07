using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RITS.StrymonEditor.Tests
{
    /// <summary>
    /// Helper classes to allow asserting PorprtyChange notifications
    /// See Mark Seeman's excellent blog post here
    /// http://blog.ploeh.dk/2009/08/06/AFluentInterfaceForTestingINotifyPropertyChanged/
    /// </summary>
    public static class NotifyPropertyChanged
    {
        public static NotifyExpectation<T>
            ShouldNotifyOn<T, TProperty>(this T owner,
            Expression<Func<T, TProperty>> propertyPicker)
            where T : INotifyPropertyChanged
        {
            return NotifyPropertyChanged.CreateExpectation(owner,
                propertyPicker, true);
        }

        public static NotifyExpectation<T>
            ShouldNotNotifyOn<T, TProperty>(this T owner,
            Expression<Func<T, TProperty>> propertyPicker)
            where T : INotifyPropertyChanged
        {
            return NotifyPropertyChanged.CreateExpectation(owner,
                propertyPicker, false);
        }

        private static NotifyExpectation<T>
            CreateExpectation<T, TProperty>(T owner,
            Expression<Func<T, TProperty>> pickProperty,
            bool eventExpected) where T : INotifyPropertyChanged
        {
            string propertyName =
                ((MemberExpression)pickProperty.Body).Member.Name;
            return new NotifyExpectation<T>(owner,
                propertyName, eventExpected);
        }
    }

    public class NotifyExpectation<T>
    where T : INotifyPropertyChanged
    {
        private readonly T owner;
        private readonly string propertyName;
        private readonly bool eventExpected;

        public NotifyExpectation(T owner,
            string propertyName, bool eventExpected)
        {
            this.owner = owner;
            this.propertyName = propertyName;
            this.eventExpected = eventExpected;
        }

        public void When(Action<T> action)
        {
            bool eventWasRaised = false;
            this.owner.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == this.propertyName)
                {
                    eventWasRaised = true;
                }
            };
            action(this.owner);

            Assert.AreEqual<bool>(this.eventExpected,
                eventWasRaised,
                "PropertyChanged on {0}", this.propertyName);
        }
    }
}
