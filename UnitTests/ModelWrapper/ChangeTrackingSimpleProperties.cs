using System.Collections.Generic;
using LinkTekTest.ModelWrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;

namespace UnitTests.ModelWrapper
{
    [TestClass]
    public class ChangeTrackingSimpleProperties
    {
        private const int Id = 2;
        private const string FirstName = "original first name";

        private Employee _employee;

        [TestInitialize]
        public void Initialize()
        {
            _employee = new Employee
            {
                Id = ChangeTrackingSimpleProperties.Id,
                FirstName = ChangeTrackingSimpleProperties.FirstName,
                Address = new Address(),
                Emails = new List<Email>()
            };
        }

        [TestMethod]
        public void ShouldStoreOriginalValue()
        {
            var employeeWrapper = new EmployeeWrapper(_employee);
            Assert.AreEqual(ChangeTrackingSimpleProperties.FirstName, employeeWrapper.FirstNameOriginalValue);

            employeeWrapper.FirstName = "different first name";
            Assert.AreEqual(ChangeTrackingSimpleProperties.FirstName, employeeWrapper.FirstNameOriginalValue);
        }

        [TestMethod]
        public void ShouldSetIsChanged()
        {
            var wrapper = new EmployeeWrapper(_employee);
            Assert.IsFalse(wrapper.FirstNameIsChanged);
            Assert.IsFalse(wrapper.IsChanged);
            wrapper.FirstName = "different first name";
            Assert.IsTrue(wrapper.FirstNameIsChanged);
            Assert.IsTrue(wrapper.IsChanged);
            wrapper.FirstName = ChangeTrackingSimpleProperties.FirstName;
            Assert.IsFalse(wrapper.FirstNameIsChanged);
            Assert.IsFalse(wrapper.IsChanged);
        }

        [TestMethod]
        public void ShouldRaisePropertyChangedEventOnSimplePropertyChange()
        {
            var eventFired = false;

            var wrapper = new EmployeeWrapper(_employee);

            wrapper.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(nameof(wrapper.FirstNameIsChanged)))
                {
                    eventFired = true;
                }
            };
            wrapper.FirstName = "something different";
            Assert.IsTrue(eventFired);
        }

        [TestMethod]
        public void ShouldRaisePropertyChangedEventOnIsChanged()
        {
            var eventFired = false;

            var wrapper = new EmployeeWrapper(_employee);

            wrapper.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(nameof(wrapper.IsChanged)))
                {
                    eventFired = true;
                }
            };
            wrapper.FirstName = "something different";
            Assert.IsTrue(eventFired);
        }

        [TestMethod]
        public void ShouldAcceptChanges()
        {
            var wrapper = new EmployeeWrapper(_employee);
            wrapper.FirstName = "something different";
            Assert.AreEqual("something different", wrapper.FirstName);
            Assert.AreEqual(ChangeTrackingSimpleProperties.FirstName, wrapper.FirstNameOriginalValue);
            Assert.IsTrue(wrapper.FirstNameIsChanged);
            Assert.IsTrue(wrapper.IsChanged);

            wrapper.AcceptChanges();

            Assert.AreEqual("something different", wrapper.FirstName);
            Assert.AreEqual("something different", wrapper.FirstNameOriginalValue);
            Assert.IsFalse(wrapper.FirstNameIsChanged);
            Assert.IsFalse(wrapper.IsChanged);
        }

        [TestMethod]
        public void ShouldRejectChanges()
        {
            var wrapper = new EmployeeWrapper(_employee);
            wrapper.FirstName = "something different";
            Assert.AreEqual("something different", wrapper.FirstName);
            Assert.AreEqual(ChangeTrackingSimpleProperties.FirstName, wrapper.FirstNameOriginalValue);
            Assert.IsTrue(wrapper.FirstNameIsChanged);
            Assert.IsTrue(wrapper.IsChanged);

            wrapper.RejectChanges();

            Assert.AreEqual(ChangeTrackingSimpleProperties.FirstName, wrapper.FirstName);
            Assert.AreEqual(ChangeTrackingSimpleProperties.FirstName, wrapper.FirstNameOriginalValue);
            Assert.IsFalse(wrapper.FirstNameIsChanged);
            Assert.IsFalse(wrapper.IsChanged);
        }
    }
}
