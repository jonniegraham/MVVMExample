using System.Collections.Generic;
using LinkTekTest.ModelWrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;

namespace UnitTests.ModelWrapper
{
    [TestClass]
    public class ChangeNotificationSimpleProperties
    {
        private const int Id = 2;
        private const string FirstName = "sgFr7";

        private Employee _employee;

        [TestInitialize]
        public void Initialize()
        {
            _employee = new Employee
            {
                Id = ChangeNotificationSimpleProperties.Id,
                FirstName = ChangeNotificationSimpleProperties.FirstName,
                Address = new Address(),
                Emails = new List<Email>()
            };
        }

        [TestMethod]
        public void ShouldRaisePropertyChangedEventOnPropertyChange()
        {
            var eventFired = false;

            var wrapper = new EmployeeWrapper(_employee);

            wrapper.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(nameof(wrapper.FirstName)))
                {
                    eventFired = true;
                }
            };
            wrapper.FirstName = "something different";
            Assert.IsTrue(eventFired);
        }

        [TestMethod]
        public void ShouldNotRaisePropertyChangedEventOnPropertySetToOriginalValue()
        {
            var eventFired = false;

            var wrapper = new EmployeeWrapper(_employee);

            wrapper.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(nameof(wrapper.FirstName)))
                {
                    eventFired = true;
                }
            };
            wrapper.FirstName = FirstName;
            Assert.IsFalse(eventFired);
        }
    }
}