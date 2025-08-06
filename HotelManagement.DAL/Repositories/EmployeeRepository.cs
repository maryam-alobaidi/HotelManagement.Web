using HotelManagement.DAL.Repositories;
using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.DAL.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;


namespace HotelManagement.Infrastructure.DAL.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {

        private readonly string _connectionString;
        private readonly ILogger<EmployeeRepository> _logger;
        public EmployeeRepository(string connectionString,    ILogger<EmployeeRepository> logger)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

            _connectionString = connectionString;
            _logger = logger;

        }


        public async Task<int?> AddAsync(Employee employee)
        {
            using (SqlCommand command = new SqlCommand("Sp_AddNewEmployees"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                command.Parameters.AddWithValue("@LastName", employee.LastName);
                command.Parameters.AddWithValue("@Username", employee.Username);
                command.Parameters.AddWithValue("@PasswordHash", employee.PasswordHash);
                command.Parameters.AddWithValue("@Role", employee.Role);
                command.Parameters.AddWithValue("@HireDate", employee.HireDate);
                // Assuming EmployeeID is an output parameter

                command.Parameters.AddWithValue("@EmployeeID", SqlDbType.Int).Direction = ParameterDirection.Output;
                return await PrimaryFunctions.AddAsync(command, _connectionString, "@EmployeeID");
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (SqlCommand command = new SqlCommand("Sp_DeleteEmployees"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@EmployeeID", id);
              return  await PrimaryFunctions.DeleteAsync(command, _connectionString);
            }
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            List<Employee> employees = new List<Employee>();
            using (SqlCommand command = new SqlCommand("Sp_GetAllEmployees"))
            {

                command.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString);
                while (reader.Read())
                {
                    employees.Add(MapToEmployee(reader));
                }

            }
            return employees;
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID must be greater than zero.", nameof(id));
            }
            using (SqlCommand command = new SqlCommand("Sp_GetEmployeesByID"))
            {
                command.Parameters.AddWithValue("@ID", id);
                command.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                {
                    if (await reader.ReadAsync())
                    {
                        return MapToEmployee(reader);
                    }
                    else
                    {
                        _logger.LogWarning("No employee found with ID {Id}", id);
                        return null;
                    }
                }
            }
        }

        public async Task<bool> UpdateAsync(Employee employee)
        {
            using (SqlCommand command = new SqlCommand("Sp_UpdateEmployees"))
            {
                command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);
                command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                command.Parameters.AddWithValue("@LastName", employee.LastName);
                command.Parameters.AddWithValue("@Username", employee.Username);
                command.Parameters.AddWithValue("@PasswordHash", employee.PasswordHash);
                command.Parameters.AddWithValue("@Role", employee.Role);
                command.Parameters.AddWithValue("@HireDate", employee.HireDate);


                command.CommandType = CommandType.StoredProcedure;
                try
                {
                    await PrimaryFunctions.UpdateAsync(command, _connectionString);

                    return true;
                }
                catch (SqlException sqlEx)
                {

                    if (sqlEx.Message.Contains("No Employee ID found with the provided ID to update.") ||
                             sqlEx.Message.Contains("No records found to update for the provided ID."))
                    {
                        _logger.LogWarning("Update failed: {Message}", sqlEx.Message);
                        return false;
                    }

                    throw;
                }
            }
        }


        private Employee MapToEmployee(SqlDataReader reader)
        {
            return new Employee(
                employeeID: reader.GetInt32(reader.GetOrdinal(nameof(Employee.EmployeeID))),
                firstName: reader.GetString(reader.GetOrdinal(nameof(Employee.FirstName))),
                lastName: reader.GetString(reader.GetOrdinal(nameof(Employee.LastName))),
                username: reader.GetString(reader.GetOrdinal(nameof(Employee.Username))),
                passwordHash: reader.GetString(reader.GetOrdinal(nameof(Employee.PasswordHash))),
                role: reader.GetString(reader.GetOrdinal(nameof(Employee.Role))),
                hireDate: reader.GetDateTime(reader.GetOrdinal(nameof(Employee.HireDate)))
            );
        }
    }
}
