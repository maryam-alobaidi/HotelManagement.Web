﻿using HotelManagement.DAL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.DAL.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;


namespace HotelManagement.DAL.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<CustomerRepository> _logger;
        public CustomerRepository(string connectionString, ILogger<CustomerRepository> logger)
        { 
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

            _connectionString = connectionString;
            _logger = logger;

        }

        public async Task<int?> AddAsync(Customer customer)
        {
            using (SqlCommand command = new SqlCommand("Sp_AddNewCustomer"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Firstname", customer. Firstname);
                command.Parameters.AddWithValue("@Lastname", customer. Lastname);
                command.Parameters.AddWithValue("@Email", customer. Email);
                command.Parameters.AddWithValue("@PhoneNumber", customer. PhoneNumber);
                command.Parameters.AddWithValue("@Address", (object)customer.Address ?? DBNull.Value);
                command.Parameters.AddWithValue("@Nationality", customer. Nationality);
                command.Parameters.AddWithValue("@IDNumber", customer. IDNumber);
                command.Parameters.AddWithValue("@CustomerID", SqlDbType.Int).Direction = ParameterDirection.Output;
                try
                {
                    return await PrimaryFunctions.AddAsync(command, _connectionString, "@CustomerID");
                }
                catch (SqlException ex)
                {
                    // تسجيل الخطأ هنا باستخدام ILogger
                    _logger.LogError(ex, "SQL Error adding customer. Command: {CommandText}, Customer Email: {Email}", command.CommandText, customer.Email);
                    throw; // إعادة رمي الاستثناء ليتم التعامل معه في طبقة الخدمة أو المتحكم
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "General Error adding customer. Customer Email: {Email}", customer.Email);
                    throw;
                }
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (SqlCommand command = new SqlCommand("Sp_DeleteCustomers"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CustomerID", id);
                try
                {
                    return await PrimaryFunctions.DeleteAsync(command, _connectionString);
                }
                catch (SqlException ex)
                {
                    // تسجيل الخطأ هنا باستخدام ILogger
                    _logger.LogError(ex, "SQL Error deleting customer with ID: {CustomerID}", id);
                    throw; // إعادة رمي الاستثناء ليتم التعامل معه في طبقة الخدمة أو المتحكم
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "General Error deleting customer with ID: {CustomerID}", id);
                    throw;
                }
            }
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            List<Customer> customers = new List<Customer>();
            using (SqlCommand command = new SqlCommand("Sp_GetAllCustomers"))
            {

                command.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader= await PrimaryFunctions.GetAsync(command, _connectionString);
                while (reader.Read())
                {
                 customers.Add(MapToCustomer(reader));
                }
                // CommandBehavior.CloseConnection ->> closes the connection when the reader is closed.
            }
            return customers;
        }

        public async  Task<Customer> GetByEmailAsync(string email)
        {
            Customer? customer = null;
            using (SqlCommand command = new SqlCommand("Sp_GetCustomersByEmail"))
            {
                command.Parameters.AddWithValue("@Email", email);
                command.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                {     if (await reader.ReadAsync())
                    {
                        customer = MapToCustomer(reader);
                    }
                }
            }
            return customer;
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID must be greater than zero.", nameof(id));
            using (SqlCommand command = new SqlCommand("Sp_GetCustomersByID"))
            {
                command.Parameters.AddWithValue("@ID", id);
                command.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                {
                    if (await reader.ReadAsync())
                    {
                       return MapToCustomer(reader);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
         
        }

        public async Task<bool> UpdateAsync(Customer customer)
        {
            using (SqlCommand command = new SqlCommand("Sp_UpdateCustomers"))
            {
                command.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                command.Parameters.AddWithValue("@Firstname", customer.Firstname);
                command.Parameters.AddWithValue("@Lastname", customer.Lastname);
                command.Parameters.AddWithValue("@Email", customer.Email);
                command.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);
                command.Parameters.AddWithValue("@Address", (object)customer.Address ?? DBNull.Value);
                command.Parameters.AddWithValue("@Nationality", customer.Nationality);
                command.Parameters.AddWithValue("@IDNumber", customer.IDNumber);

                command.CommandType = CommandType.StoredProcedure;
                try
                {
                    await PrimaryFunctions.UpdateAsync(command, _connectionString);

                    return true;
                }
                catch (SqlException sqlEx)
                {
                   _logger.LogError(sqlEx, "SQL Error updating customer with ID: {CustomerID}", customer.CustomerID);
                    throw; // إعادة رمي الاستثناء ليتم التعامل معه في طبقة الخدمة أو المتحكم
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "General Error updating customer with ID: {CustomerID}", customer.CustomerID);
                    throw;
                }
            }
          
        }

        private Customer MapToCustomer(SqlDataReader reader)
        {
            
            return new Customer(
                customerID: reader.GetInt32(reader.GetOrdinal(nameof(Customer.CustomerID))),
                firstname: reader.GetString(reader.GetOrdinal(nameof(Customer.Firstname))),
                lastname: reader.GetString(reader.GetOrdinal(nameof(Customer.Lastname))),
                email: reader.GetString(reader.GetOrdinal(nameof(Customer.Email))),
                phoneNumber: reader.GetString(reader.GetOrdinal(nameof(Customer.PhoneNumber))),
                address: reader.IsDBNull(reader.GetOrdinal(nameof(Customer.Address))) ? null : reader.GetString(reader.GetOrdinal(nameof(Customer.Address))),
                nationality: reader.GetString(reader.GetOrdinal(nameof(Customer.Nationality))),
                iDNumber: reader.GetString(reader.GetOrdinal(nameof(Customer.IDNumber)))
            );
        }
    }
}
