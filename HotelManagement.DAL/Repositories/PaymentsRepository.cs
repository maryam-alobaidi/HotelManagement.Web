using HotelManagement.Domain.Entities;
using HotelManagement.Domain.Enums;
using HotelManagement.Infrastructure.DAL.DTOs;
using HotelManagement.Infrastructure.DAL.Interfaces;
using Microsoft.Data.SqlClient;

using System.Data;


namespace HotelManagement.Infrastructure.DAL.Repositories
{
    public class PaymentsRepository : IPaymentsRepository
    {
        private readonly string _connectionString;

        public PaymentsRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null.");
        }

        public async Task<int?> AddAsync(Payment payment)
        {
            using (SqlCommand command = new SqlCommand("Sp_AddNewPayments"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@InvoiceID", payment.InvoiceID);
                command.Parameters.AddWithValue("@Amount", payment.Amount);
                command.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PaymentMethodID", payment.PaymentMethodID);
                command.Parameters.AddWithValue("@TransactionReference", payment.TransactionReference ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@RecordedByEmployeeID", payment.RecordedByEmployeeID.HasValue ? (object)payment.RecordedByEmployeeID.Value : DBNull.Value);

                command.Parameters.AddWithValue("@PaymentID", SqlDbType.Int).Direction = ParameterDirection.Output;

                return await PrimaryFunctions.AddAsync(command, _connectionString, "@PaymentID");
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (SqlCommand command = new SqlCommand("Sp_DeletePayments"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PaymentID", id);
                return await PrimaryFunctions.DeleteAsync(command, _connectionString);
            }
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            List<Payment> payments = new List<Payment>();
            using (SqlCommand command = new SqlCommand("Sp_GetAllPayments"))
            {

                command.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString);
                while (reader.Read())
                {
                    payments.Add(MapToPayment(reader));
                }

            }
            return payments;
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID must be greater than zero.", nameof(id));
            }
            using (SqlCommand command = new SqlCommand("Sp_GetPaymentsByID"))
            {
                command.Parameters.AddWithValue("@ID", id);
                command.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                {
                    if (await reader.ReadAsync())
                    {
                        return MapToPayment(reader);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        private Payment MapToPayment(SqlDataReader reader)
        {
            return new Payment(

                paymentID: reader.GetInt32(reader.GetOrdinal(nameof(Payment.PaymentID))),
                invoiceID: reader.GetInt32(reader.GetOrdinal(nameof(Payment.InvoiceID))),
                amount: reader.GetDecimal(reader.GetOrdinal(nameof(Payment.Amount))),
                paymentDate: reader.IsDBNull(reader.GetOrdinal(nameof(Payment.PaymentDate))) ? null : reader.GetDateTime(reader.GetOrdinal(nameof(Payment.PaymentDate))),
                paymentMethodID: reader.GetInt32(reader.GetOrdinal(nameof(Payment.PaymentMethodID))),
                transactionReference: reader.IsDBNull(reader.GetOrdinal(nameof(Payment.TransactionReference))) ? null : reader.GetString(reader.GetOrdinal(nameof(Payment.TransactionReference))),
                recordedByEmployeeID: reader.IsDBNull(reader.GetOrdinal(nameof(Payment.RecordedByEmployeeID))) ? null : reader.GetInt32(reader.GetOrdinal(nameof(Payment.RecordedByEmployeeID)))
              );

        }

        public async Task<bool> UpdateAsync(Payment payment)
        {
            using (SqlCommand command = new SqlCommand("Sp_UpdatePayments"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PaymentID", payment.PaymentID);
                command.Parameters.AddWithValue("@InvoiceID", payment.InvoiceID);
                command.Parameters.AddWithValue("@Amount", payment.Amount);
                command.Parameters.AddWithValue("@PaymentDate", (object)payment.PaymentDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@PaymentMethodID", payment.PaymentMethodID);
                command.Parameters.AddWithValue("@TransactionReference", (object)payment.TransactionReference ?? DBNull.Value);
                command.Parameters.AddWithValue("@RecordedByEmployeeID", (object)payment.RecordedByEmployeeID ?? DBNull.Value);


                try
                {
                    await PrimaryFunctions.UpdateAsync(command, _connectionString);

                    return true;
                }
                catch (SqlException sqlEx)
                {

                    if (sqlEx.Message.Contains("No Payment ID found with the provided ID to update.") ||
                             sqlEx.Message.Contains("No records found to update for the provided ID."))
                    {
                        return false;
                    }

                    throw;
                }
            }
        }



        public async Task<decimal> GetTotalPaidForBookingAsync(int bookingId)
        {
            using (SqlCommand command = new SqlCommand("SP_GetTotalPaidForBooking"))
            {
                command.Parameters.AddWithValue("@BookingID", bookingId);
                command.CommandType = System.Data.CommandType.StoredProcedure;


                SqlParameter outputParam = command.Parameters.Add("@TotalPaidAmount", SqlDbType.Decimal); //output parametar
                outputParam.Direction = ParameterDirection.Output;


                return await PrimaryFunctions.GetTotalPaidForBookingAsync(command, _connectionString);

            }
        }

        public async Task<IEnumerable<PaymentDetailsDTO>> GetPaymentDetailsDTOsAsync()
        {
            var payments = new List<PaymentDetailsDTO>();
            using (var command = new SqlCommand("GetPaymentDetails", new SqlConnection(_connectionString)))
            {
                command.CommandType = CommandType.StoredProcedure;
                await command.Connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    while (await reader.ReadAsync())
                    {
                        payments.Add(mapToPaymentDetailsDTO(reader));
                    }
                }
            }
            return payments;
        }

        public PaymentDetailsDTO mapToPaymentDetailsDTO(SqlDataReader reader)
        {
            // Get column ordinals once to avoid repeated lookups
            int paymentIdOrdinal = reader.GetOrdinal("PaymentID");
            int invoiceIdOrdinal = reader.GetOrdinal("InvoiceID");
            int amountOrdinal = reader.GetOrdinal("Amount");
            int paymentDateOrdinal = reader.GetOrdinal("PaymentDate");
            int paymentMethodIdOrdinal = reader.GetOrdinal("PaymentMethodID");
            int recordedByEmployeeIdOrdinal = reader.GetOrdinal("RecordedByEmployeeID");
            int transactionRefOrdinal = reader.GetOrdinal("TransactionReference");
            int usernameOrdinal = reader.GetOrdinal("Username");
            int invoiceStatusOrdinal = reader.GetOrdinal("InvoiceStatus");
            int paymentMethodNameOrdinal = reader.GetOrdinal("PaymentMethodName");

            // Create a new DTO object and populate it safely
            return new PaymentDetailsDTO
            {
                PaymentID = reader.GetInt32(paymentIdOrdinal),
                InvoiceID = reader.GetInt32(invoiceIdOrdinal),
                Amount = reader.GetDecimal(amountOrdinal),
                PaymentDate = reader.GetDateTime(paymentDateOrdinal),
                PaymentMethodID = reader.GetInt32(paymentMethodIdOrdinal),
                RecordedByEmployeeID = reader.GetInt32(recordedByEmployeeIdOrdinal),

                // Use the IsDBNull() method to safely get string values.
                // If the value is DBNull, assign a default string (e.g., an empty string).
                TransactionReference = reader.IsDBNull(transactionRefOrdinal)
                    ? string.Empty
                    : reader.GetString(transactionRefOrdinal),

                Username = reader.IsDBNull(usernameOrdinal)
                    ? "N/A"
                    : reader.GetString(usernameOrdinal),

                InvoiceStatus = reader.IsDBNull(invoiceStatusOrdinal)
                    ? InvoiceStatusEnum.Unpaid
                    : (InvoiceStatusEnum)reader.GetInt32(invoiceStatusOrdinal),

                PaymentMethodName = reader.IsDBNull(paymentMethodNameOrdinal)
                    ? "N/A"
                    : reader.GetString(paymentMethodNameOrdinal)
            };
        }

        public async Task<PaymentDetailsDTO?> GetPaymentDetailsByIdAsync(int id)
        {
            PaymentDetailsDTO paymentDetails = new PaymentDetailsDTO();

            using (SqlCommand command = new SqlCommand("Sp_GetPaymentDetailsById"))
            {
                command.Parameters.AddWithValue("@PaymentID", id);
                command.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                {
                    if (await reader.ReadAsync())
                    {
                        return MapToPaymentDetailsWithIsdDTO(reader);
                    }
                    else
                    {
                        return null;
                    }
                }
            }

        }

        public PaymentDetailsDTO MapToPaymentDetailsWithIsdDTO(SqlDataReader reader)
        {
            return new PaymentDetailsDTO
            {
                // Direct mapping for non-nullable values
                PaymentID = reader.GetInt32(reader.GetOrdinal("PaymentID")),
                InvoiceID = reader.GetInt32(reader.GetOrdinal("InvoiceID")),
                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                PaymentDate = reader.GetDateTime(reader.GetOrdinal("PaymentDate")),
                RecordedByEmployeeID = reader.GetInt32(reader.GetOrdinal("RecordedByEmployeeID")),

                // Null-safe mapping for string values
                PaymentMethodName = reader.IsDBNull(reader.GetOrdinal("PaymentMethodName"))
                ? "N/A"
                : reader.GetString(reader.GetOrdinal("PaymentMethodName")),

                TransactionReference = reader.IsDBNull(reader.GetOrdinal("TransactionReference"))
                ? "N/A"
                : reader.GetString(reader.GetOrdinal("TransactionReference")),

                Username = reader.IsDBNull(reader.GetOrdinal("EmployeeName"))
                ? "N/A"
                : reader.GetString(reader.GetOrdinal("EmployeeName")),

                InvoiceStatus = reader.IsDBNull("InvoiceStatus")
                    ? InvoiceStatusEnum.Unpaid
                    : (InvoiceStatusEnum)reader.GetInt32("InvoiceStatus")
            };
        }
    
    }
}
