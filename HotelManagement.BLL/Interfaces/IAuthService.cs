using HotelManagement.Domain.Entities;


namespace HotelManagement.BLL.Interfaces
{
    public interface IAuthService
    {
      Task<Employee?> LoginEmployee(string username, string password, string role);
      Task<Customer?> LoginCustomer(string email, string password);

    }
}
