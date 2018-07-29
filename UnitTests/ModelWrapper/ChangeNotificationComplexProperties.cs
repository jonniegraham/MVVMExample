using System.Collections.Generic;
using LinkTekTest.ModelWrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;

namespace UnitTests.ModelWrapper
{
    [TestClass]
    public class ChangeNotificationComplexProperties
    {
        private const int Id = 2;
        private const string FirstName = "sgFr7";

        private Employee _employee;

        [TestInitialize]
        public void Initialize()
        {
            _employee = new Employee
            {
                Id = ChangeNotificationComplexProperties.Id,
                FirstName = ChangeNotificationComplexProperties.FirstName,
                Address = new Address(),
                Emails = new List<Email>()
            };
        }

        [TestMethod]
        public void ShouldInitializeAddressProperty()
        {
            var wrapper = new EmployeeWrapper(_employee);
            Assert.IsNotNull(wrapper.Address);
            Assert.AreEqual(_employee.Address, wrapper.Address.Model);
        }
    }
}