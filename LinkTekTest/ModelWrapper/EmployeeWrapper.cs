using System;
using System.Linq;
using LinkTekTest.Utilities;
using Model;

namespace LinkTekTest.ModelWrapper
{
    public class EmployeeWrapper : ModelWrapper<Employee>
    {
        public EmployeeWrapper(Employee employee) : base(employee)
        {
            InitializeComplexProperties(employee);
            InitializeCollectionProperties(employee);
        } 

        #region Initialization
        private void InitializeComplexProperties(Employee employee)
        {
            if (employee.Address == null)
                throw new ArgumentException("Address cannot be null.");

            Address = new AddressWrapper(employee.Address);

            // Change tracking
            RegisterComplexOject(Address);
        }

        private void InitializeCollectionProperties(Employee employee)
        {
            if (employee.Emails == null)
                throw new ArgumentException("Emails cannot be null.");

            Emails = new ChangeTrackingCollection<EmailWrapper>(
              employee.Emails.Select(e => new EmailWrapper(e)));

            // Change tracking
            RegisterCollection(Emails, employee.Emails);

            // Select first email
            if (Emails.Count > 0)
                SelectedEmail = Emails[0];
        }
        #endregion


        #region Properties
        public int Id
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public string FirstName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string LastName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public AddressWrapper Address { get; private set; }

        public ChangeTrackingCollection<EmailWrapper> Emails { get; private set; }

        private EmailWrapper _selectedEmail;
        public EmailWrapper SelectedEmail
        {
            get => _selectedEmail;
            set
            {
                if (Equals(_selectedEmail, value))
                    return;
                _selectedEmail = value;
                OnPropertyChanged(nameof(SelectedEmail));
                OnPropertyChanged(nameof(EmailIsSelected));
            }
        }

        public bool EmailIsSelected => SelectedEmail != null;
        #endregion

        #region ChangeTrackingProperties
        public string FirstNameOriginalValue => GetOriginalValue<string>(nameof(FirstName));
        public bool FirstNameIsChanged => GetIsChanged(nameof(FirstName));
        public string LastNameOriginalValue => GetOriginalValue<string>(nameof(LastName));
        public bool LastNameIsChanged => GetIsChanged(nameof(LastName));
        #endregion
    }
}
