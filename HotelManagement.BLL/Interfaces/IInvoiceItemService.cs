

using HotelManagement.Domain.Entities;

namespace HotelManagement.BLL.Interfaces
{
    public interface IInvoiceItemService
    {
        Task<int?> AddInvoiceItemAsync(InvoiceItem invoiceItem);
        Task<bool> UpdateInvoiceItemAsync(InvoiceItem invoiceItem);
        Task<bool> DeleteInvoiceItemAsync(int invoiceItemId);
        Task<InvoiceItem?> GetInvoiceItemByIdAsync(int invoiceItemId);
        Task<IEnumerable<InvoiceItem>> GetAllInvoiceItemAsync();
        
    }
}
