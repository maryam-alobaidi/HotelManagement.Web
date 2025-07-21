using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelManagement.Domain.Entities;

namespace HotelManagement.DAL.Interfaces
{
    public interface ICustomerRepository
    {
        Task<int?> AddAsync(Customer customer);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(Customer customer);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task <Customer> GetByIdAsync(int id);
        Task<Customer> GetByEmailAsync(string email);

    }
}
