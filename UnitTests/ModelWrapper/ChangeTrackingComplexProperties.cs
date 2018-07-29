using System.Collections.Generic;
using LinkTekTest.ModelWrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;

namespace UnitTests.ModelWrapper
{
    [TestClass]
    public class ChangeTrackingComplexProperties
    {
        private const int Id = 2;
        private const string FirstName = "original first name";
        private const string City = "original city";
        private Employee _employee;

        [TestInitialize]
        public void Initialize()
        {
            _employee = new Employee
            {
                Id = ChangeTrackingComplexProperties.Id,
                FirstName = ChangeTrackingComplexProperties.FirstName,
                Address = new Address
                {
                    City = ChangeTrackingComplexProperties.City
                },
                Emails = new List<Email>()
            };
        }

        [TestMethod]
        public void ShouldSetIsChangedOfAddressWrapper()
        {
            var wrapper = new EmployeeWrapper(_employee);
            wrapper.Address.City = "different city";
            Assert.IsTrue(wrapper.Address.IsChanged);

            wrapper.Address.City = ChangeTrackingComplexProperties.City;
            Assert.IsFalse(wrapper.Address.IsChanged);
        }

        [TestMethod]
        public void ShouldSetIsChanged()
        {
            var wrapper = new EmployeeWrapper(_employee);
            wrapper.Address.City = "different city";
            Assert.IsTrue(wrapper.IsChanged);

            wrapper.Address.City = ChangeTrackingComplexProperties.City;
            Assert.IsFalse(wrapper.IsChanged);
        }

        [TestMethod]
        public void ShouldRaisePropertyChangedEventForIsChanged()
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
            wrapper.Address.City = "different city";
            Assert.IsTrue(eventFired);
        }

        [TestMethod]
        public void ShouldAcceptChanges()
        {
            var wrapper = new EmployeeWrapper(_employee);
            wrapper.Address.City = "different city";
            Assert.AreEqual(ChangeTrackingComplexProperties.City, wrapper.Address.CityOriginalValue);

            wrapper.AcceptChanges();

            Assert.IsFalse(wrapper.IsChanged);
            Assert.AreEqual("different city", wrapper.Address.City);
            Assert.AreEqual("different city", wrapper.Address.CityOriginalValue);
        }

        [TestMethod]
        public void ShouldRejectChanges()
        {
            var wrapper = new EmployeeWrapper(_employee);
            wrapper.Address.City = "different city";
            Assert.AreEqual(ChangeTrackingComplexProperties.City, wrapper.Address.CityOriginalValue);

            wrapper.RejectChanges();

            Assert.IsFalse(wrapper.IsChanged);
            Assert.AreEqual(ChangeTrackingComplexProperties.City, wrapper.Address.City);
            Assert.AreEqual(ChangeTrackingComplexProperties.City, wrapper.Address.CityOriginalValue);
        }
    }
}
