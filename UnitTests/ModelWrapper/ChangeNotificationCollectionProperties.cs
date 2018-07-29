using System.Collections.Generic;
using System.Linq;
using LinkTekTest.ModelWrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;

namespace UnitTests.ModelWrapper
{
    [TestClass]
    public class ChangeNotificationCollectionProperties
    {
        private const int Id = 2;
        private const string FirstName = "sgFr7";
        private Email _email;
        private Employee _employee;

        [TestInitialize]
        public void Initialize()
        {
            _email = new Email { EmailAddress = "some_email2@domain2.com" };
            _employee = new Employee
            {
                Id = ChangeNotificationCollectionProperties.Id,
                FirstName = ChangeNotificationCollectionProperties.FirstName,
                Address = new Address(),
                Emails = new List<Email>
                {
                    new Email{ EmailAddress = "some_email1@domain1.com"},
                    _email
                }
            };
        }

        [TestMethod]
        public void ShouldInitializeEmailsProperty()
        {
            var wrapper = new EmployeeWrapper(_employee);
            Assert.IsNotNull(wrapper.Emails);
            CheckIfModelEmailsCollectionIsInSync(wrapper);
        }


        [TestMethod]
        public void ShouldBeInSyncAfterRemovingEmail()
        {
            var wrapper = new EmployeeWrapper(_employee);
            var emailToRemove = wrapper.Emails.Single(ew => ew.Model == _email);
            wrapper.Emails.Remove(emailToRemove);
            CheckIfModelEmailsCollectionIsInSync(wrapper);
        }

        [TestMethod]
        public void ShouldBeInSyncAfterAddingEmail()
        {
            _employee.Emails.Remove(_email);
            var wrapper = new EmployeeWrapper(_employee);
            wrapper.Emails.Add(new EmailWrapper(_email));
            CheckIfModelEmailsCollectionIsInSync(wrapper);
        }

        [TestMethod]
        public void ShouldBeInSyncAfterClearingEmails()
        {
            var wrapper = new EmployeeWrapper(_employee);
            wrapper.Emails.Clear();
            CheckIfModelEmailsCollectionIsInSync(wrapper);
        }

        private void CheckIfModelEmailsCollectionIsInSync(EmployeeWrapper wrapper)
        {
            Assert.AreEqual(_employee.Emails.Count, wrapper.Emails.Count);
            Assert.IsTrue(_employee.Emails.All(e =>
                wrapper.Emails.Any(we => we.Model == e)));
        }
    }
}
