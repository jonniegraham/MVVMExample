using System;
using System.Collections.Generic;
using LinkTekTest.ModelWrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;

namespace UnitTests.ModelWrapper
{
    [TestClass]
    public class SimpleProperties
    {
        private const int Id = 2;
        private const string FirstName = "sgFr7";

        private Employee _employee;

        [TestInitialize]
        public void Initialize()
        {
            _employee = new Employee
            {
                Id = SimpleProperties.Id,
                FirstName = SimpleProperties.FirstName,
                Address = new Address(),
                Emails = new List<Email>()
            };
        }

        [TestMethod]
        public void ShouldContainModelInModelProperty()
        {
            var wrapper = new EmployeeWrapper(_employee);
            Assert.AreEqual(_employee, wrapper.Model);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowArgumentNullExceptionIfModelIsNull()
        {
            try
            {
                new EmployeeWrapper(null);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("model", ex.ParamName);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowArgumentExceptionIfAddressIsNull()
        {
            try
            {
                _employee.Address = null;
                new EmployeeWrapper(_employee);
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Address cannot be null.", ex.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowArgumentExceptionIfEmailsCollectionIsNull()
        {
            try
            {
                _employee.Emails = null;
                new EmployeeWrapper(_employee);
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Emails cannot be null.", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void ShouldGetValueOfUnderlyingModelProperty()
        {
            var wrapper = new EmployeeWrapper(_employee);
            Assert.AreEqual(_employee.FirstName, wrapper.FirstName);
        }

        [TestMethod]
        public void ShouldSetValueOfUnderlyingModelProperty()
        {
            var wrapper = new EmployeeWrapper(_employee);
            const string firstName = "gHwo3";
            wrapper.FirstName = firstName;
            Assert.AreEqual(firstName, _employee.FirstName);
        }
    }
}
