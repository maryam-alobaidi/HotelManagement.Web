
using HotelManagement.Domain.Entities;

namespace HotelManagement.Infrastructure.DAL.Interfaces
{
    public interface IInvoiceItemRepository
    {
        Task<int?> AddAsync(InvoiceItem invoiceItem);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(InvoiceItem invoiceItem);
        Task<IEnumerable<InvoiceItem>> GetAllAsync();
        Task<InvoiceItem?> GetByIdAsync(int id);

    }
}
