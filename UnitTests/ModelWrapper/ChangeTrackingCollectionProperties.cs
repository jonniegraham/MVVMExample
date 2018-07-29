using System.Collections.Generic;
using System.Linq;
using LinkTekTest.Utilities;
using LinkTekTest.ModelWrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;

namespace UnitTests.ModelWrapper
{
    [TestClass]
    public class ChangeTrackingCollectionProperties
    {
        private string email1 = "john.graham@outlook.co.nz";
        private string email2 = "datasemantics@outlook.co.nz";

        private List<EmailWrapper> _emails;

        [TestInitialize]
        public void Initialize()
        {
            _emails = new List<EmailWrapper>
            {
                new EmailWrapper(new Email {EmailAddress = email1}),
                new EmailWrapper(new Email {EmailAddress = email2})
            };
        }

        [TestMethod]
        public void ShouldTrackAddedItems()
        {
            var emailToAdd = new EmailWrapper(new Email());
            var trackingCollection = new ChangeTrackingCollection<EmailWrapper>(_emails);
            Assert.AreEqual(2, trackingCollection.Count);
            Assert.IsFalse(trackingCollection.IsChanged);

            trackingCollection.Add(emailToAdd);

            Assert.AreEqual(3, trackingCollection.Count);
            Assert.AreEqual(1, trackingCollection.AddedItems.Count);
            Assert.AreEqual(0, trackingCollection.RemovedItems.Count);
            Assert.AreEqual(0, trackingCollection.ModifiedItems.Count);
            Assert.AreEqual(emailToAdd, trackingCollection.AddedItems.First());

            trackingCollection.Remove(emailToAdd);
            Assert.AreEqual(2, trackingCollection.Count);
            Assert.AreEqual(0, trackingCollection.AddedItems.Count);
            Assert.AreEqual(0, trackingCollection.RemovedItems.Count);
            Assert.AreEqual(0, trackingCollection.ModifiedItems.Count);
            Assert.IsFalse(trackingCollection.IsChanged);
        }

        [TestMethod]
        public void ShouldTrackModifiedItems()
        {
            var emailToRemove = _emails.First();
            var trackingCollection = new ChangeTrackingCollection<EmailWrapper>(_emails);
            Assert.AreEqual(2, trackingCollection.Count);
            Assert.IsFalse(trackingCollection.IsChanged);

            trackingCollection.Remove(emailToRemove);
            Assert.AreEqual(1, trackingCollection.Count);
            Assert.AreEqual(0, trackingCollection.AddedItems.Count);
            Assert.AreEqual(1, trackingCollection.RemovedItems.Count);
            Assert.AreEqual(0, trackingCollection.ModifiedItems.Count);
            Assert.AreEqual(emailToRemove, trackingCollection.RemovedItems.First());
            Assert.IsTrue(trackingCollection.IsChanged);

            trackingCollection.Add(emailToRemove);
            Assert.AreEqual(2, trackingCollection.Count);
            Assert.AreEqual(0, trackingCollection.AddedItems.Count);
            Assert.AreEqual(0, trackingCollection.RemovedItems.Count);
            Assert.AreEqual(0, trackingCollection.ModifiedItems.Count);
            Assert.IsFalse(trackingCollection.IsChanged);
        }

        [TestMethod]
        public void ShouldTrackRemovedItems()
        {
            var emailToModify = _emails.First();
            var trackingCollection = new ChangeTrackingCollection<EmailWrapper>(_emails);
            Assert.AreEqual(2, trackingCollection.Count);
            Assert.IsFalse(trackingCollection.IsChanged);

            emailToModify.EmailAddress = "newemail@new.com";
            Assert.AreEqual(0, trackingCollection.AddedItems.Count);
            Assert.AreEqual(1, trackingCollection.ModifiedItems.Count);
            Assert.AreEqual(0, trackingCollection.RemovedItems.Count);
            Assert.IsTrue(trackingCollection.IsChanged);

            emailToModify.EmailAddress = email1;
            Assert.AreEqual(0, trackingCollection.AddedItems.Count);
            Assert.AreEqual(0, trackingCollection.ModifiedItems.Count);
            Assert.AreEqual(0, trackingCollection.RemovedItems.Count);
            Assert.IsFalse(trackingCollection.IsChanged);
        }

        [TestMethod]
        public void ShouldTrackAddedAsModified()
        {
            var emailToAdd = new EmailWrapper(new Email());

            var c = new ChangeTrackingCollection<EmailWrapper>(_emails);
            c.Add(emailToAdd);
            emailToAdd.EmailAddress = "newemail1@new1.com";
            Assert.IsTrue(emailToAdd.IsChanged);
            Assert.AreEqual(3, c.Count);
            Assert.AreEqual(1, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.IsTrue(c.IsChanged);
        }

        [TestMethod]
        public void ShouldTrackRemovedAsModified()
        {
            var emailToModifyAndRemove = _emails.First();

            var trackingCollection = new ChangeTrackingCollection<EmailWrapper>(_emails);
            emailToModifyAndRemove.EmailAddress = "newemail1@new1.com";
            Assert.AreEqual(2, trackingCollection.Count);
            Assert.AreEqual(0, trackingCollection.AddedItems.Count);
            Assert.AreEqual(0, trackingCollection.RemovedItems.Count);
            Assert.AreEqual(1, trackingCollection.ModifiedItems.Count);
            Assert.AreEqual(emailToModifyAndRemove, trackingCollection.ModifiedItems.First());
            Assert.IsTrue(trackingCollection.IsChanged);

            trackingCollection.Remove(emailToModifyAndRemove);
            Assert.AreEqual(1, trackingCollection.Count);
            Assert.AreEqual(0, trackingCollection.AddedItems.Count);
            Assert.AreEqual(1, trackingCollection.RemovedItems.Count);
            Assert.AreEqual(0, trackingCollection.ModifiedItems.Count);
            Assert.AreEqual(emailToModifyAndRemove, trackingCollection.RemovedItems.First());
            Assert.IsTrue(trackingCollection.IsChanged);
        }

        [TestMethod]
        public void ShouldAcceptChanged()
        {
            var emailToModify = _emails.First();
            var emailToRemove = _emails.Skip(1).First();
            var emailToAdd = new EmailWrapper(new Email { EmailAddress = "anotherOne@thomasclaudiushuber.com" });

            var trackingCollection = new ChangeTrackingCollection<EmailWrapper>(_emails);

            trackingCollection.Add(emailToAdd);
            trackingCollection.Remove(emailToRemove);
            emailToModify.EmailAddress = "newemail1@new1.com";
            Assert.AreEqual(email1, emailToModify.EmailAddressOriginalValue);

            Assert.AreEqual(2, trackingCollection.Count);
            Assert.AreEqual(1, trackingCollection.AddedItems.Count);
            Assert.AreEqual(1, trackingCollection.ModifiedItems.Count);
            Assert.AreEqual(1, trackingCollection.RemovedItems.Count);

            trackingCollection.AcceptChanges();

            Assert.AreEqual(2, trackingCollection.Count);
            Assert.IsTrue(trackingCollection.Contains(emailToModify));
            Assert.IsTrue(trackingCollection.Contains(emailToAdd));

            Assert.AreEqual(0, trackingCollection.AddedItems.Count);
            Assert.AreEqual(0, trackingCollection.ModifiedItems.Count);
            Assert.AreEqual(0, trackingCollection.RemovedItems.Count);

            Assert.IsFalse(emailToModify.IsChanged);
            Assert.AreEqual("newemail1@new1.com", emailToModify.EmailAddress);
            Assert.AreEqual("newemail1@new1.com", emailToModify.EmailAddressOriginalValue);

            Assert.IsFalse(trackingCollection.IsChanged);
        }

        [TestMethod]
        public void ShouldRejectChanges()
        {
            var emailToModify = _emails.First();
            var emailToRemove = _emails.Skip(1).First();
            var emailToAdd = new EmailWrapper(new Email { EmailAddress = "newEmail1@new1.com" });

            var trackingCollection = new ChangeTrackingCollection<EmailWrapper>(_emails);

            trackingCollection.Add(emailToAdd);
            trackingCollection.Remove(emailToRemove);
            emailToModify.EmailAddress = "newEmail2@new2.com";
            Assert.AreEqual(email1, emailToModify.EmailAddressOriginalValue);

            Assert.AreEqual(2, trackingCollection.Count);
            Assert.AreEqual(1, trackingCollection.AddedItems.Count);
            Assert.AreEqual(1, trackingCollection.ModifiedItems.Count);
            Assert.AreEqual(1, trackingCollection.RemovedItems.Count);

            trackingCollection.RejectChanges();

            Assert.AreEqual(2, trackingCollection.Count);
            Assert.IsTrue(trackingCollection.Contains(emailToModify));
            Assert.IsTrue(trackingCollection.Contains(emailToRemove));

            Assert.AreEqual(0, trackingCollection.AddedItems.Count);
            Assert.AreEqual(0, trackingCollection.ModifiedItems.Count);
            Assert.AreEqual(0, trackingCollection.RemovedItems.Count);

            Assert.IsFalse(emailToModify.IsChanged);
            Assert.AreEqual(email1, emailToModify.EmailAddress);
            Assert.AreEqual(email1, emailToModify.EmailAddressOriginalValue);

            Assert.IsFalse(trackingCollection.IsChanged);
        }
    }
}
