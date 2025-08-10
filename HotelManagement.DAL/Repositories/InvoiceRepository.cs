

using HotelManagement.Domain.Entities;
using HotelManagement.Domain.Enums;
using HotelManagement.Infrastructure.DAL.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HotelManagement.Infrastructure.DAL.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly string _connectionString;

        public InvoiceRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null.");
        }
        public async Task<int?> AddAsync(Invoice invoice)
        {
            using (SqlCommand command = new SqlCommand("Sp_AddNewInvoice"))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@BookingID", invoice.BookingID);
                command.Parameters.AddWithValue("@CustomerID", invoice.CustomerID);
                command.Parameters.AddWithValue("@InvoiceDate", invoice.InvoiceDate);
                command.Parameters.AddWithValue("@AmountPaid", invoice.AmountPaid);
                command.Parameters.AddWithValue("@TotalAmount", invoice.TotalAmount);
                command.Parameters.AddWithValue("@GeneratedByEmployeeID", invoice.GeneratedByEmployeeID);

                command.Parameters.AddWithValue("@InvoiceStatus", (int)invoice.InvoiceStatus);

                // Output parameter for the new InvoiceID
                command.Parameters.AddWithValue("@InvoiceID", SqlDbType.Int).Direction = ParameterDirection.Output;
                return await PrimaryFunctions.AddAsync(command, _connectionString, "@InvoiceID");

            }
        }

        //I will do later when the class of Invoice item is comleted.
        public async Task<decimal?> CalculateInvoiceTotalAsync(int invoiceId)
        { 

            using (SqlCommand command = new SqlCommand("Sp_CalculateInvoiceTotalAsync"))
            {
                command.Parameters.AddWithValue("@InvoiceID", invoiceId);
                command.CommandType = CommandType.StoredProcedure;

                return await PrimaryFunctions.CalculateInvoiceTotalAsync(command, _connectionString);
           
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (SqlCommand command = new SqlCommand("Sp_DeleteInvoices"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@InvoiceID", id);
              return   await PrimaryFunctions.DeleteAsync(command, _connectionString);
            }
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            List<Invoice> invoices = new List<Invoice>();
            using (SqlCommand command = new SqlCommand("Sp_GetAllInvoices"))
            {
                command.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString);
                while (reader.Read())
                {
                    invoices.Add(MapToInvoice(reader));
                }

            }
            return invoices;
        }

        private Invoice MapToInvoice(SqlDataReader reader)
        {
            return new Invoice(
                invoiceID: reader.GetInt32(reader.GetOrdinal(nameof(Invoice.InvoiceID))),
                bookingID: reader.GetInt32(reader.GetOrdinal(nameof(Invoice.BookingID))),
                customerID: reader.GetInt32(reader.GetOrdinal(nameof(Invoice.CustomerID))),
                invoiceDate: reader.GetDateTime(reader.GetOrdinal(nameof(Invoice.InvoiceDate))),
                totalAmount: reader.GetDecimal(reader.GetOrdinal(nameof(Invoice.TotalAmount))),
                amountPaid: reader.GetDecimal(reader.GetOrdinal(nameof(Invoice.AmountPaid))),
                generatedByEmployeeID: reader.GetInt32(reader.GetOrdinal(nameof(Invoice.GeneratedByEmployeeID))),
                invoiceStatus: (InvoiceStatusEnum)reader.GetInt32(reader.GetOrdinal(nameof(Invoice.InvoiceStatus)))// تحويل int إلى Enum
                );
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByCustomerIdAsync(int customerId)
        {
            List<Invoice> invoices = new List<Invoice>();
            using (SqlCommand command = new SqlCommand("Sp_GetInvoicesByCustomerId"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CustomerID", customerId);
                try
                {
                    using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                    {
                        // 4. Read data and map to objects
                        while (await reader.ReadAsync()) // Use ReadAsync() for asynchronous reading
                        {
                            invoices.Add(MapToInvoice(reader));
                        }
                    } // The SqlDataReader is disposed here

                    return invoices; // Return the list of invoices (empty if none found)
                }
                catch (SqlException sqlEx)
                {
                    // This catch block handles SQL exceptions that PrimaryFunctions.GetAsync might re-throw.
                    // Examples: connection errors, invalid SP name, SP syntax errors.
                    // PrimaryFunctions.GetAsync should already log these.
                    throw new ApplicationException($"Database error retrieving invoices for customer ID {customerId}.", sqlEx);
                }
                catch (Exception ex)
                {
                    // Catch any other unexpected exceptions. PrimaryFunctions should ideally catch most.
                    throw new ApplicationException($"An unexpected error occurred while retrieving invoices for customer ID {customerId}.", ex);
                }
            }
        }

        public async Task<bool> UpdateAsync(Invoice invoice) 
        {
            using (SqlCommand command = new SqlCommand("Sp_UpdateInvoice"))
            {
                command.CommandType = CommandType.StoredProcedure;

                // ربط خصائص كائن الفاتورة بمعاملات الإجراء المخزن
                command.Parameters.AddWithValue("@InvoiceID", invoice.InvoiceID);
                command.Parameters.AddWithValue("@TotalAmount", invoice.TotalAmount);
                command.Parameters.AddWithValue("@AmountPaid", invoice.AmountPaid);
                command.Parameters.AddWithValue("@GeneratedByEmployeeID", invoice.GeneratedByEmployeeID);
                command.Parameters.AddWithValue("@InvoiceStatus", (int)invoice.InvoiceStatus); // تحويل Enum إلى int

                try
                {
                    // استدعاء دالة UpdateAsync العامة. هذه الدالة سترمي SqlException
                    // إذا كان الإجراء المخزن أرسل RAISERROR (مثلاً، الفاتورة غير موجودة).
                    await PrimaryFunctions.UpdateAsync(command, _connectionString);
                    // إذا لم يتم رمي أي استثناء، فهذا يعني أن التحديث قد تم بنجاح
                    return true;
                }
                catch (SqlException sqlEx)
                {
                    // ترجمة أخطاء SQL الخاصة بعدم وجود السجل إلى 'false'
                    if (sqlEx.Message.Contains("No invoice found with the provided ID to update.") ||
                        sqlEx.Message.Contains("No records were actually updated for the provided ID (after initial check)."))
                    {
                        return false; // الإبلاغ بأن الفاتورة لم توجد/لم تحدث
                    }
                    // لأي أخطاء SqlException أخرى (مثل انتهاك قيود، أخطاء في البيانات)،
                    // أعد رمي الاستثناء للسماح لطبقة الخدمة بالتعامل معها كخطأ فني
                    throw;
                }
                // لا حاجة لـ catch عام هنا، PrimaryFunctions.UpdateAsync ستقوم بالتقاط كل شيء آخر.
            }
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID must be greater than zero.", nameof(id));
            }
            using (SqlCommand command = new SqlCommand("Sp_GetInvoicesByID"))
            {
                command.Parameters.AddWithValue("@InvoiceID", id);
                command.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                {
                    if (await reader.ReadAsync())
                    {
                        return MapToInvoice(reader);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}
