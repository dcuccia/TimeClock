using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightW8.TimeClock.Business
{
    public interface IEmployeeService
    {
        IAsyncEnumerable<Employee> GetEmployeesAsync();
        Task<bool> TryAddEmployeeAsync(Employee e);
        Task<bool> TryRemoveEmployeeAsync(Employee e);
        Task<bool> TryUpdateEmployeeAsync(Employee e);
    }
}