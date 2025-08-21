using HotelManagement.Domain.Entities;

namespace HotelManagement.Infrastructure.DAL.Interfaces
{
    public interface IInvoiceRepository
    {
       
        
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task <int?> AddAsync(Invoice invoice);
        Task <bool> UpdateAsync(Invoice invoice);
        Task<bool> DeleteAsync(int id);
        Task<Invoice?> GetByIdAsync(int id);
        Task<IEnumerable<Invoice>> GetInvoicesByCustomerIdAsync(int customerId);
        Task<decimal?> CalculateInvoiceTotalAsync(int invoiceId);
        Task<IEnumerable<Invoice?>> GetInvoicesByBookingIdAsync(int bookingId);

        Task<int?> GetBookingIdByInvoiceIdAsync(int invoiceId);
    }
}
