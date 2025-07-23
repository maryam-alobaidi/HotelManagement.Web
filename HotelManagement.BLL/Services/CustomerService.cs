using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.DAL.Interfaces;

public class CustomerService : ICustomerService
{

    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<int?> AddCustomerAsync(Customer customer)
    {
        Customer existingCustomer = await  _customerRepository.GetByEmailAsync(customer.Email);

        if(existingCustomer != null)
        {
            throw new InvalidOperationException("Customer with this email already exists.");
        }

      return  await _customerRepository.AddAsync(customer);
    }
   
    public async Task<bool> DeleteCustomerAsync(int id)
    {
        Customer customerToDelete = await _customerRepository.GetByIdAsync(id);

        if(customerToDelete == null)
        {
            throw new KeyNotFoundException($"Customer with ID {id} not found.");
        }
        try
        {
         return   await _customerRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete customer with ID {id}.", ex);
        }
    }

    public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
    {
       return await _customerRepository.GetAllAsync();
    }

    public async Task<Customer> GetCustomerByIdAsync(int id)
    {
        Customer customerToFound = await _customerRepository.GetByIdAsync(id);
        
        if(customerToFound == null)
        {
            throw new KeyNotFoundException($"Customer with ID {id} not found.");
        }

        return customerToFound;
    }

    public async Task<bool> UpdateCustomerAsync(Customer customer)
    {
        Customer existingCustomer = await _customerRepository.GetByIdAsync(customer.CustomerID);

        if (existingCustomer == null)
        {
            throw new InvalidOperationException($"Customer with ID {customer.CustomerID} not found for update.");
        }

        try
        {
         return   await _customerRepository.UpdateAsync(customer);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update customer with ID {customer.CustomerID}.", ex);
        }
      
    }
}

