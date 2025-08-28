
using HotelManagement.Domain.Entities;

namespace HotelManagement.BLL.Interfaces
{
    public interface ICustomerService
    {



        // Define methods for customer service here
        // For example:
        // Task<Customer> GetByIdAsync(int id);
        // Task<IEnumerable<Customer>> GetAllAsync();
        // Task AddCustomerAsync(Customer customer);
        // Task UpdateCustomerAsync(Customer customer);
        // Task DeleteCustomerAsync(int id);
        // Note: The actual implementation of these methods would be in a class that implements this interface.

        Task<Customer> GetCustomerByIdAsync(int id);
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<int?> AddCustomerAsync(Customer customer);
        Task<bool> UpdateCustomerAsync(Customer customer);
        Task<bool> DeleteCustomerAsync(int id);   

        Task<Customer?> GetCustomerByEmailAsync(string email);

        Task<int> GetTotalCustomersAsync();

    }
}
