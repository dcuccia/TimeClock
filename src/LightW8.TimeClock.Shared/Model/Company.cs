using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LightW8.TimeClock.Shared.Model
{
    public class Company
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<string> EmployeeIds { get; set; }

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
}
