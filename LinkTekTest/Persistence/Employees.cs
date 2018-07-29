using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DataAccess;
using LinkTekTest.ModelWrapper;
using Model;

namespace LinkTekTest.Persistence
{
    /// <summary>
    /// Interacts with data source and:
    /// 1.) Persists Employee model-wrapper object data.
    /// 2.) Gets Employee model-wrapper objects initialised from persisted data.
    /// </summary>
    internal partial class Employees
    {
        private readonly IDatabaseService _databaseService;

        public Employees(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// Persists Employee model-wrapper object data to the data source.
        /// </summary>
        /// <param name="employee">Employee model-wrapper object</param>
        /// <returns>Primary key of newly persisted employee model-wrapper data</returns>
        public async Task SaveEmployeeAsync(EmployeeWrapper employee)
        {
            if (!employee.IsChanged)
                return;

            if (await _databaseService.ExistsAsync("employee", new Dictionary<string, dynamic>
            {
                { "employee_id", employee.Id }
            }))
            {
                await _databaseService.UpdateRowAsync("employee", new Dictionary<string, dynamic>
                {
                    { "first_name", employee.FirstName },
                    { "last_name", employee.LastName }
                }, new Dictionary<string, dynamic>
                {
                    { "employee_id", employee.Id }
                });
            }
            else
            {
                employee.Id = Convert.ToInt32(await _databaseService.InsertRowAsync("employee", new Dictionary<string, dynamic>()
                {
                    { "first_name", employee.FirstName },
                    { "last_name", employee.LastName }
                }));
            }

            await SaveAddressAsync(employee);
            await SaveEmailAsync(employee);
        }

        /// <summary>
        /// Gets Employee model-wrapper objects initialised from persisted data.
        /// </summary>
        /// <returns>Observable collection of employee model-wrapper objects</returns>
        public async Task<ObservableCollection<EmployeeWrapper>> GetEmployeesAsync()
        {
            var employeeRows = await _databaseService.SelectRowsAsync("employee", new Dictionary<string, dynamic>
            {
                {"is_deleted", 0 }
            });
            var employees = new ObservableCollection<EmployeeWrapper>();
            foreach (var employeeRow in employeeRows)
            {
                var employeeId = employeeRow["employee_id"];

                employees.Add(new EmployeeWrapper(
                    new Employee
                    {
                        Id = employeeId,
                        FirstName = employeeRow["first_name"],
                        LastName = employeeRow["last_name"],
                        Address = await GetAddressByEmployeeIdAsyc(employeeId),
                        Emails = await GetEmailsByEmployeeIdAsync(employeeId)
                    }));
            }
            return employees;
        }

        /// <summary>
        /// Get single employee(wrapper) model oject by primary key
        /// </summary>
        /// <param name="employeeId">primary key</param>
        /// <returns>employee(wrapper) model object</returns>
        public async Task<EmployeeWrapper> GetEmployeeWrapperByIdAsync(int employeeId)
        {
            var employeeRows = await _databaseService.SelectRowsAsync("employee", new Dictionary<string, dynamic>()
            {
                { "employee_id", employeeId },
                { "is_deleted", 0 }
            });

            if (employeeRows.Count == 0)
                throw new ArgumentException($"Invalid employeeId: {employeeId}");

            return new EmployeeWrapper(
                new Employee
                {
                    Id = employeeId,
                    FirstName = employeeRows[0]["first_name"],
                    LastName = employeeRows[0]["last_name"],
                    Address = await GetAddressByEmployeeIdAsyc(employeeId),
                    Emails = await GetEmailsByEmployeeIdAsync(employeeId)
                });
        }

        public async Task DeleteEmployeeAsync(EmployeeWrapper employee)
        {
            if (await _databaseService.ExistsAsync("employee", new Dictionary<string, dynamic>
            {
                { "employee_id", employee.Id }
            }))
            {
                await _databaseService.UpdateRowAsync("employee", new Dictionary<string, dynamic>
                {
                    { "is_deleted", 1 }
                }, new Dictionary<string, dynamic>
                {
                    { "employee_id", employee.Id }
                });
            }
        }
    }
}
