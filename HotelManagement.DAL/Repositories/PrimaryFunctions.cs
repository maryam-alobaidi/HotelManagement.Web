using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace HotelManagement.Infrastructure.DAL.Repositories
{
    public class PrimaryFunctions
    {
        public static async Task<int?> AddAsync(SqlCommand command, string ConnectionString, string outputParameterName)
        {
            int? ID = null;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                command.Connection = connection;

                try
                {
                    await connection.OpenAsync();

                    // تنفيذ الإجراء المخزن. بما أننا نتوقع معامل إخراج، نستخدم ExecuteNonQueryAsync().
                    await command.ExecuteNonQueryAsync();

                    // قراءة قيمة معامل الإخراج المحدد بواسطة 'outputParameterName'
                    SqlParameter? outputParam = command.Parameters[outputParameterName] as SqlParameter;

                    if (outputParam != null && outputParam.Value != DBNull.Value)
                    {
                        // حاول التحويل إلى int
                        if (int.TryParse(outputParam.Value.ToString(), out int insertedID))
                        {
                            ID = insertedID;
                        }
                        else
                        {
                            // سجل أو ارمِ استثناء إذا كانت القيمة ليست عددًا صحيحًا صالحًا
                          WriteErrorToEventLog  ($"Warning: Output parameter '{outputParameterName}' value '{outputParam.Value}' could not be parsed as an integer.");
                        }
                    }
                    else
                    {
                        // سجل أو ارمِ استثناء إذا كان معامل الإخراج غير موجود أو قيمته DBNull
                        WriteErrorToEventLog($"Warning: Output parameter '{outputParameterName}' not found or its value is DBNull.Value.");
                    }
                }
                catch (Exception ex)
                {
                    WriteErrorToEventLog($"Error executing command '{command.CommandText}': {ex.Message} - {ex.StackTrace}");
                    throw; // أعد رمي الاستثناء الأصلي
                }
                return ID;
            }
        }


      
        public static async Task<bool> DeleteAsync(SqlCommand command, string ConnectionString)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                command.Connection = connection;

                try
                {
                    await connection.OpenAsync();
                    int rowAffected = await command.ExecuteNonQueryAsync();

                    // If your SP did NOT use RAISERROR for @@ROWCOUNT=0,
                    // this line would determine success/failure.
                    // However, with RAISERROR, rowAffected will either be >0 or an exception is thrown.
                    return rowAffected > 0;
                }
                catch (SqlException sqlEx)
                {
                    WriteErrorToEventLog($"SQL Error executing command: {command.CommandText}. Parameters: {GetCommandParametersLog(command)}. Error: {sqlEx.Message} - {sqlEx.StackTrace}");

                    // Check if this specific SqlException is the one from your RAISERROR for no records found.
                    // You might check sqlEx.Message, or if you use specific error numbers in RAISERROR.
                    // For example, if you set RAISERROR('...', 16, 1, 50001); you could check sqlEx.Number == 50001
                    if (sqlEx.Message.Contains("No records found to delete for the provided ID."))
                    {
                        // If the SP explicitly said no records were found, return false.
                        // Or, if your business logic demands, you could throw KeyNotFoundException here.
                        // For a Delete operation, returning false is often a clean way to indicate "not deleted because not found".
                        return false;
                    }
                    else
                    {
                        // For any other SQL-related error, re-throw the specific SqlException
                        // (or a custom exception wrapping it)
                        throw; // Re-throws the original SqlException
                    }
                }
                catch (Exception ex)
                {
                    // Catch any other non-SQL exceptions
                    WriteErrorToEventLog($"General Error executing command: {command.CommandText}. Parameters: {GetCommandParametersLog(command)}. Error: {ex.Message} - {ex.StackTrace}");
                    throw; // Re-throws the original exception
                }
            }
        }


        // Helper to log command parameters (optional, but good for debugging)
        private static string GetCommandParametersLog(SqlCommand command)
        {
            return string.Join(", ", command.Parameters.Cast<SqlParameter>().Select(p => $"{p.ParameterName} = {(p.Value == DBNull.Value ? "NULL" : p.Value)} (Direction: {p.Direction})"));
        }
       
        public static async Task<bool> UpdateAsync(SqlCommand command, string ConnectionString) // ترجع bool لتدل على عدد الصفوف المتأثرة
        {
            int rowAffected = 0;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                command.Connection = connection;

                try
                {
                    await connection.OpenAsync();
                    rowAffected = await command.ExecuteNonQueryAsync();
                }
                catch (SqlException sqlEx) // التقاط أخطاء SQL المحددة
                {
                    // تسجيل الخطأ الفني في سجل الأحداث (للتصحيح والمراقبة)
                    WriteErrorToEventLog($"SQL Error executing command '{command.CommandText}': {sqlEx.Message} - {sqlEx.StackTrace} - SQL Error Number: {sqlEx.Number}");

                    // إعادة رمي SqlException لطبقة المستودع للتعامل معها بشكل أكثر تحديداً
                    throw;
                }
                catch (Exception ex) // التقاط أي استثناءات أخرى غير SqlException
                {
                    // تسجيل الأخطاء العامة
                    WriteErrorToEventLog($"General Error executing command '{command.CommandText}': {ex.Message} - {ex.StackTrace}");

                    // رمي استثناء جديد أو إعادة رمي الأصلي كخطأ عام غير متوقع
                    throw new Exception("An unexpected error occurred during the update database operation.", ex);
                }
            }
            // إرجاع True إذا تم تحديث صف واحد على الأقل، False إذا لم يتم تحديث أي صف
            // (هذا لن يحدث عملياً في حالة RAISERROR من SP، بل عندما لا يتم العثور على سجل ولم يرمِ SP خطأ)
            return rowAffected > 0;
        }

        public static async Task<SqlDataReader> GetAsync(SqlCommand command, string ConnectionString)
        {

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (command)
                {
                    try
                    {
                        command.Connection = connection;
                        await connection.OpenAsync();


                        return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                    }
                    catch (Exception ex)
                    {
                        WriteErrorToEventLog($"Error executing command:  {ex.Message}  -  {ex.StackTrace}");
                        throw new Exception("GetAsync operation failed!", ex);
                    }

                }
            }
        }

        public static void WriteErrorToEventLog(string Message)
        {

            string source = "HotelManagementApp";
            string logName = "Application";

            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, logName);
            }

            EventLog eventLog = new EventLog(logName);
            eventLog.Source = source;
            eventLog.WriteEntry(Message, EventLogEntryType.Error);
        }

        public static async Task<bool> IsRoomAvailable(SqlCommand command, string ConnectionString)
        {

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                command.Connection = connection;
                try
                {
                    await connection.OpenAsync();

                    object? result = await command.ExecuteScalarAsync();
                    if (result != null && int.TryParse(result.ToString(), out int count))
                    {
                        return count == 0; // If count is 0, room is available.  
                    }
                    else
                    {
                        throw new Exception("Unexpected result from database query.");
                    }
                }
                catch (Exception ex)
                {

                    WriteErrorToEventLog($"Error executing command: {ex.Message} - {ex.StackTrace}");
                    throw new Exception("Room not available failed !!", ex);
                }
            }
        }

        public static async Task<decimal> GetTotalPaidForBookingAsync(SqlCommand command, string ConnectionString)
        {
            decimal TotalPaid = 0;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                command.Connection = connection;

                try
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    SqlParameter outputParam = command.Parameters["@TotalPaidAmount"] as SqlParameter; // الحصول على المعامل بواسطة اسمه
                    if (outputParam != null && outputParam.Value != DBNull.Value)
                    {
                        TotalPaid = Convert.ToDecimal(outputParam.Value);
                    }  
                }
                catch (Exception ex)
                {
                    WriteErrorToEventLog($"Error executing command: {ex.Message} - {ex.StackTrace}");
                    throw; 
                }
                return TotalPaid;
            }
        }


        public static async Task<decimal?> CalculateInvoiceTotalAsync(SqlCommand command, string ConnectionString)
        {
            decimal? TotalInvoice = null;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                command.Connection = connection;

                try
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    SqlParameter outputParam = command.Parameters["@TotalInvoice"] as SqlParameter; // الحصول على المعامل بواسطة اسمه
                    if (outputParam != null && outputParam.Value != DBNull.Value)
                    {
                        TotalInvoice = Convert.ToDecimal(outputParam.Value);
                    }
                }
                catch (Exception ex)
                {
                    WriteErrorToEventLog($"Error executing command: {ex.Message} - {ex.StackTrace}");
                    throw;
                }
                return TotalInvoice;
            }
        }
    }
}
