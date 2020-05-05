using System;
using System.Collections.Generic;
using System.Linq;

namespace LightW8.TimeClock.Business
{
    public class Company
    {
        public Company(string name)
        {
            Name = name;
        }

        public bool TryAddEmployee(Employee e)
        {
            if (e == null || Employees.Contains(e))
                return false;

            Employees.Add(e);

            return true;
        }

        public bool TryUpdateEmployee(Employee oldEmployee, Employee newEmployee)
        {
            if (newEmployee == null || !Employees.Contains(oldEmployee))
                return false;

            var e = Employees.Where(e => e == oldEmployee).FirstOrDefault()
                ?? new Employee("", "", "", DateTime.Now.Date);
            
            e.FirstName = newEmployee.FirstName;
            e.MiddleName = newEmployee.MiddleName;
            e.LastName = newEmployee.LastName;
            e.DateOfBirth = newEmployee.DateOfBirth;
            e.IsManager = newEmployee.IsManager;

            return true;
        }

        public bool TryRemoveEmployee(Employee e)
        {
            if (e == null || !Employees.Contains(e))
                return false;

            return Employees.Remove(e);
        }

        public string Name { get; set; }
        public IList<Employee> Employees { get; set; } = new List<Employee>();
    }

    public class Employee
    {
        public Employee(string firstName, string middleName, string lastName, DateTime dateOfBirth)
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
        }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set;}
        public bool IsManager { get; set; } = false;
        public IList<Employee> Reports { get; set; } = new List<Employee>();

        public bool TryAddReport(Employee employee)
        {
            if (!IsManager || employee == null || employee == this || Reports.Contains(employee)) // can't add self
                return false;

            Reports.Add(employee);

            return true;
        }

        public override bool Equals(object obj) => obj is Employee e && e.FirstName == FirstName && e.LastName == LastName && e.DateOfBirth == DateOfBirth;

        public Employee Clone() => 
            new Employee(FirstName, MiddleName, LastName, DateOfBirth)
            {
                Reports = Reports,
                IsManager = IsManager 
            };

        public void Reset(Employee resetValues)
        {
            if(resetValues == null)
                return;

            FirstName = resetValues.FirstName;
            MiddleName = resetValues.MiddleName;
            LastName = resetValues.LastName;
            DateOfBirth = resetValues.DateOfBirth;
            IsManager = resetValues.IsManager;
        }
    }
}
