using System.Collections.Generic;
using System.Threading.Tasks;
using LightW8.TimeClock.Business.Model;

namespace LightW8.TimeClock.Business.Service
{
    public interface IEmployeeService
    {
        IAsyncEnumerable<Employee> GetEmployeesAsync();
        Task<Employee> GetEmployeeByIdAsync(string id);
        Task<bool> TryAddEmployeeAsync(Employee e);
        Task<bool> TryRemoveEmployeeAsync(Employee e);
        Task<bool> TryUpdateEmployeeAsync(Employee e);
    }
}