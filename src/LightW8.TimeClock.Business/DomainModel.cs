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

    public static class EmployeeExtensions
    {
        // create a duplicate (shallow-copy reports, skip Id)
        public static Employee Clone(this Employee e) => new Employee
        {
            FirstName = e.FirstName,
            MiddleName = e.MiddleName,
            LastName = e.LastName,
            DateOfBirth = e.DateOfBirth,
            IsManager = e.IsManager,
            ReportIds = e.ReportIds,
        };

        // set values based on resetValues instance (shallow-copy reports, skip Id)
        public static void Reset(this Employee e, Employee resetValues)
        {
            if (resetValues == null)
                return;

            e.FirstName = resetValues.FirstName;
            e.MiddleName = resetValues.MiddleName;
            e.LastName = resetValues.LastName;
            e.DateOfBirth = resetValues.DateOfBirth;
            e.IsManager = resetValues.IsManager;
            e.ReportIds = resetValues.ReportIds;
        }
    }

    public class Employee
    {
        public static string GetUniqueIdString(Employee e) => $"{e.LastName},{e.FirstName};{e.DateOfBirth.ToString("yyyy-MM-dd")}";

        [JsonPropertyName("id")]
        public string Id { get; set; }
        public string Partition { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsManager { get; set; } = false;
        public IList<string> ReportIds { get; set;}

        public override bool Equals(object obj) => obj is Employee e && (GetUniqueIdString(e) == GetUniqueIdString(this));

        public override string ToString() => JsonSerializer.Serialize(this);

    }

    public class ReportItems
    {
        [JsonPropertyName("id")]
        public string ManagerId { get; set; }
    }
}
