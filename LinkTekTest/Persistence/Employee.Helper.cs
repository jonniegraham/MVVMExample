using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinkTekTest.ModelWrapper;
using Model;

namespace LinkTekTest.Persistence
{
    internal partial class Employees
    {
        private async Task SaveAddressAsync(EmployeeWrapper employee)
        {
            if (!employee.Address.IsChanged)
                return;

            if (await _databaseService.ExistsAsync(tableName: "address", whereConditions: new Dictionary<string, dynamic>
            {
                { "address_id", employee.Address.Id }
            }))
            {
                await _databaseService.UpdateRowAsync(tableName: "address", columnValuePairs: new Dictionary<string, dynamic>
                {
                    { "number", employee.Address.Number },
                    { "street", employee.Address.Street },
                    { "city", employee.Address.City }
                }, whereConditions: new Dictionary<string, dynamic>
                {
                    { "address_id", employee.Address.Id }
                });
            }
            else
            {
                employee.Id = Convert.ToInt32(await _databaseService.InsertRowAsync(tableName: "address", columnValuePairs: new Dictionary<string, dynamic>()
                {
                    { "number", employee.Address.Number },
                    { "street", employee.Address.Street },
                    { "city", employee.Address.City },
                    { "employee_id", employee.Id }
                }));
            }
        }

        private async Task SaveEmailAsync(EmployeeWrapper employee)
        {
            if (!employee.Emails.IsChanged)
                return;

            foreach (var addedEmail in employee.Emails.AddedItems)
            {
                await _databaseService.InsertRowAsync(tableName: "email", columnValuePairs: new Dictionary<string, dynamic>
                {
                    {"email_address", addedEmail.EmailAddress},
                    {"notes", addedEmail.Notes},
                    {"employee_id", employee.Id}
                });
            }
            foreach (var modifiedEmail in employee.Emails.ModifiedItems)
            {
                await _databaseService.UpdateRowAsync(tableName: "email", columnValuePairs: new Dictionary<string, dynamic>
                {
                    {"email_address", modifiedEmail.EmailAddress},
                    {"notes", modifiedEmail.Notes},
                }, whereConditions: new Dictionary<string, dynamic>
                {
                    {  "email_id", modifiedEmail.Id }
                });
            }
            foreach (var removedEmail in employee.Emails.RemovedItems)
            {
                await _databaseService.UpdateRowAsync(tableName: "email", columnValuePairs: new Dictionary<string, dynamic>
                {
                    { "id_deleted", 1 }
                }, whereConditions: new Dictionary<string, dynamic>
                {
                    { "email_id", removedEmail.Id }
                });
            }
        }

        private async Task<List<Email>> GetEmailsByEmployeeIdAsync(int employeeId)
        {
            var emailRows = await _databaseService.SelectRowsAsync(tableName: "email", whereConditions: new Dictionary<string, dynamic>
            {
                {"employee_id", employeeId},
                {"is_deleted", 0}
            });

            var emails = new List<Email>();
            foreach (var row in emailRows)
            {
                emails.Add(new Email
                {
                    Id = row["email_id"],
                    EmailAddress = row["email_address"],
                    Notes = row["notes"]
                });
            }
            return emails;
        }

        private async Task<Address> GetAddressByEmployeeIdAsyc(int employeeId)
        {
            var addressRows = await _databaseService.SelectRowsAsync(tableName: "address", whereConditions: new Dictionary<string, dynamic>
            {
                {"employee_id", employeeId}
            });

            return addressRows.Count == 0 ? new Address() : new Address
            {
                Id = addressRows[0]["address_id"],
                Number = addressRows[0]["number"],
                Street = addressRows[0]["street"],
                City = addressRows[0]["city"]
            };
        }
    }
}
