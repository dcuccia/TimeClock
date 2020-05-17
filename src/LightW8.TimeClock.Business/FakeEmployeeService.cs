using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LightW8.TimeClock.Business
{
    public static class FakeEmployeeServiceExtensions
    {
        public static IServiceCollection AddFakeEmployeeServiceClient(this IServiceCollection services)
        {
            services.AddSingleton<IEmployeeService, FakeEmployeeService>();
            return services;
        }
    }

    public class FakeEmployeeService : IEmployeeService
    {
        private bool _isInitialized = false;
        private Company _company = new Company()
        {
            Name = "NewCo",
            Employees = new List<Employee>()
        };

        public async Task<bool> TryAddEmployeeAsync(Employee employee)
        {
            await InitIfNecessaryAsync();

            if (employee == null || employee.Id != null)
                return false;

            // create a new unique id for this employee based on first name, last name, and DOB
            // how to tell Cosmos to auto-gen uniqueness based on these fields?
            employee.Id = employee.LastName + ", " + employee.FirstName + ", " + employee.DateOfBirth.ToString("d");

            if(_company.Employees.Any(e => e.Id == employee.Id))
                return false;

            _company.Employees.Add(employee);

            return true;
        }

        public async Task<bool> TryUpdateEmployeeAsync(Employee employee)
        {
            await InitIfNecessaryAsync();

            if (employee == null || employee.Id == null)
                return false;

            var e = _company.Employees.Where(e => e.Id == employee.Id).FirstOrDefault();

            if (e == null)
                return false;

            e.FirstName = employee.FirstName;
            e.MiddleName = employee.MiddleName;
            e.LastName = employee.LastName;
            e.DateOfBirth = employee.DateOfBirth;
            e.IsManager = employee.IsManager;

            return true;
        }

        public async Task<bool> TryRemoveEmployeeAsync(Employee e)
        {
            await InitIfNecessaryAsync();

            if (e == null || !_company.Employees.Contains(e))
                return false;

            return _company.Employees.Remove(e);
        }

        public async IAsyncEnumerable<Employee> GetEmployeesAsync()
        {
            await InitIfNecessaryAsync();

            var employees = _company.Employees.OrderBy(e => e.LastName).ToArray();

            foreach (Employee employee in employees)
            {
                yield return await Task.FromResult(employee);
                //yield return employee;
            }
        }

        private async Task InitIfNecessaryAsync()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            await AddItemsToContainerAsync();
        }

        /// <summary>
        /// Add Employee items to the container
        /// </summary>
        private async Task AddItemsToContainerAsync()
        {
            // Create a new company object for the NewCo company
            var employees = new List<Employee>
            {
                new Employee { FirstName = "Alice", MiddleName = "Applesauce", LastName = "Anselmino", DateOfBirth = new DateTime(1976, 6, 19)},
                new Employee { FirstName = "Bob", MiddleName = "Bontificate", LastName = "Bandorama", DateOfBirth = new DateTime(1979, 6, 19) },
                new Employee { FirstName = "Chris", MiddleName = "Causality", LastName = "Cranstonette", DateOfBirth = new DateTime(1929, 6, 19) },
                new Employee { FirstName = "Hector", MiddleName = "Head", LastName = "Honcho", DateOfBirth = new DateTime(2000, 6, 19), IsManager = true },
                new Employee { FirstName = "Ingrid", MiddleName = "Incrastical", LastName = "Interlocutor", DateOfBirth = new DateTime(1975, 6, 19) },
                new Employee { FirstName = "Jericho", MiddleName = "Jonseyhones", LastName = "Jelmonico", DateOfBirth = new DateTime(0001, 12, 25) },
                new Employee { FirstName = "Mandy", MiddleName = "Managerial", LastName = "Mandator", DateOfBirth = new DateTime(2140, 6, 19), IsManager = true },
                new Employee { FirstName = "Norman", MiddleName = "Netheregion", LastName = "Nederlander", DateOfBirth = new DateTime(1066, 6, 19) },
                new Employee { FirstName = "Yolanda", MiddleName = "Yellowtail", LastName = "Yammerstammer", DateOfBirth = new DateTime(105, 6, 19) },
                new Employee { FirstName = "Zach", MiddleName = "", LastName = "Zebransky", DateOfBirth = new DateTime(1976, 6, 19) }
            };

            foreach (var employee in employees)
            {
                await TryAddEmployeeAsync(employee);
            }
        }
    }
}
