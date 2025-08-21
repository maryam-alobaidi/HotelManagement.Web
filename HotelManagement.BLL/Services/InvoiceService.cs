
using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Domain.Enums;
using HotelManagement.Infrastructure.DAL.Interfaces;
using HotelManagement.Infrastructure.DAL.Repositories;
using Microsoft.Data.SqlClient;

namespace HotelManagement.BLL.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        public InvoiceService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository ?? throw new ArgumentNullException(nameof(invoiceRepository));
        }

        public async Task<int?> AddInvoiceAsync(Invoice invoice)
        {
           if(invoice== null) throw new ArgumentNullException("invoice", "Invoice cannot be null.");
           
            return await _invoiceRepository.AddAsync(invoice);
        }

        public async Task<decimal?> CalculateInvoiceTotalAsync(int invoiceId)
        {
           if(invoiceId <= 0) throw new ArgumentException("Invoice ID must be a positive integer.", nameof(invoiceId));

            decimal? total= await _invoiceRepository.CalculateInvoiceTotalAsync(invoiceId);
            return total ?? 0.0m;
        }

        public async Task<bool> DeleteInvoiceAsync(int invoiceId)
        {
          if(invoiceId <= 0) throw new ArgumentException("Invoice ID must be a positive integer.", nameof(invoiceId));
            return await _invoiceRepository.DeleteAsync(invoiceId);
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
        {
            return await _invoiceRepository.GetAllAsync();
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int invoiceId)
        {
          if(invoiceId <= 0) throw new ArgumentException("Invoice ID must be a positive integer.", nameof(invoiceId));
            return await _invoiceRepository.GetByIdAsync(invoiceId);
        }

        public async Task<IEnumerable< Invoice?>> GetInvoicesBybookingIdAsync(int bookingId)
        {
            if(bookingId <= 0) throw new ArgumentException("Booking ID must be a positive integer.", nameof(bookingId));
            return  await _invoiceRepository.GetInvoicesByBookingIdAsync(bookingId);
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByCustomerIdAsync(int customerId)
        {
            if(customerId <= 0) throw new ArgumentException("Customer ID must be a positive integer.", nameof(customerId));
            return await _invoiceRepository.GetInvoicesByCustomerIdAsync(customerId);
        }

        public async Task<bool> MarkInvoiceAsPaidAsync(int invoiceId, decimal amountPaid)
        {
            // 1. Input Validation
            if (invoiceId <= 0)
                throw new ArgumentException("Invoice ID must be a positive integer.", nameof(invoiceId));
            if (amountPaid < 0)
                throw new ArgumentException("Payment amount cannot be negative.", nameof(amountPaid));

            try
            {
                // 2. Retrieve the existing invoice to apply business rules
                Invoice? existingInvoice = await _invoiceRepository.GetByIdAsync(invoiceId);

                if (existingInvoice == null)
                {
                    // Invoice not found, this is a business outcome
                    return false; // Or throw new KeyNotFoundException($"Invoice with ID {invoiceId} not found.");
                }

                // 3. Apply Business Rules for marking as paid
                // Example Rule 1: Cannot mark as paid if already fully paid and paymentAmount is 0 (no new payment)
                if (existingInvoice.InvoiceStatus == InvoiceStatusEnum.Paid && amountPaid == 0)
                {
                    // You might choose to return true if it's already paid and no new payment is given,
                    // indicating the desired state is already met. Or throw an error if it's strictly
                    // about *making* a payment. For "mark as paid", returning true makes sense.
                    return true;
                }

                // Example Rule 2: Update amountPaid and status
                decimal? newAmountPaid = existingInvoice.AmountPaid + amountPaid;
                InvoiceStatusEnum newStatus = existingInvoice.InvoiceStatus;

                if (newAmountPaid >= existingInvoice.TotalAmount)
                {
                    newStatus = InvoiceStatusEnum.Paid;
                    newAmountPaid = existingInvoice.TotalAmount; // Ensure AmountPaid doesn't exceed TotalAmount
                }
                else if (newAmountPaid > 0)
                {
                    newStatus = InvoiceStatusEnum.PartiallyPaid;
                }
                else if (newAmountPaid == 0)
                {
                    newStatus = InvoiceStatusEnum.Unpaid; // If no amount paid, revert to unpaid
                }


                // 4. Update the existing invoice object with new values
                // (You might need a method to update properties or create a new Invoice if it's immutable)
                existingInvoice.AmountPaid = newAmountPaid;
                existingInvoice.InvoiceStatus = newStatus;
                // You might need to update GeneratedByEmployeeID if the person making payment is different
                // existingInvoice.GeneratedByEmployeeID = currentUser.EmployeeId; // Example

                // 5. Call the generic UpdateInvoiceAsync in the repository
                bool updateSuccess = await _invoiceRepository.UpdateAsync(existingInvoice);

                // 6. Handle repository result for higher layers
                if (!updateSuccess)
                {
                    // This means the invoice disappeared between Get and Update (race condition)
                    // Or a different underlying DB error that UpdateAsync didn't convert to 'false'.
                    throw new ApplicationException($"Failed to update invoice ID {invoiceId} after processing payment.");
                }

                return true; // Successfully marked as paid/updated
            }
            catch (SqlException sqlEx)
            {
                // This catches specific DB errors that the repository might re-throw (e.g., foreign key violations).
                if (sqlEx.Number == 547) // Foreign key constraint violation (example for other methods)
                {
                    throw new InvalidOperationException($"Cannot update invoice ID {invoiceId} due to data integrity issues.", sqlEx);
                }
                throw new ApplicationException($"A database error occurred while processing payment for invoice ID {invoiceId}.", sqlEx);
            }
            catch (Exception ex)
            {
                // Catch any other general exceptions and wrap them
                throw new ApplicationException($"An unexpected error occurred while marking invoice ID {invoiceId} as paid.", ex);
            }
        }
        
        public async Task<bool> UpdateInvoiceAsync(Invoice invoice) // ترجع Task<bool> للإشارة إلى النجاح/الفشل
        {
            // 1. التحقق من صحة المدخلات (Input Validation)
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice), "Invoice cannot be null.");
            if (invoice.InvoiceID <= 0)
                throw new ArgumentException("Invoice ID must be a positive integer.", nameof(invoice.InvoiceID));
            if (invoice.TotalAmount < 0)
                throw new ArgumentException("Total amount cannot be negative.", nameof(invoice.TotalAmount));

           

            try
            {
                // 3. استدعاء طبقة المستودع لتنفيذ التحديث في قاعدة البيانات
                bool updateSuccessful = await _invoiceRepository.UpdateAsync(invoice);

                // 4. معالجة النتيجة من طبقة المستودع
                if (!updateSuccessful)
                {
                   
                    throw new KeyNotFoundException($"Invoice with ID {invoice.InvoiceID} was not found or could not be updated.");
                }

                return true; 
            }
            catch (Exception ex)
            {
               
                throw new ApplicationException($"An error occurred while trying to update invoice ID {invoice.InvoiceID}.", ex);
            }
        }


        public async Task<int?> GetBookingIdByInvoiceIdAsync(int invoiceId)
        {
            return await _invoiceRepository.GetBookingIdByInvoiceIdAsync(invoiceId);
        }

       
    }
}
