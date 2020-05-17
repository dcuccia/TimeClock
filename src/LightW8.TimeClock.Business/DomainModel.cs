using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LightW8.TimeClock.Business
{
    public class Company
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<Employee> Employees { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }

    public class Employee
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsManager { get; set; } = false;
        public IList<Employee> Reports { get; set; }

        public bool TryAddReport(Employee employee)
        {
            if (!IsManager || employee == null || employee == this || Reports.Contains(employee)) // can't add self
                return false;

            Reports.Add(employee);

            return true;
        }

        public override bool Equals(object obj) => obj is Employee e && e.FirstName == FirstName && e.LastName == LastName && e.DateOfBirth == DateOfBirth;

        // create a duplicate (shallow-copy reports)
        public Employee Clone() => new Employee 
        {
            FirstName = FirstName,
            MiddleName = MiddleName,
            LastName = LastName,
            DateOfBirth = DateOfBirth,
            IsManager = IsManager,
            Reports = Reports,
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
            Reports = resetValues.Reports;
        }
    }
}
