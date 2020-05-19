using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Azure.Cosmos;

namespace LightW8.TimeClock.Business
{
    public class CosmosEmployeeService : IEmployeeService
    {
        private const string DatabaseId = "EmployeeDatabase";
        private const string ContainerId = "EmployeeContainer";

        private CosmosClient _cosmosClient;
        private string _companyName;
        private bool _isInitialized;

        private CosmosContainer _cosmosContainer;

        public CosmosEmployeeService(CosmosClient cosmosClient, string companyName)
        {
            _cosmosClient = cosmosClient;
            _companyName = companyName;
            _isInitialized = false;
        }

        public async Task<bool> TryAddEmployeeAsync(Employee e)
        {
            await InitIfNecessaryAsync();

            // abort if employee already has an Id
            if (e.Id != null)
                return false;

            // create a new unique id for this employee based on first name, last name, and DOB
            // how to tell Cosmos to auto-gen uniqueness based on these fields?
            e.Id = Employee.GetUniqueIdString(e);
            e.Partition = _companyName;

            try
            {
                // Read the item to see if it exists.  
                var employeeExistsResponse = await _cosmosContainer.ReadItemAsync<Employee>(e.Id, new PartitionKey(e.Partition));

                // if no exception thrown, item already exists
                return false;
            }
            catch (CosmosException ex) when (ex.Status == (int)HttpStatusCode.NotFound)
            {
                // otherwise, create item
                ItemResponse<Employee> itemCreatedResponse = await _cosmosContainer.CreateItemAsync<Employee>(e, new PartitionKey(e.Partition));
            }

            return true;
        }

        public async Task<bool> TryUpdateEmployeeAsync(Employee e)
        {
            await InitIfNecessaryAsync();

            // abort if employee does not already have an Id
            if (e == null || e.Id == null)
                return false;

            try
            {
                // Read the item to see if it exists.  
                var employeeExistsResponse = await _cosmosContainer.ReadItemAsync<Employee>(e.Id, new PartitionKey(e.Partition));

                // Recommended (?) approach: use ReplaceItemAsync to add the employee
                var employeeUpdatedResponse = await _cosmosContainer.ReplaceItemAsync<Employee>(e, e.Id, new PartitionKey(e.Partition));

                // alternative usage 1:
                //var employeeUpdatedResponse = await container.UpsertItemAsync<Employee>(e, new PartitionKey(e.Partition));

                // alternative usage 2 (mutate received object):
                //Employee oldEmployee = employeeExistsResponse;
                //oldEmployee.FirstName = newEmployee.FirstName;
                //oldEmployee.MiddleName = newEmployee.MiddleName;
                //oldEmployee.LastName = newEmployee.LastName;
                //oldEmployee.DateOfBirth = newEmployee.DateOfBirth;
                //oldEmployee.IsManager = newEmployee.IsManager;
                //var employeeUpdatedResponse = await container.UpsertItemAsync<Employee>(oldEmployee, new PartitionKey(e.Partition));
            }
            catch (CosmosException ex) when (ex.Status == (int)HttpStatusCode.NotFound)
            {
                // if item doesn't exist, or update fails return false
                return false;
            }

            return true;
        }

        public async Task<bool> TryRemoveEmployeeAsync(Employee e)
        {
            await InitIfNecessaryAsync();

            try
            {
                // Read the item to see if it exists.  
                var employeeExistsResponse = await _cosmosContainer.ReadItemAsync<Employee>(e.Id, new PartitionKey(e.Partition));

                var employeeRemovedResponse = await _cosmosContainer.DeleteItemAsync<Employee>(e.Id, new PartitionKey(e.Partition));
            }
            catch (CosmosException ex) // when (ex.Status == (int)HttpStatusCode.Conflict) // 409
            {
                // if item doesn't exist, or remove fails return false
                return false;
            }

            return true;
        }

        private async IAsyncEnumerable<Employee> QueryItemsAsync(Employee e)
        {
            await InitIfNecessaryAsync();

            var sqlQueryText = $"SELECT * FROM e WHERE e.{nameof(e.FirstName)} = '{e.FirstName}'"; // expand queries

            var queryDefinition = new QueryDefinition(sqlQueryText);

            await foreach (Employee employee in _cosmosContainer.GetItemQueryIterator<Employee>(queryDefinition))
            {
                yield return employee;
            }
        }

        public async IAsyncEnumerable<Employee> GetEmployeesAsync()
        {
            await InitIfNecessaryAsync();

            var sqlQueryText = $"SELECT * FROM e ORDER BY LOWER(e.{nameof(Employee.LastName)})";

            var queryDefinition = new QueryDefinition(sqlQueryText);

            await foreach (Employee employee in _cosmosContainer.GetItemQueryIterator<Employee>(queryDefinition))
            {
                yield return employee;
            }
        }

        private async Task InitIfNecessaryAsync()
        {
            if (_isInitialized)
                return;

            CosmosDatabase database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId);

            _cosmosContainer = await database.CreateContainerIfNotExistsAsync(ContainerId, "/Partition");

            _isInitialized = true;

            await AddItemsToContainerAsync();
        }

        /// <summary>
        /// Add Employee items to the container
        /// </summary>
        private async Task AddItemsToContainerAsync()
        {
            // Create a new company object for the NewCo company
            var newCompany = new Company()
            {
                Name = _companyName,
                Employees = new List<Employee>
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
                }
            };

            foreach (var employee in newCompany.Employees)
            {
                await TryAddEmployeeAsync(employee);
            }
        }
    }
}
