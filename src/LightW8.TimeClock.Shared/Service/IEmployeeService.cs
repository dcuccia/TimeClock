using System.Collections.Generic;
using System.Threading.Tasks;
using LightW8.TimeClock.Shared.Model;

namespace LightW8.TimeClock.Shared.Service
{
    public interface IEmployeeService
    {
        Task<Employee> GetEmployeeByIdAsync(string id);
        Task<bool> TryAddEmployeeAsync(Employee e);
        Task<bool> TryRemoveEmployeeAsync(Employee e);
        Task<bool> TryUpdateEmployeeAsync(Employee e);
        IAsyncEnumerable<Employee> GetEmployeesAsync();
        IAsyncEnumerable<Employee> GetEmployeeReportsAsync(Employee e);
        IAsyncEnumerable<Employee> SearchEmployeesAsync(Employee e);
    }
}