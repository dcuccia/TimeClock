using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LightW8.TimeClock.Business.Model
{
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
}
