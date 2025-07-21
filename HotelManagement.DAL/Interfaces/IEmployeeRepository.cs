using HotelManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Infrastructure.DAL.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<int?> AddAsync(Employee employee);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(Employee employee);
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
      
    }
}
