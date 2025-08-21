using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.DAL.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;


namespace HotelManagement.Infrastructure.DAL.Repositories
{
    public class PaymentMethodsRepository : IPaymentMethodsRepository
    {
        private readonly string _connectionString;

        public PaymentMethodsRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null.");
        }

        public async Task<int?> AddAsync(PaymentMethod paymentMethod)
        {
            using (SqlCommand command = new SqlCommand("Sp_AddNewPaymentMethods"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@MethodName", paymentMethod.MethodName);
                command.Parameters.AddWithValue("@Description", paymentMethod.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@IsActive", paymentMethod.IsActive);
               
                command.Parameters.AddWithValue("@MethodID", SqlDbType.Int).Direction = ParameterDirection.Output;

                return await PrimaryFunctions.AddAsync(command, _connectionString, "@MethodID");
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (SqlCommand command = new SqlCommand("Sp_DeletePaymentMethods"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@MethodID", id);
                return await PrimaryFunctions.DeleteAsync(command, _connectionString);
            }
        }

        public async Task<IEnumerable<PaymentMethod>> GetAllAsync()
        {
            List<PaymentMethod> paymentMethods = new List<PaymentMethod>();
            using (SqlCommand command = new SqlCommand("Sp_GetAllPaymentMethods"))
            {
                command.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString);
                while (reader.Read())
                {
                    paymentMethods.Add(MapToPaymentMethods(reader));
                }
            }
            return paymentMethods;
        }

        private PaymentMethod MapToPaymentMethods(SqlDataReader reader)
        {
            return new PaymentMethod
            (
                methodID: reader.GetInt32(reader.GetOrdinal(nameof(PaymentMethod.MethodID))),
                methodName: reader.GetString(reader.GetOrdinal(nameof(PaymentMethod.MethodName))),
                description: reader.IsDBNull(reader.GetOrdinal(nameof(PaymentMethod.Description))) ? null : reader.GetString(reader.GetOrdinal(nameof(PaymentMethod.Description))),
                isActive:reader.GetBoolean(reader.GetOrdinal(nameof(PaymentMethod.IsActive)))

            );
        }

        public async Task<PaymentMethod?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID must be greater than zero.", nameof(id));
            }
            using (SqlCommand command = new SqlCommand("Sp_GetPaymentMethodsByID"))
            {
                command.Parameters.AddWithValue("@ID", id);
                command.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                {
                    if (await reader.ReadAsync())
                    {
                        return MapToPaymentMethods(reader);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public async Task<bool> UpdateAsync(PaymentMethod paymentMethod)
        {
            using (SqlCommand command = new SqlCommand("Sp_UpdatePaymentMethods"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@MethodID", paymentMethod.MethodID);
                command.Parameters.AddWithValue("@MethodName", paymentMethod.MethodName);
                command.Parameters.AddWithValue("@Description", (object) paymentMethod.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@IsActive", paymentMethod.IsActive);

                try
                {
                    await PrimaryFunctions.UpdateAsync(command, _connectionString);

                    return true;
                }
                catch (SqlException sqlEx)
                {

                    if (sqlEx.Message.Contains("No Method ID found with the provided ID to update.") ||
                             sqlEx.Message.Contains("No records found to update for the provided ID."))
                    {
                        return false;
                    }

                    throw;
                }
            }
        }
    }
}
