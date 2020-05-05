using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LightW8.TimeClock.Business
{
    public class EmployeeService
    {
        private Company _company = new Company("NewCo")
        {
            Employees = new List<Employee>
            {
                new Employee("Alice", "Applesauce", "Anselmino", new DateTime(1976, 6, 19)),
                new Employee("Bob", "Bontificate", "Bandorama", new DateTime(1979, 6, 19)),
                new Employee("Chris", "Causality", "Cranstonette", new DateTime(1929, 6, 19)),
                new Employee("Hector", "Head", "Honcho", new DateTime(2000, 6, 19)){ IsManager = true },
                new Employee("Ingrid", "Incrastical", "Interlocutor", new DateTime(1975, 6, 19)),
                new Employee("Jericho", "Jonseyhones", "Jelmonico", new DateTime(0001, 12, 25)),
                new Employee("Mandy", "Managerial", "Mandator", new DateTime(2140, 6, 19)){ IsManager = true },
                new Employee("Norman", "Netheregion", "Nederlander", new DateTime(1066, 6, 19)),
                new Employee("Yolanda", "Yellowtail", "Yammerstammer", new DateTime(105, 6, 19)),
                new Employee("Zach", "", "Zebransky", new DateTime(1976, 6, 19))
            }
        };

        public bool TryAddEmployee(Employee e) => _company.TryAddEmployee(e);
        public bool TryUpdateEmployee(Employee oldEmployee, Employee newEmployee) => _company.TryUpdateEmployee(oldEmployee, newEmployee);
        public bool TryRemoveEmployee(Employee e) => _company.TryRemoveEmployee(e);
        public Task<Employee[]> GetEmployeesAsync() => Task.FromResult(_company.Employees.OrderBy(e => e.LastName).ToArray());
    }
}
