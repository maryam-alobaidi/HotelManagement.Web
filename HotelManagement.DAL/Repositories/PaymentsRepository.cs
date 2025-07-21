using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.DAL.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                command.Parameters.AddWithValue("@InvoiceID",payment. InvoiceID);
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
              return  await PrimaryFunctions.DeleteAsync(command, _connectionString);
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

                paymentID:reader.GetInt32(reader.GetOrdinal(nameof(Payment.PaymentID))),
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
                command.Parameters.AddWithValue("@PaymentDate", (object)payment.PaymentDate??DBNull.Value);
                command.Parameters.AddWithValue("@PaymentMethodID",payment.PaymentMethodID);
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

    }
}
