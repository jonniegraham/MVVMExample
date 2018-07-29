using System;
using System.Collections.Generic;
using LinkTekTest.Persistence;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using LinkTekTest.Command;
using LinkTekTest.ModelWrapper;
using Model;

namespace LinkTekTest.ViewModel
{
    internal class MainWindowViewModel : Observable
    {
        public MainWindowViewModel()
        {
            Application.Current.Dispatcher.Invoke(async () => { Employees = await Data.Employees().GetEmployeesAsync(); });
            if (Employees.Count > 0)
                SelectedEmployee = Employees.First();
        }

        /// <summary>
        /// Properties Exposed to the UI.
        /// </summary>
        #region Properties
        public ObservableCollection<EmployeeWrapper> Employees { get; set; }

        private EmployeeWrapper _selectedEmployee;
        public EmployeeWrapper SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                if (Equals(_selectedEmployee, value))
                    return;
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));
            }
        }
        #endregion

        /// <summary>
        /// Button commands
        /// </summary>
        #region ICommand
        public RelayCommand NewEmployee => new RelayCommand(o =>
        {
            var newEmployee = new EmployeeWrapper(new Employee
            {
                Emails = new List<Email> { new Email() },
                Address = new Address()
            });

            // This results in 'change' indication in UI.
            //newEmployee.Emails.Add(new EmailWrapper(new Email()));

            Employees.Add(newEmployee);
            // Select new employee in UI.
            SelectedEmployee = newEmployee;
            // Select new email in UI.
            newEmployee.SelectedEmail = newEmployee.Emails.ElementAt(0);

        }, o => true);

        public RelayCommand SaveEmployee => new RelayCommand( async o => 
        {
            // Update data source.
            await Data.Employees().SaveEmployeeAsync(SelectedEmployee);
            // Update model (changes to Bound values will be reflected in UI).
            SelectedEmployee.AcceptChanges();
        }, o => SelectedEmployee?.IsChanged ?? false);

        public RelayCommand ResetEmployee => new RelayCommand(o =>
        {
            // Update model (changes to Bound values will be reflected in UI).
            SelectedEmployee.RejectChanges();
        }, o => SelectedEmployee?.IsChanged ?? false);

        public RelayCommand DeleteEmployee => new RelayCommand(async o =>
        {
            // Update data source.
            await Data.Employees().DeleteEmployeeAsync(SelectedEmployee);
            // Update UI.
            var index = Employees.IndexOf(SelectedEmployee);
            Employees.Remove(SelectedEmployee);
            SelectedEmployee = Employees.Count > 0 ? Employees.ElementAt(Math.Max(index - 1, 0)) : null;
        }, o => SelectedEmployee != null);

        public RelayCommand DeleteEmail => new RelayCommand(o =>
        {
            var oldEmail = SelectedEmployee.SelectedEmail;
            SelectedEmployee.Emails.Remove(oldEmail);
            if (SelectedEmployee.Emails.Count > 0)
                SelectedEmployee.SelectedEmail = SelectedEmployee.Emails.ElementAt(0);
        }, o => SelectedEmployee.EmailIsSelected);

        public RelayCommand AddEmail => new RelayCommand(o =>
        {
            var newEmail = new EmailWrapper(new Email());
            SelectedEmployee.Emails.Add(newEmail);

            // Select new email in UI.
            SelectedEmployee.SelectedEmail = newEmail;
        }, o => SelectedEmployee != null);
        #endregion
    }
}
