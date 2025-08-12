
using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.DAL.Interfaces;
using HotelManagement.Infrastructure.DAL.Repositories;

namespace HotelManagement.BLL.Services
{
    public class InvoiceItemService : IInvoiceItemService
    {
        private readonly IInvoiceItemRepository _invoiceItemRepository;
        public InvoiceItemService(IInvoiceItemRepository invoiceItemRepository)
        {
            _invoiceItemRepository = invoiceItemRepository ?? throw new ArgumentNullException(nameof(invoiceItemRepository));
        }
        public async Task<int?> AddInvoiceItemAsync(InvoiceItem invoiceItem)
        {
            if (invoiceItem == null) throw new ArgumentNullException("invoiceItem", "Invoice Item cannot be null.");

            return await _invoiceItemRepository.AddAsync(invoiceItem);
        }

        public async Task<bool> DeleteInvoiceItemAsync(int invoiceItemId)
        {
            if (invoiceItemId <= 0) throw new ArgumentException("Invoice ID must be a positive integer.", nameof(invoiceItemId));
            return await _invoiceItemRepository.DeleteAsync(invoiceItemId);
        }

        public async Task<IEnumerable<InvoiceItem>> GetAllInvoiceItemAsync()
        {
           return await _invoiceItemRepository.GetAllAsync();
        }

        public async Task<InvoiceItem?> GetInvoiceItemByIdAsync(int invoiceItemId)
        {
            if (invoiceItemId <= 0) throw new ArgumentException("Invoice ID must be a positive integer.", nameof(invoiceItemId));
            return await _invoiceItemRepository.GetByIdAsync(invoiceItemId);
        }

        public async Task<bool> UpdateInvoiceItemAsync(InvoiceItem invoiceItem)
        {
            if (invoiceItem.InvoiceItemID <= 0) throw new ArgumentException("Invoice ID must be a positive integer.", nameof(invoiceItem.InvoiceItemID));
          

            if (invoiceItem.InvoiceID <= 0)
                throw new ArgumentException("Invoice ID must be a positive integer.", nameof(invoiceItem.InvoiceID));
          
            try
            {
                // 3. استدعاء طبقة المستودع لتنفيذ التحديث في قاعدة البيانات
                bool updateSuccessful = await _invoiceItemRepository.UpdateAsync(invoiceItem);

                // 4. معالجة النتيجة من طبقة المستودع
                if (!updateSuccessful)
                {
                   
                    throw new KeyNotFoundException($"Invoice item  with ID {invoiceItem.InvoiceItemID} was not found or could not be updated.");
                }

                return true; 
            }
            catch (Exception ex)
            {
               
                throw new ApplicationException($"An error occurred while trying to update invoice ID {invoiceItem.InvoiceItemID}.", ex);
            }


        }
    }
}
