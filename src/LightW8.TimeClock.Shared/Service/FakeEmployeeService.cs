using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightW8.TimeClock.Shared.Model;

namespace LightW8.TimeClock.Shared.Service
{
    public static class IEnumerableExtensions { public static bool All(this IEnumerable<bool> items) { return items.All(item => item); } }

    public class FakeEmployeeService : IEmployeeService
    {
        private bool _isInitialized;
        private Company _company;
        private List<Employee> _employees;

        public FakeEmployeeService(CompanyNameResolver companyNameResolver)
        {
            _isInitialized = false;
            _company = new Company()
            {
                Name = companyNameResolver.GetCompanyName(),
                EmployeeIds = new List<string>()
            };
            _employees = new List<Employee>();
        }

        public async Task<bool> TryAddEmployeeAsync(Employee employee)
        {
            await InitIfNecessaryAsync();

            if (employee == null || employee.Id != null)
                return false;

            // create a new unique id for this employee based on first name, last name, and DOB
            // how to tell Cosmos to auto-gen uniqueness based on these fields?
            employee.Id = Employee.GetUniqueIdString(employee);

            if(_employees.Any(e => e.Id == employee.Id))
                return false;

            _employees.Add(employee);
            _company.EmployeeIds.Add(employee.Id);

            return true;
        }

        public async Task<bool> TryUpdateEmployeeAsync(Employee employee)
        {
            await InitIfNecessaryAsync();

            if (employee == null || employee.Id == null)
                return false;

            var e = _employees.Where(e => e.Id == employee.Id).FirstOrDefault();

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

            if (e == null || !_employees.Contains(e))
                return false;

            _company.EmployeeIds.Remove(e.Id);
            return _employees.Remove(e);
        }

        public async Task<bool> TryAddReportsAsync(Employee manager, IEnumerable<Employee> reports)
        {
            if (manager == null || !manager.IsManager || reports == null || reports.Count() == 0 || 
                reports.Any(report => report == null || report.Id == manager.Id || manager.ReportIds.Contains(report.Id)))
                return false;

            bool TryAddReport(Employee report)
            {
                manager.ReportIds.Add(report.Id);
                return true;
            }

            return await Task.FromResult(reports.Select(r => TryAddReport(r)).All());
        }

        public async IAsyncEnumerable<Employee> GetEmployeesAsync()
        {
            await InitIfNecessaryAsync();

            var employees = _employees.OrderBy(e => e.LastName).ToArray();

            foreach (Employee employee in employees)
            {
                yield return await Task.FromResult(employee);
            }
        }

        public async IAsyncEnumerable<Employee> GetEmployeeReportsAsync(Employee employee)
        {
            await InitIfNecessaryAsync();

            if (employee == null || employee.IsManager)
                yield break;

            var reports = employee.ReportIds
                .Select(rid => _employees.Where(e => e.Id == rid).FirstOrDefault())
                .Distinct()
                .OrderBy(r => r.LastName).ToArray();

            foreach (Employee report in reports)
            {
                yield return await Task.FromResult(report);
            }
        }

        public async IAsyncEnumerable<Employee> SearchEmployeesAsync(Employee employee)
        {
            await InitIfNecessaryAsync();

            if (employee == null)
                yield break;

            var subqueries = new (string prop, string val, Predicate<Employee> func)[]{
                (nameof(Employee.FirstName), employee.FirstName, e => employee?.FirstName == e.FirstName ),
                (nameof(Employee.MiddleName), employee.MiddleName, e => employee?.MiddleName == e.MiddleName ),
                (nameof(Employee.LastName), employee.LastName, e => employee?.LastName == e.LastName )}
                .Where(tuple => !string.IsNullOrWhiteSpace(tuple.val))
                .ToArray();

            if (subqueries.Length == 0)
                yield break;

            var foundEmployees = _employees.Where(e => subqueries.First().func(e));
            foreach (var subquery in subqueries.Skip(1))
            {
                foundEmployees = foundEmployees.Where(e => subquery.func(e));
            }

            foundEmployees = foundEmployees.ToArray();

            foreach (Employee foundEmployee in foundEmployees)
            {
                yield return await Task.FromResult(foundEmployee);
            }
        }

        public async Task<Employee> GetEmployeeByIdAsync(string id)
        {
            await InitIfNecessaryAsync();

            return _employees.Where(e => e.Id == id).FirstOrDefault();
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
