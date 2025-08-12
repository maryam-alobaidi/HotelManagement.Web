
using HotelManagement.Domain.Entities;
using HotelManagement.Domain.Enums;
using HotelManagement.Infrastructure.DAL.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HotelManagement.Infrastructure.DAL.Repositories
{
    public class InvoiceItemRepository : IInvoiceItemRepository
    {
        private readonly string _connectionString;
        public InvoiceItemRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null.");
        }


        public async Task<int?> AddAsync(InvoiceItem invoiceItem)
        {
            using (SqlCommand command = new SqlCommand("Sp_AddNewInvoiceItem"))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@InvoiceID", invoiceItem.InvoiceID);
                command.Parameters.AddWithValue("@ItemDescription", invoiceItem.ItemDescription);
                command.Parameters.AddWithValue("@Quantity", invoiceItem.Quantity);
                command.Parameters.AddWithValue("@UnitPrice", invoiceItem.UnitPrice);
                command.Parameters.AddWithValue("@ItemType",(int) invoiceItem.ItemType);


                // Output parameter for the new InvoiceItemID
                command.Parameters.AddWithValue("@InvoiceItemID", SqlDbType.Int).Direction = ParameterDirection.Output;
                return await PrimaryFunctions.AddAsync(command, _connectionString, "@InvoiceItemID");
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (SqlCommand command = new SqlCommand("Sp_DeleteInvoiceItems"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@InvoiceItemID", id);
                return await PrimaryFunctions.DeleteAsync(command, _connectionString);
            }
        }

        public async Task<bool> UpdateAsync(InvoiceItem invoiceItem)
        {
            using (SqlCommand command = new SqlCommand("Sp_UpdateInvoiceItem"))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@InvoiceItemID", invoiceItem.InvoiceItemID);
                command.Parameters.AddWithValue("@InvoiceID", invoiceItem.InvoiceID);
                command.Parameters.AddWithValue("@ItemDescription", invoiceItem.ItemDescription);
                command.Parameters.AddWithValue("@Quantity", invoiceItem.Quantity);
                command.Parameters.AddWithValue("@UnitPrice", invoiceItem.UnitPrice);
                command.Parameters.AddWithValue("@ItemType", (int)invoiceItem.ItemType);


                try
                {
                    // استدعاء دالة UpdateAsync العامة. هذه الدالة سترمي SqlException
                    // إذا كان الإجراء المخزن أرسل RAISERROR (مثلاً،عناصر الفاتورة غير موجودة).
                    await PrimaryFunctions.UpdateAsync(command, _connectionString);
                    // إذا لم يتم رمي أي استثناء، فهذا يعني أن التحديث قد تم بنجاح
                    return true;
                }
                catch (SqlException sqlEx)
                {
                    // ترجمة أخطاء SQL الخاصة بعدم وجود السجل إلى 'false'
                    if (sqlEx.Message.Contains("No Invoice Item found with the provided ID to update.") ||
                             sqlEx.Message.Contains("No records found to update for the provided ID."))
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

        public async Task<IEnumerable<InvoiceItem>> GetAllAsync()
        {
            List<InvoiceItem> invoiceItems = new List<InvoiceItem>();
            using (SqlCommand command = new SqlCommand("Sp_GetAllInvoiceItems"))
            {
                command.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString);
                while (reader.Read())
                {
                    invoiceItems.Add(MapToInvoiceItem(reader));
                }
            }
            return invoiceItems;
        }

        private InvoiceItem MapToInvoiceItem(SqlDataReader reader)
        {
            return new InvoiceItem(
                invoiceItemID: reader.GetInt32(reader.GetOrdinal(nameof(InvoiceItem.InvoiceItemID))),
                invoiceID: reader.GetInt32(reader.GetOrdinal(nameof(InvoiceItem.InvoiceID))),
                itemDescription: reader.GetString(reader.GetOrdinal(nameof(InvoiceItem.ItemDescription))),
                quantity: reader.GetInt32(reader.GetOrdinal(nameof(InvoiceItem.Quantity))),
                unitPrice: reader.GetDecimal(reader.GetOrdinal(nameof(InvoiceItem.UnitPrice))),
                lineTotal: reader.GetDecimal(reader.GetOrdinal(nameof(InvoiceItem.LineTotal))),
                itemType: (InvoiceItemTypeEnum)reader.GetInt32(reader.GetOrdinal(nameof(InvoiceItem.ItemType)))
            );
        }

        public async Task<InvoiceItem?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID must be greater than zero.", nameof(id));
            }
            using (SqlCommand command = new SqlCommand("Sp_GetInvoiceItemsByID"))
            {
                command.Parameters.AddWithValue("@InvoiceItemID", id);
                command.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader reader = await PrimaryFunctions.GetAsync(command, _connectionString))
                {
                    if (await reader.ReadAsync())
                    {
                        return MapToInvoiceItem(reader);
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
