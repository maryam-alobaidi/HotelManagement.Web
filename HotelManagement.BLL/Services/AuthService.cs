using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;


namespace HotelManagement.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly ICustomerService _customerService;
        private readonly IEmployeeService _employeeService;
        private readonly IPasswordHasherService _passwordHasherService;

        public AuthService(ICustomerService customerService, IEmployeeService employeeService, IPasswordHasherService passwordHasherService)
        {
            _customerService = customerService;
            _employeeService = employeeService;
            _passwordHasherService =passwordHasherService;
        }

        private bool IsValidCustomer(Customer customer, string password)
        {
            return _passwordHasherService.VerifyPassword(password, customer.PasswordHash);
        }

        public async Task<Customer?> LoginCustomer(string email,string password)
        {
            var customer= await _customerService.GetCustomerByEmailAsync(email);
            if (customer != null && IsValidCustomer(customer, password))
                return customer;
            return null;
        }

        private bool IsValidEmployee(Employee employee, string password, string role)
        {
            return _passwordHasherService.VerifyPassword(password, employee.PasswordHash) && employee.Role == role;
        }

        public async Task<Employee?> LoginEmployee(string username, string password, string role)
        {
            var employee = await _employeeService.GetEmployeeByUsernameAsync(username);
            if (employee != null && IsValidEmployee(employee, password, role))
                return employee;
            return null;
        }

      
    }
}
