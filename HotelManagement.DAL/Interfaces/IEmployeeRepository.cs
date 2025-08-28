using HotelManagement.Domain.Entities;

namespace HotelManagement.Infrastructure.DAL.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<int?> AddAsync(Employee employee);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(Employee employee);
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee?> GetByUsernameAsync(string username);

    }
}
