using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.DAL.Interfaces;

namespace HotelManagement.BLL.Services
{
    public class EmployeeService : IEmployeeService
    {

        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        }

        public async Task<int?> AddEmployeeAsync(Employee employee)
        {
            if (string.IsNullOrWhiteSpace(employee.FirstName) || string.IsNullOrWhiteSpace(employee.LastName))
            {
                throw new ArgumentException("Employee first name and last name cannot be empty.");
            }

            return await _employeeRepository.AddAsync(employee);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Employee ID must be greater than zero.", nameof(id));
            }
            return    await _employeeRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _employeeRepository.GetAllAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Employee ID must be greater than zero.", nameof(id));
            }
            return await _employeeRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            if (employee.EmployeeID <= 0)
            {
                throw new ArgumentException("Employee ID must be greater than zero.", nameof(employee.EmployeeID));
            }
           return await _employeeRepository.UpdateAsync(employee);
        }
    }
}
